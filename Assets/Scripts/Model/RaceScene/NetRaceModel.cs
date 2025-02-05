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
            int raceDistanceMeters,
            CarRaceData playerCarData,
            ComplexityData complexityData,
            CarRaceData opponent1CarData,
            CarRaceData opponent2CarData = null,
            CarRaceData opponent3CarData = null)
            : base(raceDistanceMeters, 
            playerCarData,
            complexityData,
            opponent1CarData,
            opponent2CarData,
            opponent3CarData)
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