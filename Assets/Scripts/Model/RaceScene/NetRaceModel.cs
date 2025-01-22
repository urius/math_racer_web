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
            CarRaceModelData playerCarModelData,
            ComplexityData complexityData,
            CarRaceModelData opponent1CarModelData,
            CarRaceModelData opponent2CarModelData = null)
            : base(playerCarModelData,
            complexityData,
            opponent1CarModelData,
            opponent2CarModelData)
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