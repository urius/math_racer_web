using System;
using Data;

namespace Model.RaceScene
{
    public class RaceModel
    {
        public event Action<bool> IsFinishingFlagChanged; 
        public event Action<bool> IsFinishedFlagChanged; 
        
        private readonly ComplexityData _complexityData;
        
        public readonly int DistanceMeters = 500;

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

        public QuestionsModel QuestionsModel { get; }
        public CarModel PlayerCar { get; }
        public CarModel[] OpponentCarModels { get; }
        public RaceResultsModel RaceResultsModel { get; }
        public RaceRewardsModel RaceRewards { get; private set; }
        public bool IsFinishing { get; private set; }
        public bool IsFinished { get; private set; }
        public float PlayerCarDistanceToFinish => DistanceMeters - PlayerCar.PassedMeters;

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
                IsFinished = true;
                RaceResultsModel.SetResults(PlayerCar, OpponentCarModels, QuestionsModel);
                RaceRewards = new RaceRewardsModel(DistanceMeters, RaceResultsModel, _complexityData);
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
        public readonly CarKey CarKey;
        public readonly int CarPositionIndex;

        public CarRaceModelData(CarKey carKey, int carPositionIndex)
        {
            CarKey = carKey;
            CarPositionIndex = carPositionIndex;
        }
    }
}