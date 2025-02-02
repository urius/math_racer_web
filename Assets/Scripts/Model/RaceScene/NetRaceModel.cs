using System;
using System.Collections.Generic;
using Data;

namespace Model.RaceScene
{
    public class NetRaceModel : RaceModel
    {
        public event Action<int> NetOpponentResultAdded;
        
        private readonly Dictionary<int, NetOpponentRaceResult> _netOpponentRaceResultById = new();
        
        public NetRaceModel(
            CarRaceData playerCarData,
            ComplexityData complexityData,
            CarRaceData opponent1CarData,
            CarRaceData opponent2CarData = null)
            : base(playerCarData,
            complexityData,
            opponent1CarData,
            opponent2CarData)
        {
        }

        public override bool IsSinglePlayerRace => false;
        public IReadOnlyDictionary<int, NetOpponentRaceResult> NetOpponentRaceResults => _netOpponentRaceResultById;

        public void SetOpponentResult(int id, NetOpponentRaceResult result)
        {
            _netOpponentRaceResultById[id] = result;

            RaceResultsModel.ConsiderOpponentResult(result.RaceTimeMs);
            
            NetOpponentResultAdded?.Invoke(id);
        }
    }
}