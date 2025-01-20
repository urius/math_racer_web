using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data;
using Data.Dto.P2P;
using Helpers;
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
        public event Action StartGameReceived;
        
        public const string CommandAccelerated = "accelerated";
        public const string CommandAcceleratedTurbo = "accelerated_turbo";
        public const string CommandDecelerated = "decelerated";
        
        private const string CommandInit = "init";
        private const string CommandInitResponse = "init_response";
        private const string CommandSetTime = "set_time";
        private const string CommandPlayerReady = "player_ready";
        private const string StartGameCommand = "start_race";

        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();

        private P2PRoom _p2pRoom;
        private long _hostDeltaTimeMs;
        private bool _gameIsStarted;

        private static long UtcTimestampMs => (long)DateTimeHelper.GetTotalMilliseconds(DateTime.Now);
        
        public string RoomId => _p2pRoom?.RoomId.ToString() ?? "-";
        public bool HasRoom => _p2pRoom != null;
        public int ReadyPlayersCount => PlayersData?.AllPlayerDataList.Count(p => p.IsReady) ?? 0;
        public P2PPlayersData PlayersData { get; private set; }

        private PlayerModel PlayerModel => _modelsHolder.GetPlayerModel();
        private long HostUtcTimestampMs => UtcTimestampMs + _hostDeltaTimeMs;

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
            PlayersData = new P2PPlayersData();
            PlayersData.CreateLocalPlayerData(PlayerModel.CurrentCar);
            PlayersData.LocalPlayerData.PositionIndex = 0;

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
            PlayersData = new P2PPlayersData();
            
            var joinResult = await _p2pRoom.Join(roomId);
            
            return joinResult;
        }

        public void DestroyCurrentRoom()
        {
            if (_p2pRoom != null)
            {
                UnsubscribeFromRoom(_p2pRoom);
                _p2pRoom.Dispose();
                _p2pRoom = null;
                PlayersData = null;
            }
        }

        public void SendStartToReadyPlayers()
        {
            _gameIsStarted = true;
            var readyPlayerConnections = PlayersData.RemotePlayerDataByConnection
                .Where(pair => pair.Value.IsReady)
                .Select(pair => pair.Key)
                .ToArray();
            var readyPlayerDataList = PlayersData.AllPlayerDataList
                .Where(d => d.IsReady)
                .ToArray();
            
            SetupCarPositionIndexes(readyPlayerDataList);

            var commandBodyDto = new P2PStartCommandBodyDto(readyPlayerDataList.Select(P2PPlayerDataDto.FromP2PPlayerData).ToArray());
            
            foreach (var connection in readyPlayerConnections)
            {
                connection.SendMessage(ToCommandFormat(StartGameCommand, commandBodyDto.ToString()));
            }
        }

        public void SendToAll(string command, string data)
        {
            _p2pRoom.SendToAll(ToCommandFormat(command, data));
        }

        private P2PRoom CreateRoomInstance()
        {
            DestroyCurrentRoom();

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
                var remotePlayerData = PlayersData.CreateRemotePlayerData(connection);
                
                var commandBody = new P2PInitCommandBodyDto(
                    PlayersData.LocalPlayerData.Id,
                    PlayerModel.CurrentCar,
                    remotePlayerData.Id,
                    UtcTimestampMs);
                SendCommandTo(connection, CommandInit, commandBody.ToString());
            }
        }

        private void SendCommandTo(IP2PConnection connection, string command, string data = null)
        {
            connection.SendMessage(ToCommandFormat(command, data));
        }

        private static string ToCommandFormat(string command, string data)
        {
            return data == null ? command : $"{command}{Constants.P2PCommandSeparator}{data}";
        }

        private void OnMessageReceivedFrom(IP2PConnection connection, string message)
        {
            var splitted = message.Split(Constants.P2PCommandSeparator);
            var command = splitted[0];
            var data = splitted[1];

            switch (command)
            {
                case CommandInit:
                    ProcessInitCommand(connection, data);
                    break;
                case CommandInitResponse:
                    ProcessInitResponseCommand(connection, data);
                    break;
                case CommandSetTime:
                    ProcessSetTimeCommand(connection, data);
                    break;
                case CommandPlayerReady:
                    ProcessPlayerReadyCommand(connection);
                    break;
                case StartGameCommand:
                    ProcessStartGameCommand(connection, data);
                    break;
                default:
                    if (_p2pRoom.IsHostRoom)
                    {
                        SendToAll(command, data);
                    }
                    MessageReceived?.Invoke(connection, message);
                    break;
            }
        }

        private void ProcessInitCommand(IP2PConnection connection, string body)  // join side
        {
            var commandBodyDto = P2PInitCommandBodyDto.Parse(body);
            
            PlayersData.CreateLocalPlayerData(PlayerModel.CurrentCar, commandBodyDto.JoinId);
            
            var hostPlayerData = PlayersData.CreateRemotePlayerData(connection, commandBodyDto.HostId);
            hostPlayerData.CarKey = commandBodyDto.HostCarKey;

            var initResponseCommandBodyDto = new P2PInitResponseCommandBodyDto(PlayersData.LocalPlayerData.CarKey, commandBodyDto.HostTimeMs);
            SendCommandTo(connection, CommandInitResponse, initResponseCommandBodyDto.ToString());
        }

        private void ProcessInitResponseCommand(IP2PConnection connection, string body)  //host side
        {
            var currentTimeMs = UtcTimestampMs;
            var commandBodyDto = P2PInitResponseCommandBodyDto.Parse(body);

            var travelTimeMs = currentTimeMs - commandBodyDto.HostTimeMs;
            var pingMs = (int)Math.Ceiling(travelTimeMs * 0.5f);
            PlayersData.PingByConnection[connection] = pingMs;
            
            var playerData = PlayersData.RemotePlayerDataByConnection[connection];
            playerData.CarKey = commandBodyDto.JoinedCarKey;

            Debug.Log($"{connection.ChannelLabel} Ping: {pingMs} ms");
            
            var setTimeCommandBodyDto = new P2PSetTimeCommandBodyDto(UtcTimestampMs, pingMs);
            SendCommandTo(connection, CommandSetTime, setTimeCommandBodyDto.ToString());
        }

        private void ProcessSetTimeCommand(IP2PConnection connection, string body)  // join side
        {
            var currentTimeMs = UtcTimestampMs;
            
            var commandBodyDto = P2PSetTimeCommandBodyDto.Parse(body);
            _hostDeltaTimeMs = commandBodyDto.EstimatedJoinSideTimeMs - currentTimeMs;

            PlayersData.LocalPlayerData.SetReady();
            
            SendCommandTo(connection, CommandPlayerReady, string.Empty);
        }

        private void ProcessPlayerReadyCommand(IP2PConnection connection)  // host side
        {
            PlayersData.LocalPlayerData.SetReady();
            PlayersData.RemotePlayerDataByConnection[connection].SetReady();
            ConnectedPlayerReady?.Invoke(connection);
        }

        private void ProcessStartGameCommand(IP2PConnection _, string data) // join side
        {
            var commandBodyDto = P2PStartCommandBodyDto.Parse(data);

            foreach (var netPlayerDataDto in commandBodyDto.PlayerDataDtoList)
            {
                var playerData = PlayersData.AllPlayerDataList
                                     .FirstOrDefault(d => d.Id == netPlayerDataDto.Id)
                                 ?? PlayersData.CreatePlayerData(netPlayerDataDto.Id);
                
                FillPLayerData(playerData, netPlayerDataDto);
            }
            
            StartGameReceived?.Invoke();
        }

        private static void FillPLayerData(P2PPlayerData playerData, P2PPlayerDataDto netPlayerDataDto)
        {
            playerData.CarKey = netPlayerDataDto.CarKey;
            playerData.PositionIndex = netPlayerDataDto.PositionIndex;
        }

        private void OnPeerDisconnected(IP2PConnection connection)
        {
            PlayerDisconnected?.Invoke(connection);
        }

        private void SetupCarPositionIndexes(IEnumerable<P2PPlayerData> readyPlayerDataList) // host side
        {
            var positionIndex = 1;
            foreach (var playerData in readyPlayerDataList)
            {
                if (playerData != PlayersData.LocalPlayerData)
                {
                    playerData.PositionIndex = positionIndex;
                    positionIndex++;
                }
                else
                {
                    playerData.PositionIndex = 0; //host player car always first
                }
            }
        }
    }

    public interface IP2PRoomService
    {
        public event Action<IP2PConnection> NewPlayerConnected;
        public event Action<IP2PConnection, string> MessageReceived;
        public event Action<IP2PConnection> PlayerDisconnected;
        public event Action<IP2PConnection> ConnectedPlayerReady;
        public event Action StartGameReceived;
        
        public string RoomId { get; }
        public bool HasRoom { get; }
        public int ReadyPlayersCount { get; }
        public bool IsJoinAllowed { get; set; }
        public P2PPlayersData PlayersData { get; }

        public UniTask<bool> HostNewRoom();
        public void DestroyCurrentRoom();
        UniTask<bool> JoinRoom(int parse);
        public void SendStartToReadyPlayers();
        public void SendToAll(string command, string data = null);
    }
}