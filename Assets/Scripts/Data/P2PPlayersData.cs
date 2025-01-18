using System.Collections.Generic;
using Utils.P2PLib;

namespace Data
{
    public class P2PPlayersData
    {
        public readonly Dictionary<IP2PConnection, P2PPlayerData> PlayerDataByConnection = new();
        public readonly Dictionary<IP2PConnection, int> PingByConnection = new();
        public readonly P2PPlayerData SelfData;

        public P2PPlayersData(P2PPlayerData self)
        {
            SelfData = self;
        }

        public P2PPlayerData CreatePlayerData(IP2PConnection connection)
        {
            var result = new P2PPlayerData();
            PlayerDataByConnection.Add(connection, result);

            return result;
        }
    }

    public class P2PPlayerData
    {
        public int PositionIndex;
        public CarKey CarKey;

        public P2PPlayerData()
        {
        }

        public bool IsReady { get; private set; } = false;

        public P2PPlayerData(CarKey carKey, int positionIndex = 0)
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