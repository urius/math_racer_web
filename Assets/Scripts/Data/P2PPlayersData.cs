using System.Collections.Generic;
using Utils.P2PLib;

namespace Data
{
    public class P2PPlayersData
    {
        public readonly List<P2PPlayerData> AllPlayerDataList = new();
        public readonly Dictionary<IP2PConnection, P2PPlayerData> RemotePlayerDataByConnection = new();
        public readonly Dictionary<IP2PConnection, int> PingByConnection = new();
        
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

        public void CreateLocalPlayerData(CarKey carKey, int id = -1)
        {
            LocalPlayerData = CreatePlayerData(id);
            LocalPlayerData.CarKey = carKey;
        }

        private static int GetNextId()
        {
            return ++_lastCumulativeId;
        }
    }

    public class P2PPlayerData
    {
        public readonly int Id;
        
        public int PositionIndex;
        public CarKey CarKey;

        public P2PPlayerData(int id)
        {
            Id = id;
        }

        public bool IsReady { get; private set; } = false;

        public P2PPlayerData(int id, CarKey carKey, int positionIndex)
            : this(id)
        {
            CarKey = carKey;
            PositionIndex = positionIndex;
        }

        public void SetReady()
        {
            IsReady = true;
        }
    }
}