using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Utils.P2PLib;
using Utils.P2PRoomLib.Data;
using Utils.WebRequestSender;

namespace Utils.P2PRoomLib
{
    public class P2PRoom : IDisposable
    {
        public event Action<IP2PConnection> PeerConnected;
        public event Action<IP2PConnection, string> MessageReceivedFrom;
        public event Action<string> MessageReceived;
        public event Action<IP2PConnection> PeerDisconnected;
        
        private readonly string _serviceUrl;
        private readonly List<IP2PHostSideConnection> _hostedConnections = new();
        private readonly List<IP2PJoinSideConnection> _joiningConnections = new();
        private readonly List<IP2PConnection> _activeConnections = new();
        private readonly List<IP2PConnection> _closedConnections = new();
        private readonly CancellationTokenSource _disposeCts = new();

        private CancellationTokenSource _joiningLoopTcs;
        private string _getConnectingChannelsUrl;
        private int _maxConnectionsCount;
        private bool _isJoinAllowed = true;

        public P2PRoom(string serviceUrl)
        {
            _serviceUrl = serviceUrl;
        }

        public int RoomId { get; private set; }
        public bool IsRoomCreated => RoomId > 0;
        private string CreateRoomUrl => _serviceUrl + "?command=create_room";
        private string AddHostChannelsUrl => _serviceUrl + $"?command=add_host_channels&room_id={RoomId}";
        private string ReserveFreeChannelUrl => _serviceUrl + $"?command=reserve_free_channel&room_id={RoomId}";
        private string ConnectToRoomUrl => _serviceUrl + $"?command=connect_to_room&room_id={RoomId}";
        public bool IsDisposed => _disposeCts.IsCancellationRequested;
        public IReadOnlyList<IP2PConnection> ActiveConnections => _activeConnections;
        public bool IsJoinAllowed
        {
            get => _isJoinAllowed;
            set => SetIsJoinAllowed(value);
        }
        public bool IsHostRoom { get; private set; }

        private bool IsJoinLoopActive => _joiningLoopTcs is { IsCancellationRequested: false };

        public async UniTask<bool> Create(int maxConnectionsCount)
        {
            _maxConnectionsCount = maxConnectionsCount;
            IsHostRoom = true;

            var stopToken = _disposeCts.Token;
            var newConnections = await PrepareMissingHostConnections(stopToken);
            
            if (newConnections.Length > 0
                && stopToken.IsCancellationRequested == false)
            {
                var descriptionsStr = string.Join(",", _hostedConnections.Select(c => c.ConnectionLocalDescription));

                var createRoomResponse = await WebRequestsSender.PostAsync<P2PCreateRoomResponseDto>(CreateRoomUrl,
                    new Dictionary<string, string>
                    {
                        { "channel_keys", descriptionsStr }
                    });

                if (createRoomResponse.IsSuccess
                    && createRoomResponse.Result.IsNoError
                    && stopToken.IsCancellationRequested == false)
                {
                    HandleRoomCreated(createRoomResponse.Result);
                }
                else
                {
                    _hostedConnections.RemoveAll(newConnections.Contains);
                    DisposeConnections(newConnections);
                }
            }

            return IsRoomCreated;
        }

        public async UniTask<bool> Join(int roomId)
        {
            var result = false;
            RoomId = roomId;
            var token = _disposeCts.Token;

            var reserveFreeChannelResponse = await WebRequestsSender.GetAsync<P2PReserveFreeChannelResponseDto>(ReserveFreeChannelUrl);
            
            if (reserveFreeChannelResponse.IsSuccess
                && reserveFreeChannelResponse.Result.IsNoError
                && !token.IsCancellationRequested)
            {
                var channelData = reserveFreeChannelResponse.Result.Data;

                var joinConnection = await P2PConnection.Join(channelData.ChannelKey);
                _joiningConnections.Add(joinConnection);
                var tsc = new UniTaskCompletionSource();
                
                var cancelChannelAction = new Action(() =>
                {
                    UnsubscribeFromConnection(joinConnection);
                    _joiningConnections.Remove(joinConnection);
                    joinConnection.Close();
                    tsc.TrySetResult();
                });
                
                if (token.IsCancellationRequested)
                {
                    cancelChannelAction();
                }
                else
                {
                    SubscribeOnConnection(joinConnection);
                    var cancellationRegistration = token.Register(cancelChannelAction);

                    var connectResponse = await WebRequestsSender
                        .GetAsync<P2PConnectToRoomResponseDto>(
                            ConnectToRoomUrl +
                            $"&channel_id={channelData.Id}&join_key={joinConnection.ConnectionLocalDescription}");

                    if (connectResponse.IsSuccess 
                        && !token.IsCancellationRequested)
                    {
                        await UniTask.WhenAny(joinConnection.ConnectionEstablishedTask, tsc.Task);
                    }

                    await cancellationRegistration.DisposeAsync();

                    if (!token.IsCancellationRequested)
                    {
                        result = connectResponse.IsSuccess;
                    }
                }
            }

            return result;
        }

        public void CancelJoiningLoop()
        {
            _joiningLoopTcs?.Cancel();
        }

        public void SendToAll(string message)
        {
            foreach (var activeConnection in _activeConnections)
            {
                activeConnection.SendMessage(message);
            }
        }

        public void Dispose()
        {
            DisposeAllConnections();
            _disposeCts.Cancel();
        }

        private async UniTask<IP2PHostSideConnection[]> PrepareMissingHostConnections(CancellationToken stopToken)
        {
            var newConnectionsCount = _maxConnectionsCount - _hostedConnections.Count - _activeConnections.Count;
            
            if (newConnectionsCount > 0)
            {
                var initConnectionTasks = new UniTask<IP2PHostSideConnection>[newConnectionsCount];
                for (var i = 0; i < initConnectionTasks.Length; i++)
                {
                    initConnectionTasks[i] = P2PConnection.HostInit();
                }

                var connections = await UniTask.WhenAll(initConnectionTasks);
            
                if (stopToken.IsCancellationRequested)
                {
                    DisposeConnections(connections);
                }
                else
                {
                    foreach (var connection in connections)
                    {
                        SubscribeOnConnection(connection);
                    }
                    
                    _hostedConnections.AddRange(connections);
                    
                    return connections;
                }
            }

            return Array.Empty<IP2PHostSideConnection>();
        }

        private async UniTaskVoid RunCheckJoinLoop()
        {
            if (IsJoinLoopActive) return;
            
            _joiningLoopTcs = new CancellationTokenSource();
            var stopToken = CancellationTokenSource.CreateLinkedTokenSource(_disposeCts.Token, _joiningLoopTcs.Token);
            
            var iterationsCount = 1000;
            while (iterationsCount > 0 
                   && stopToken.IsCancellationRequested == false)
            {
                iterationsCount--;

                await UniTask.Delay(2000, cancellationToken: stopToken.Token).SuppressCancellationThrow();

                if (stopToken.IsCancellationRequested
                    || _hostedConnections.All(c => c.ConnectionState == P2PConnectionState.Established))
                {
                    break;
                }

                var getJoiningResponse = await WebRequestsSender.GetAsync<P2PGetJoiningResponseDto>(_getConnectingChannelsUrl);

                if (getJoiningResponse.IsSuccess
                    && stopToken.IsCancellationRequested == false)
                {
                    if (getJoiningResponse.Result.IsNoError 
                        && getJoiningResponse.Result.Data.Length > 0)
                    {
                        foreach (var joiningDataDto in getJoiningResponse.Result.Data)
                        {
                            var connection = _hostedConnections.FirstOrDefault(c =>
                                c.ConnectionLocalDescription == joiningDataDto.ChannelKey
                                && c.ConnectionState == P2PConnectionState.Connecting);

                            connection?.HostComplete(joiningDataDto.JoinKey);
                        }
                    }
                    else if (getJoiningResponse.Result.ErrorCode == ErrorCodes.ERROR_ROOM_NOT_FOUND)
                    {
                        _joiningLoopTcs.Cancel();
                    }
                }
            }
        }

        private void DisposeAllConnections()
        {
            DisposeConnections(_hostedConnections);
            DisposeConnections(_joiningConnections);
            DisposeConnections(_activeConnections);
            
            _hostedConnections.Clear();
            _joiningConnections.Clear();
            _activeConnections.Clear();
        }

        private void DisposeConnections(IEnumerable<IP2PConnection> connections)
        {
            foreach (var connection in connections)
            {
                DisposeConnection(connection);
            }
        }

        private void DisposeConnection(IP2PConnection connection)
        {
            UnsubscribeFromConnection(connection);
            connection.Close();
        }

        private void HandleRoomCreated(P2PCreateRoomResponseDto createRoomResponse)
        {
            RoomId = createRoomResponse.Data.RoomId;
            _getConnectingChannelsUrl = _serviceUrl + $"?command=get_connecting&room_id={RoomId}";
            
            RunCheckJoinLoop().Forget();
        }

        private void SetIsJoinAllowed(bool isAllowed)
        {
            _isJoinAllowed = isAllowed;
            UpdateJoinLoopState();
        }

        private void UpdateJoinLoopState()
        {
            if (_isJoinAllowed)
            {
                RunCheckJoinLoop().Forget();
            }
            else if (IsJoinLoopActive)
            {
                _joiningLoopTcs.Cancel();
            }
        }

        private void SubscribeOnConnection(IP2PConnection connection)
        {
            UnsubscribeFromConnection(connection);
            connection.ConnectionOpened += OnConnectionOpened;
            connection.MessageReceived += OnMessageReceived;
            connection.ConnectionClosed += OnConnectionClosed;
        }

        private void UnsubscribeFromConnection(IP2PConnection connection)
        {
            connection.ConnectionOpened -= OnConnectionOpened;
            connection.MessageReceived -= OnMessageReceived;
            connection.ConnectionClosed -= OnConnectionClosed;
        }

        private void OnConnectionOpened(P2PConnection connection)
        {
            _joiningConnections.Remove(connection);
            _hostedConnections.Remove(connection);
            _activeConnections.Add(connection);

            PeerConnected?.Invoke(connection);
        }

        private void OnMessageReceived(P2PConnection connection, string message)
        {
            MessageReceived?.Invoke(message);
            MessageReceivedFrom?.Invoke(connection, message);
        }

        private void OnConnectionClosed(P2PConnection connection)
        {
            _activeConnections.Remove(connection);
            _closedConnections.Add(connection);
            DisposeConnection(connection);

            PeerDisconnected?.Invoke(connection);

            if (IsJoinAllowed)
            {
                RunAddMissingHostConnectionsFlow().Forget();
            }
        }

        private async UniTaskVoid RunAddMissingHostConnectionsFlow()
        {
            var stopToken = _disposeCts.Token;
            
            var newConnections = await PrepareMissingHostConnections(stopToken);
            
            if (newConnections.Length > 0
                && stopToken.IsCancellationRequested == false)
            {
                var currentConnectionKeysStr = string.Join(",",
                    _activeConnections.Concat(_closedConnections).Select(c => c.ConnectionLocalDescription));
                var newConnectionKeysStr = string.Join(",", newConnections.Select(c => c.ConnectionLocalDescription));

                var addHostChannelsResponse = await WebRequestsSender.PostAsync<P2PAddHostChannelResponseDto>(AddHostChannelsUrl,
                    new Dictionary<string, string>
                    {
                        { "existing_channel_keys", currentConnectionKeysStr },
                        { "new_channel_keys", newConnectionKeysStr },
                    });

                if (addHostChannelsResponse.IsSuccess
                    && addHostChannelsResponse.Result.IsNoError
                    && stopToken.IsCancellationRequested == false)
                {
                    RunCheckJoinLoop().Forget();
                }
                else
                {
                    _hostedConnections.RemoveAll(newConnections.Contains);
                    DisposeConnections(newConnections);
                }
            }
        }
    }
}