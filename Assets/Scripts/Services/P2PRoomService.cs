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
        public event Action<int, long> AccelerateReceived;
        public event Action<int, long> AccelerateTurboReceived;
        public event Action<int, long> DecelerateReceived;
        public event Action<OpponentFinishedReceivedEventPayload> OpponentFinishedReceived;
        
        private const string CommandInit = "init";
        private const string CommandInitResponse = "init_response";
        private const string CommandSetTime = "set_time";
        private const string CommandPlayerReady = "player_ready";
        private const string StartGameCommand = "start_race";
        private const string CommandAccelerate = "accelerate";
        private const string CommandAccelerateTurbo = "accelerate_turbo";
        private const string CommandDecelerate = "decelerate";
        private const string CommandFinished = "finished";
        private const string CommandPing1 = "ping1";
        private const string CommandPing2 = "ping2";
        private const string CommandPing3 = "ping3";

        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();

        private P2PRoom _p2pRoom;
        private long _hostDeltaTimeMs;
        private bool _gameIsStarted;

        private static long LocalUtcTimestampMs => (long)DateTimeHelper.GetTotalMilliseconds(DateTime.Now);
        
        public string RoomId => _p2pRoom?.RoomId.ToString() ?? "-";
        public bool HasRoom => _p2pRoom != null;
        public int ReadyPlayersCount => PlayersData?.AllPlayerDataList.Count(p => p.IsReady) ?? 0;
        public P2PPlayersData PlayersData { get; private set; }

        private PlayerModel PlayerModel => _modelsHolder.GetPlayerModel();
        private long HostUtcTimestampMs => LocalUtcTimestampMs + _hostDeltaTimeMs;
        private int PlayerNetId => PlayersData.LocalPlayerData.Id;

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
            else
            {
                RunCheckPeersLoop().Forget();
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

        public void SendToAllExcept(IP2PConnection exceptConnection, string command, string data)
        {
            for (var i = 0; i < _p2pRoom.ActiveConnections.Count; i++)
            {
                if (_p2pRoom.ActiveConnections[i] != exceptConnection)
                {
                    _p2pRoom.ActiveConnections[i].SendMessage(ToCommandFormat(command, data));
                }
            }
        }

        public void SendAccelerate()
        {
            var commandBody = new P2PCommonRaceCommandBodyDto(PlayerNetId, HostUtcTimestampMs);
            SendToAll(CommandAccelerate, commandBody.ToString());
        }

        public void SendAccelerateTurbo()
        {
            var commandBody = new P2PCommonRaceCommandBodyDto(PlayerNetId, HostUtcTimestampMs);
            SendToAll(CommandAccelerateTurbo, commandBody.ToString());
        }

        public void SendDecelerate()
        {
            var commandBody = new P2PCommonRaceCommandBodyDto(PlayerNetId, HostUtcTimestampMs);
            SendToAll(CommandDecelerate, commandBody.ToString());
        }

        public void SendFinished(int playerSpeed, float raceTimeSec, int rightAnswersCount, int wrongAnswersCount)
        {
            var commandBody = new P2PPlayerFinishedCommandBodyDto(
                PlayerNetId,
                playerSpeed,
                (int)(raceTimeSec * 1000),
                rightAnswersCount,
                wrongAnswersCount);
            SendToAll(CommandFinished, commandBody.ToString());
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
                PlayersData.CreateRemotePlayerData(connection);
                SendInit(connection);
            }
        }

        private void SendInit(IP2PConnection connection)
        {
            var remotePlayerData = PlayersData.RemotePlayerDataByConnection[connection];
            
            var commandBody = new P2PInitCommandBodyDto(
                PlayersData.LocalPlayerData.Id,
                PlayerModel.CurrentCar,
                remotePlayerData.Id,
                LocalUtcTimestampMs);
            SendCommandTo(connection, CommandInit, commandBody.ToString());

            remotePlayerData.InitCommandSendTimestamp = LocalUtcTimestampMs;
        }

        private void SendCommandTo(IP2PConnection connection, string command, string data = null)
        {
            connection.SendMessage(ToCommandFormat(command, data));
        }

        private static string ToCommandFormat(string command, string data)
        {
            return data == null ? command : $"{command}{Constants.P2PCommandSeparator}{data}";
        }

        private void SendToReadyPlayers(string message)
        {
            if (_p2pRoom == null) return;
            
            foreach (var connection in _p2pRoom.ActiveConnections)
            {
                if (PlayersData.RemotePlayerDataByConnection.TryGetValue(connection, out var playerData)
                    && playerData.IsReady)
                {
                    connection.SendMessage(message);
                }
            }
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
                case CommandPing1:
                    ProcessPing1Command(connection, data);
                    break;
                case CommandPing2:
                    ProcessPing2Command(connection, data);
                    break;
                case CommandPing3:
                    ProcessPing3Command(connection, data);
                    break;
                default:
                    if (_p2pRoom.IsHostRoom)
                    {
                        SendToAllExcept(connection, command, data);
                    }

                    HandleCommonMessage(command, data);
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
            var commandBodyDto = P2PInitResponseCommandBodyDto.Parse(body);

            var pingMs = UpdatePingForConnection(connection, commandBodyDto.HostTimeMs);
            
            var playerData = PlayersData.RemotePlayerDataByConnection[connection];
            playerData.CarKey = commandBodyDto.JoinedCarKey;
            
            var setTimeCommandBodyDto = new P2PSetTimeCommandBodyDto(LocalUtcTimestampMs, pingMs);
            SendCommandTo(connection, CommandSetTime, setTimeCommandBodyDto.ToString());
        }

        private void ProcessSetTimeCommand(IP2PConnection connection, string body)  // join side
        {
            var currentTimeMs = LocalUtcTimestampMs;
            
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

        private void ProcessPing1Command(IP2PConnection connection, string data)  // join side
        {
            var commandBody = P2PPingCommandBodyDto.Parse(data);
            var commandBoyToSend = new P2PPingCommandBodyDto(commandBody.HostTimeMs, LocalUtcTimestampMs);
            SendCommandTo(connection, CommandPing2, commandBoyToSend.ToString());
        }

        private void ProcessPing2Command(IP2PConnection connection, string data)  //host side
        {
            var commandBody = P2PPingCommandBodyDto.Parse(data);
            UpdatePingForConnection(connection, commandBody.HostTimeMs);

            var commandBoyToSend = new P2PPingCommandBodyDto(LocalUtcTimestampMs, commandBody.JoinTimeMs);
            SendCommandTo(connection, CommandPing3, commandBoyToSend.ToString());
        }

        private void ProcessPing3Command(IP2PConnection connection, string data)  // join side
        {
            var commandBody = P2PPingCommandBodyDto.Parse(data);
            UpdatePingForConnection(connection, commandBody.JoinTimeMs);
        }

        private void HandleCommonMessage(string command, string data)
        {
            switch (command)
            {
                case CommandAccelerate:
                    DispatchCommonCommand(AccelerateReceived, data);
                    break;
                case CommandAccelerateTurbo:
                    DispatchCommonCommand(AccelerateTurboReceived, data);
                    break;
                case CommandDecelerate:
                    DispatchCommonCommand(DecelerateReceived, data);
                    break;
                case CommandFinished:
                    var bodyDto = P2PPlayerFinishedCommandBodyDto.Parse(data);
                    OpponentFinishedReceived?.Invoke(
                        new OpponentFinishedReceivedEventPayload(
                            bodyDto.NetId,
                            bodyDto.Speed,
                            bodyDto.RaceTimeMs,
                            bodyDto.RightAnswersCount,
                            bodyDto.WrongAnswersCount)
                    );
                    break;
            }
        }

        private static void DispatchCommonCommand(Action<int, long> eventToDispatch, string data)
        {
            var bodyDto = P2PCommonRaceCommandBodyDto.Parse(data);
            eventToDispatch?.Invoke(bodyDto.Id, bodyDto.TimestampMs);
        }

        private static void FillPLayerData(P2PPlayerData playerData, P2PPlayerDataDto netPlayerDataDto)
        {
            playerData.CarKey = netPlayerDataDto.CarKey;
            playerData.PositionIndex = netPlayerDataDto.PositionIndex;
        }

        private int UpdatePingForConnection(IP2PConnection connection, long receivedPrevLocalTimeMs)
        {
            var travelTimeMs = LocalUtcTimestampMs - receivedPrevLocalTimeMs;
            var pingMs = (int)Math.Ceiling(travelTimeMs * 0.5f);
            PlayersData.PingByConnection[connection] = new P2PPingData(pingMs, LocalUtcTimestampMs);

            Debug.Log($"{connection.ChannelLabel} Ping: {pingMs} ms");

            return pingMs;
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

        private async UniTaskVoid RunCheckPeersLoop()
        {
            const int millisecondsInSecond = 1000;
            const int retryInitCommandTimeMs = 5 * millisecondsInSecond;
            const int noRespondTimeForDisconnectMs = 15 * millisecondsInSecond;
            
            while (_p2pRoom != null && PlayersData != null)
            {
                await UniTask.Delay(millisecondsInSecond);

                if (_p2pRoom == null || PlayersData == null) break;
                
                foreach (var connection in _p2pRoom.ActiveConnections)
                {
                    if (PlayersData.RemotePlayerDataByConnection.TryGetValue(connection, out var playerData))
                    {
                        var localTimeMs = LocalUtcTimestampMs;
                        if (playerData.IsReady)
                        {
                            if (PlayersData.PingByConnection.TryGetValue(connection, out var pingData))
                            {
                                if (pingData.MeasureLocalTimeMs <= localTimeMs - millisecondsInSecond)
                                {
                                    if (pingData.MeasureLocalTimeMs <= localTimeMs - noRespondTimeForDisconnectMs)
                                    {
                                        connection.Close();
                                    }
                                    else
                                    {
                                        SendCommandTo(connection, CommandPing1,
                                            new P2PPingCommandBodyDto(localTimeMs).ToString());
                                    }
                                }
                            }
                        }
                        else if (playerData.InitCommandSendTimestamp <= localTimeMs - retryInitCommandTimeMs)
                        {
                            SendInit(connection);
                        }
                    }
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
        public event Action<int, long> AccelerateReceived;
        public event Action<int, long> AccelerateTurboReceived;
        public event Action<int, long> DecelerateReceived;
        public event Action<OpponentFinishedReceivedEventPayload> OpponentFinishedReceived;
        
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
        public void SendAccelerate();
        public void SendAccelerateTurbo();
        public void SendDecelerate();
        public void SendFinished(int playerSpeed, float raceTimeSec, int rightAnswersCount, int wrongAnswersCount);
    }

    public struct OpponentFinishedReceivedEventPayload
    {
        public readonly int NetId;
        public readonly int Speed;
        public readonly int RaceTimeMs;
        public readonly int RightAnswersCount;
        public readonly int WrongAnswersCount;

        public OpponentFinishedReceivedEventPayload(int netId, int speed, int raceTimeMs, int rightAnswersCount, int wrongAnswersCount)
        {
            NetId = netId;
            Speed = speed;
            RaceTimeMs = raceTimeMs;
            RightAnswersCount = rightAnswersCount;
            WrongAnswersCount = wrongAnswersCount;
        }
    }
}