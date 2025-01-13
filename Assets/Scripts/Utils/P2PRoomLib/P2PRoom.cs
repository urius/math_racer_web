using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Utils.P2PLib;
using Utils.P2PRoomLib.Data;
using Utils.WebRequestSender;

namespace Utils.P2PRoomLib
{
    public class P2PRoom
    {
        private readonly string _serviceUrl;
        private readonly List<IP2PHostSideConnection> _hostedConnections = new();
        private readonly List<IP2PConnection> _activeConnections = new();

        private string _getJoiningUrl;
        private bool _cancelJoiningLoopFlag = false;

        public P2PRoom(string serviceUrl)
        {
            _serviceUrl = serviceUrl;
        }

        public int RoomId { get; private set; }
        public bool IsRoomCreated => RoomId > 0;
        private string CreateRoomUrl => _serviceUrl + "?command=create_room";
        private string ReserveFreeChannelUrl => _serviceUrl + $"?command=reserve_free_channel&room_id={RoomId}";
        private string ConnectToRoomUrl => _serviceUrl + $"?command=connect_to_room&room_id={RoomId}";

        public async UniTask<bool> Create(int maxConnectionsCount = 1)
        {
            var initConnectionTasks = new UniTask<IP2PHostSideConnection>[maxConnectionsCount];
            for (var i = 0; i < initConnectionTasks.Length; i++)
            {
                initConnectionTasks[i] = P2PConnection.HostInit();
            }

            var connections = await UniTask.WhenAll(initConnectionTasks);
            
            _hostedConnections.AddRange(connections);

            var descriptionsStr = string.Join(",", _hostedConnections.Select(c => c.ConnectionLocalDescription));
            
            var createRoomResult = await WebRequestsSender.PostAsync<P2PCreateRoomResponseDto>(CreateRoomUrl,
                new Dictionary<string, string>
                {
                    { "channel_keys", descriptionsStr }
                });

            if (createRoomResult.IsSuccess)
            {
                HandleRoomCreated(createRoomResult.Result);
            }

            return IsRoomCreated;
        }

        public async UniTask<bool> Join(int roomId)
        {
            RoomId = roomId;

            var reserveFreeChannelResult = await WebRequestsSender.GetAsync<P2PReserveFreeChannelResponseDto>(ReserveFreeChannelUrl);
            
            if (reserveFreeChannelResult.IsSuccess)
            {
                var channelData = reserveFreeChannelResult.Result.data;

                var joinConnection = await P2PConnection.Join(channelData.channel_key);
                SubscribeOnChannelOpened(joinConnection);

                var connectResult = await WebRequestsSender
                    .GetAsync<P2PConnectToRoomResponseDto>(
                        ConnectToRoomUrl +
                        $"&channel_id={channelData.id}&join_key={joinConnection.ConnectionLocalDescription}");

                if (connectResult.IsSuccess)
                {
                    await joinConnection.ConnectionEstablishedTask;
                }

                return connectResult.IsSuccess;
            }

            return false;
        }

        public void CancelJoiningLoop()
        {
            _cancelJoiningLoopFlag = true;
        }

        private void HandleRoomCreated(P2PCreateRoomResponseDto createRoomResponse)
        {
            RoomId = createRoomResponse.data.room_id;
            _getJoiningUrl = _serviceUrl + $"?command=get_reserved&room_id={RoomId}";

            foreach (var hostSideConnection in _hostedConnections)
            {
                SubscribeOnChannelOpened(hostSideConnection);
            }
            
            StartCheckJoiningLoop();
        }

        private void SubscribeOnChannelOpened(IP2PConnection connection)
        {
            connection.ChanelOpened -= OnConnectionChannelOpened;
            connection.ChanelOpened += OnConnectionChannelOpened;
        }

        private void OnConnectionChannelOpened(P2PConnection connection)
        {
            connection.ChanelOpened -= OnConnectionChannelOpened;
            
            _hostedConnections.Remove(connection);
            _activeConnections.Add(connection);
        }

        private void StartCheckJoiningLoop()
        {
            CheckJoiningLoop().Forget();
        }

        private async UniTaskVoid CheckJoiningLoop()
        {
            _cancelJoiningLoopFlag = false;
            var iterationsCount = 1000;
            while (iterationsCount > 0 && _cancelJoiningLoopFlag == false)
            {
                iterationsCount--;

                await UniTask.Delay(2000);

                if (_cancelJoiningLoopFlag
                    || _hostedConnections.All(c => c.ConnectionState == P2PConnectionState.Established))
                {
                    break;
                }

                var getJoiningResult = await WebRequestsSender.GetAsync<P2PGetJoiningResponseDto>(_getJoiningUrl);

                if (getJoiningResult.IsSuccess)
                {
                    if (getJoiningResult.Result.data.Length > 0)
                    {
                        foreach (var joiningDataDto in getJoiningResult.Result.data)
                        {
                            var connection = _hostedConnections.FirstOrDefault(c =>
                                c.ConnectionLocalDescription == joiningDataDto.channel_key
                                && c.ConnectionState == P2PConnectionState.Connecting);

                            if (connection != null)
                            {
                                connection.HostComplete(joiningDataDto.join_key);
                            }
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}