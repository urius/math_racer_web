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
        private readonly CancellationTokenSource _disposeCts = new();

        private CancellationTokenSource _joiningLoopTcs;
        private string _getReservedChannelsUrl;

        public P2PRoom(string serviceUrl)
        {
            _serviceUrl = serviceUrl;
        }

        public int RoomId { get; private set; }
        public bool IsRoomCreated => RoomId > 0;
        private string CreateRoomUrl => _serviceUrl + "?command=create_room";
        private string ReserveFreeChannelUrl => _serviceUrl + $"?command=reserve_free_channel&room_id={RoomId}";
        private string ConnectToRoomUrl => _serviceUrl + $"?command=connect_to_room&room_id={RoomId}";
        public bool IsDisposed => _disposeCts.IsCancellationRequested;

        public async UniTask<bool> Create(int maxConnectionsCount, CancellationToken cancellationToken)
        {
            var linkedTcs = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposeCts.Token);
            var stopToken = linkedTcs.Token;
            
            var initConnectionTasks = new UniTask<IP2PHostSideConnection>[maxConnectionsCount];
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
                _hostedConnections.AddRange(connections);
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
                    HandleRoomCreated(createRoomResponse.Result, stopToken);
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

                var joinConnection = await P2PConnection.Join(channelData.channel_key);
                _joiningConnections.Add(joinConnection);
                var tsc = new UniTaskCompletionSource();
                
                var cancelChannelAction = new Action(() =>
                {
                    UnsubscribeFromChannelOpened(joinConnection);
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
                    SubscribeOnChannelOpened(joinConnection);
                    var cancellationRegistration = token.Register(cancelChannelAction);

                    var connectResponse = await WebRequestsSender
                        .GetAsync<P2PConnectToRoomResponseDto>(
                            ConnectToRoomUrl +
                            $"&channel_id={channelData.id}&join_key={joinConnection.ConnectionLocalDescription}");

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

        private void HandleRoomCreated(P2PCreateRoomResponseDto createRoomResponse, CancellationToken cancellationToken)
        {
            RoomId = createRoomResponse.Data.room_id;
            _getReservedChannelsUrl = _serviceUrl + $"?command=get_reserved&room_id={RoomId}";
            
            _hostedConnections.ForEach(SubscribeOnChannelOpened);
            
            RunCheckJoinLoop(cancellationToken).Forget();
        }

        private void SubscribeOnChannelOpened(IP2PConnection connection)
        {
            connection.ChanelOpened -= OnConnectionChannelOpened;
            connection.ChanelOpened += OnConnectionChannelOpened;
        }
        
        private void SubscribeOnActiveConnection(IP2PConnection connection)
        {
            UnsubscribeFromConnection(connection);
            
            connection.MessageReceived += OnConnectionChannelMessageReceived;
            connection.ChanelClosed += OnConnectionChannelClosed;
        }
        
        private void UnsubscribeFromConnection(IP2PConnection connection)
        {
            UnsubscribeFromChannelOpened(connection);
            connection.ChanelClosed -= OnConnectionChannelClosed;
            connection.MessageReceived -= OnConnectionChannelMessageReceived;
        }

        private void UnsubscribeFromChannelOpened(IP2PConnection connection)
        {
            connection.ChanelOpened -= OnConnectionChannelOpened;
        }

        private async UniTaskVoid RunCheckJoinLoop(CancellationToken cancellationToken)
        {
            var iterationsCount = 1000;
            _joiningLoopTcs = new CancellationTokenSource();
            var stopToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _joiningLoopTcs.Token);
            
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

                var getJoiningResponse = await WebRequestsSender.GetAsync<P2PGetJoiningResponseDto>(_getReservedChannelsUrl);

                if (getJoiningResponse.IsSuccess
                    && stopToken.IsCancellationRequested == false)
                {
                    if (getJoiningResponse.Result.IsNoError 
                        && getJoiningResponse.Result.Data.Length > 0)
                    {
                        foreach (var joiningDataDto in getJoiningResponse.Result.Data)
                        {
                            var connection = _hostedConnections.FirstOrDefault(c =>
                                c.ConnectionLocalDescription == joiningDataDto.channel_key
                                && c.ConnectionState == P2PConnectionState.Connecting);

                            connection?.HostComplete(joiningDataDto.join_key);
                        }
                    }
                    else if (getJoiningResponse.Result.ErrorCode == ErrorCodes.ERROR_ROOM_NOT_FOUND)
                    {
                        break;
                    }
                }
            }
        }

        private void OnConnectionChannelOpened(P2PConnection connection)
        {
            UnsubscribeFromChannelOpened(connection);

            _joiningConnections.Remove(connection);
            _hostedConnections.Remove(connection);
            _activeConnections.Add(connection);

            SubscribeOnActiveConnection(connection);

            PeerConnected?.Invoke(connection);

            if (connection.IsHost)
            {
                connection.SendMessage("Initial message");
            }
        }

        private void OnConnectionChannelMessageReceived(P2PConnection connection, string message)
        {
            MessageReceived?.Invoke(message);
            MessageReceivedFrom?.Invoke(connection, message);

            if (message == "Initial message")
            {
                connection.SendMessage("Response initial message");
            }
        }

        private void OnConnectionChannelClosed(P2PConnection connection)
        {
            _activeConnections.Remove(connection);
            DisposeConnection(connection);

            PeerDisconnected?.Invoke(connection);
        }
    }
}