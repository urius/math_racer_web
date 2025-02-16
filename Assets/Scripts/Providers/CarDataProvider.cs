using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.MonoBehaviourData;

namespace Providers
{
    public class CarDataProvider : ICarDataProvider
    {
        private readonly Dictionary<CarKey, CarSettings> _carDataByKey = new();
        private readonly List<CarSettings> _carDataList = new();

        public CarDataProvider(IPrefabHolder prefabHolder)
        {
            CollectCarsData(prefabHolder);
        }

        public IReadOnlyList<CarSettings> CarDataList => _carDataList;
        
        public CarSettings GetCarData(CarKey carKey)
        {
            return _carDataByKey[carKey];
        }

        public IReadOnlyList<CarSettings> GetUnlockedCarsByLevel(int targetLevel)
        {
            return CarDataList
                .Where(c => c.UnlockLevel <= targetLevel)
                .OrderBy(c => c.UnlockLevel)
                .ToArray();
        }

        private void CollectCarsData(IPrefabHolder prefabHolder)
        {
            var value = Enum.GetValues(typeof(CarKey));
            
            _carDataList.Capacity = value.Length;
            
            foreach (int i in value)
            {
                if (i > 0)
                {
                    CollectCarData((CarKey)i, prefabHolder);
                }
            }
        }

        private void CollectCarData(CarKey carKey, IPrefabHolder prefabHolder)
        {
            if (_carDataByKey.ContainsKey(carKey) == false)
            {
                var carPrefabKey = (PrefabKey)((int)carKey + Constants.PrefabCarsOffset);
                var prefab = prefabHolder.GetPrefabByKey(carPrefabKey);
                var mbData = prefab.GetComponent<CarMonoBehaviourData>();

                var carData = new CarSettings(
                    carKey,
                    mbData.IconSprite,
                    mbData.Price,
                    mbData.UnlockLevel,
                    mbData.Acceleration,
                    mbData.MaxSpeed);

                _carDataByKey[carKey] = carData;
                _carDataList.Add(carData);
            }
        }
    }

    public interface ICarDataProvider
    {
        public IReadOnlyList<CarSettings> CarDataList { get; }
        public CarSettings GetCarData(CarKey carKey);
        public IReadOnlyList<CarSettings> GetUnlockedCarsByLevel(int level);
    }
}