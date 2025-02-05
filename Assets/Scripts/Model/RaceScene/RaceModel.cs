using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Model.RaceScene
{
    public class RaceModel
    {
        public event Action<bool> IsFinishingFlagChanged; 
        public event Action<bool> IsFinishedFlagChanged; 
        
        public readonly int DistanceMeters;
        
        private readonly ComplexityData _complexityData;
        
        private float _startRaceTime;

        public RaceModel(
            int raceDistanceMeters,
            CarRaceData playerCarData,
            ComplexityData complexityData,
            CarRaceData opponent1CarData,
            CarRaceData opponent2CarData = null,
            CarRaceData opponent3CarData = null)
        {
            DistanceMeters = raceDistanceMeters;
            _complexityData = complexityData;
            QuestionsModel = new QuestionsModel(complexityData);
            PlayerCar = new CarModel(playerCarData);
            RaceResultsModel = new RaceResultsModel();

            var tempOpponentsList = new List<CarModel>(Constants.MaxOpponentsCount) { new CarModel(opponent1CarData) };
            if (opponent2CarData!= null) tempOpponentsList.Add(new CarModel(opponent2CarData));
            if (opponent3CarData!= null) tempOpponentsList.Add(new CarModel(opponent3CarData));
            OpponentCarModels = tempOpponentsList.ToArray();
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
}