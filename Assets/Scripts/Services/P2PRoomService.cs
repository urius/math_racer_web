using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using Data.Dto.P2P;
using Infra.Instance;
using Model;
using Providers;
using UnityEngine;
using Utils.P2PLib;
using Utils.P2PRoomLib;

namespace Services
{
    public class P2PRoomService : IP2PRoomService
    {
        public event Action<IP2PConnection> NewPlayerConnected;
        public event Action<IP2PConnection, string> MessageReceived;
        public event Action<IP2PConnection> PlayerDisconnected;
        public event Action<IP2PConnection> ConnectedPlayerReady;

        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        
        private const string CommandInit = "init";
        private const string CommandInitResponse = "init_response";
        private const string CommandSetTime = "set_time";
        private const string CommandPlayerReady = "player_ready";
        private const string StartRaceCommand = "start_race";
        
        private P2PRoom _p2pRoom;
        private long _hostDeltaTimeMs;

        public string RoomId => _p2pRoom?.RoomId.ToString() ?? "-";
        public bool HasRoom => _p2pRoom != null;
        public IReadOnlyList<IP2PConnection> ActiveConnections => _p2pRoom.ActiveConnections;
        public P2PPlayersData PlayersData { get; private set; }

        private PlayerModel PlayerModel => _modelsHolder.GetPlayerModel();
        private long MillisecondsSinceStartup => (long)(Time.realtimeSinceStartupAsDouble * 1000);
        private long HostMillisecondsSinceStartup => MillisecondsSinceStartup + _hostDeltaTimeMs;

        public bool IsJoinAllowed
        {
            get => _p2pRoom?.IsJoinAllowed ?? false;
            set
            {
                if (_p2pRoom != null)
                {
                    _p2pRoom.IsJoinAllowed = value;
                }
            }
        }

        public async UniTask<bool> HostNewRoom()
        {
            _p2pRoom = CreateRoomInstance();

            var createRoomResult = await _p2pRoom.Create(2);

            if (createRoomResult == false)
            {
                DestroyCurrentRoom();
            }

            return createRoomResult;
        }

        public async UniTask<bool> JoinRoom(int roomId)
        {
            _p2pRoom = CreateRoomInstance();

            var joinResult = await _p2pRoom.Join(roomId);
            
            return joinResult;
        }

        public void SendStartRace()
        {
            _p2pRoom.SendToAll(StartRaceCommand);
        }

        public void DestroyCurrentRoom()
        {
            if (_p2pRoom != null)
            {
                UnsubscribeFromRoom(_p2pRoom);
                _p2pRoom.Dispose();
                _p2pRoom = null;
            }
        }

        private P2PRoom CreateRoomInstance()
        {
            DestroyCurrentRoom();

            var selfPlayerData = new P2PPlayerData(PlayerModel.CurrentCar);
            PlayersData = new P2PPlayersData(selfPlayerData);
            var p2pRoom = new P2PRoom(Urls.P2PRoomsServiceUrl);
            SubscribeOnRoom(p2pRoom);

            return p2pRoom;
        }

        private void SubscribeOnRoom(P2PRoom room)
        {
            room.PeerConnected += OnPeerConnected;
            room.MessageReceivedFrom += OnMessageReceivedFrom;
            room.PeerDisconnected += OnPeerDisconnected;
        }

        private void UnsubscribeFromRoom(P2PRoom room)
        {
            room.PeerConnected -= OnPeerConnected;
            room.MessageReceivedFrom -= OnMessageReceivedFrom;
            room.PeerDisconnected -= OnPeerDisconnected;
        }

        private void OnPeerConnected(IP2PConnection connection)
        {
            NewPlayerConnected?.Invoke(connection);
            
            if (_p2pRoom.IsHostRoom)
            {
                var remotePlayerData = PlayersData.CreatePlayerData(connection);
                remotePlayerData.PositionIndex = PlayersData.PlayerDataByConnection.Count;
                var commandBody = new P2PInitCommandBodyDto(
                    PlayerModel.CurrentCar, remotePlayerData.PositionIndex, MillisecondsSinceStartup);
                
                SendCommandTo(connection, CommandInit, commandBody.ToString());
            }
        }

        private void SendCommandTo(IP2PConnection connection, string command, string data)
        {
            connection.SendMessage($"{command}{Constants.P2PCommandSeparator}{data}");
        }

        private void OnMessageReceivedFrom(IP2PConnection connection, string message)
        {
            var splitted = message.Split(Constants.P2PCommandSeparator);
            var command = splitted[0];
            var body = splitted[1];

            switch (command)
            {
                case CommandInit:
                    ProcessInitCommand(connection, body);
                    break;
                case CommandInitResponse:
                    ProcessInitResponseCommand(connection, body);
                    break;
                case CommandSetTime:
                    ProcessSetTimeCommand(connection, body);
                    break;
                case CommandPlayerReady: // host side
                    ConnectedPlayerReady?.Invoke(connection);
                    break;
                default:
                    MessageReceived?.Invoke(connection, message);
                    break;
            }
        }

        // join side
        private void ProcessInitCommand(IP2PConnection connection, string body)
        {
            var commandBodyDto = P2PInitCommandBodyDto.Parse(body);

            PlayersData.SelfData.PositionIndex = commandBodyDto.PositionIndex;
            var hostPlayerData = PlayersData.CreatePlayerData(connection);
            hostPlayerData.CarKey = commandBodyDto.HostCarKey;

            var initResponseCommandBodyDto = new P2PInitResponseCommandBodyDto(PlayerModel.CurrentCar, commandBodyDto.HostTimeMs);
            SendCommandTo(connection, CommandInitResponse, initResponseCommandBodyDto.ToString());
        }
        
        //host side
        private void ProcessInitResponseCommand(IP2PConnection connection, string body)
        {
            var currentTimeMs = MillisecondsSinceStartup;
            var commandBodyDto = P2PInitResponseCommandBodyDto.Parse(body);

            var travelTimeMs = currentTimeMs - commandBodyDto.HostTimeMs;
            var pingMs = (int)Math.Ceiling(travelTimeMs * 0.5f);
            PlayersData.PingByConnection[connection] = pingMs;
            
            var playerData = PlayersData.PlayerDataByConnection[connection];
            playerData.CarKey = commandBodyDto.JoinedCarKey;

            Debug.Log($"{connection.ChannelLabel} Ping: {pingMs} ms");
            
            var setTimeCommandBodyDto = new P2PSetTimeCommandBodyDto(MillisecondsSinceStartup, pingMs);
            SendCommandTo(connection, CommandSetTime, setTimeCommandBodyDto.ToString());
        }

        // join side
        private void ProcessSetTimeCommand(IP2PConnection connection, string body)
        {
            var currentTimeMs = MillisecondsSinceStartup;
            
            var commandBodyDto = P2PSetTimeCommandBodyDto.Parse(body);
            _hostDeltaTimeMs = commandBodyDto.EstimatedJoinSideTimeMs - currentTimeMs;
            
            SendCommandTo(connection, CommandPlayerReady, string.Empty);
        }

        private void OnPeerDisconnected(IP2PConnection connection)
        {
            PlayerDisconnected?.Invoke(connection);
        }
    }

    public interface IP2PRoomService
    {
        public event Action<IP2PConnection> NewPlayerConnected;
        public event Action<IP2PConnection, string> MessageReceived;
        public event Action<IP2PConnection> PlayerDisconnected;
        public event Action<IP2PConnection> ConnectedPlayerReady;
        
        public string RoomId { get; }
        public bool HasRoom { get; }
        public IReadOnlyList<IP2PConnection> ActiveConnections { get; }
        public bool IsJoinAllowed { get; set; }

        public UniTask<bool> HostNewRoom();
        public void DestroyCurrentRoom();
        UniTask<bool> JoinRoom(int parse);
        public void SendStartRace();
    }
}