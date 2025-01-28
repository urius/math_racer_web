using System.Collections.Generic;
using Utils.P2PLib;

namespace Data
{
    public class P2PPlayersData
    {
        public readonly List<P2PPlayerData> AllPlayerDataList = new();
        public readonly Dictionary<IP2PConnection, P2PPlayerData> RemotePlayerDataByConnection = new();
        public readonly Dictionary<IP2PConnection, P2PPingData> PingByConnection = new();
        
        private static int _lastCumulativeId = 0;
        
        public P2PPlayerData LocalPlayerData { get; private set; }

        public P2PPlayerData CreatePlayerData(int id)
        {
            var result = new P2PPlayerData(id < 0 ? GetNextId() : id);
            AllPlayerDataList.Add(result);

            return result;
        }

        public P2PPlayerData CreateRemotePlayerData(IP2PConnection connection, int id = -1)
        {
            var result = CreatePlayerData(id);
            RemotePlayerDataByConnection.Add(connection, result);

            return result;
        }

        public void CreateLocalPlayerData(string playerName, CarKey carKey,  int id = -1)
        {
            LocalPlayerData = CreatePlayerData(id);
            LocalPlayerData.CarKey = carKey;
            LocalPlayerData.PlayerName = playerName;
        }

        private static int GetNextId()
        {
            return ++_lastCumulativeId;
        }
    }

    public class P2PPlayerData
    {
        public readonly int Id;

        public string PlayerName;
        public int PositionIndex;
        public CarKey CarKey;
        public long InitCommandSendTimestamp;

        public P2PPlayerData(int id)
        {
            Id = id;
        }

        public bool IsReady { get; private set; } = false;

        public void SetReady()
        {
            IsReady = true;
        }
    }

    public struct P2PPingData
    {
        public readonly int PingMs;
        public readonly long MeasureLocalTimeMs;

        public P2PPingData(int pingMs, long measureLocalTimeMs)
        {
            PingMs = pingMs;
            MeasureLocalTimeMs = measureLocalTimeMs;
        }
    }
}