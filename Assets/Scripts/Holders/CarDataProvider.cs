using System;
using System.Collections.Generic;
using Data;
using Data.MonoBehaviourData;

namespace Holders
{
    public class CarDataProvider : ICarDataProvider
    {
        private readonly Dictionary<CarKey, CarData> _carDataByKey = new Dictionary<CarKey, CarData>();
        private readonly List<CarData> _carDataList = new();

        public CarDataProvider(IPrefabHolder prefabHolder)
        {
            CollectCarsData(prefabHolder);
        }

        public IReadOnlyList<CarData> CarDataList => _carDataList;

        
        public CarData GetCarData(CarKey carKey)
        {
            return _carDataByKey[carKey];
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

                var carData = new CarData(
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
        public IReadOnlyList<CarData> CarDataList { get; }
        public CarData GetCarData(CarKey carKey);
    }
}