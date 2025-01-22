using System;
using Data;
using UnityEngine;

namespace Model.RaceScene
{
    public class RaceModel
    {
        public event Action<bool> IsFinishingFlagChanged; 
        public event Action<bool> IsFinishedFlagChanged; 
        
        public readonly int DistanceMeters = 500;
        
        private readonly ComplexityData _complexityData;
        
        private float _startRaceTime;

        public RaceModel(CarRaceModelData playerCarModelData, ComplexityData complexityData, CarRaceModelData opponent1CarModelData, CarRaceModelData opponent2CarModelData = null)
        {
            _complexityData = complexityData;
            QuestionsModel = new QuestionsModel(complexityData);
            PlayerCar = new CarModel(playerCarModelData);
            OpponentCarModels = opponent2CarModelData != null
                ? new[] { new CarModel(opponent1CarModelData), new CarModel(opponent2CarModelData) }
                : new[] { new CarModel(opponent1CarModelData) };
            RaceResultsModel = new RaceResultsModel();
        }

        public virtual bool IsSinglePlayerRace => true;
        public QuestionsModel QuestionsModel { get; }
        public CarModel PlayerCar { get; }
        public CarModel[] OpponentCarModels { get; }
        public RaceResultsModel RaceResultsModel { get; }
        public RaceRewardsModel RaceRewards => RaceResultsModel.RaceRewards;
        public bool IsFinishing { get; private set; }
        public bool IsFinished { get; private set; }
        public float PlayerCarDistanceToFinish => DistanceMeters - PlayerCar.PassedMeters;

        public void StartRace()
        {
            _startRaceTime = Time.realtimeSinceStartup;
        }

        public void Update(float deltaTime)
        {
            QuestionsModel.Update(deltaTime);
            PlayerCar.Update(deltaTime);
            foreach (var opponentCarModel in OpponentCarModels)
            {
                UpdateOpponentCar(opponentCarModel, deltaTime);
            }

            if (IsFinishing == false && PlayerCarDistanceToFinish < 5 * PlayerCar.CurrentSpeedKmph * 0.01f)
            {
                IsFinishing = true;
                IsFinishingFlagChanged?.Invoke(IsFinishing);
            }

            if (IsFinished == false && PlayerCarDistanceToFinish <= -1f)
            {
                var raceTimeSec = Time.realtimeSinceStartup - _startRaceTime;
                IsFinished = true;
                RaceResultsModel.SetResults(PlayerCar, OpponentCarModels, QuestionsModel, raceTimeSec, DistanceMeters, _complexityData);
                IsFinishedFlagChanged?.Invoke(IsFinished);
            }
        }

        private void UpdateOpponentCar(CarModel carModel, float deltaTime)
        {
            carModel.Update(deltaTime);
            carModel.SetDistanceToPlayerCar(PlayerCar.PassedMeters - carModel.PassedMeters);
        }
    }

    public class CarRaceModelData
    {
        public readonly int Id;
        public readonly CarKey CarKey;
        public readonly int CarPositionIndex;

        public CarRaceModelData(CarKey carKey, int carPositionIndex, int id)
        {
            CarKey = carKey;
            CarPositionIndex = carPositionIndex;
            Id = id;
        }
    }
}