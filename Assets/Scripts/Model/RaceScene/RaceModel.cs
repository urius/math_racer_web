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

        public RaceModel(CarKey playerCarKey, CarKey botCarKey, ComplexityData complexityData)
        {
            _complexityData = complexityData;
            QuestionsModel = new QuestionsModel(complexityData);
            PlayerCar = new CarModel(playerCarKey);
            BotCar = new CarModel(botCarKey);
            RaceResultsModel = new RaceResultsModel();
        }

        public QuestionsModel QuestionsModel { get; }
        public CarModel PlayerCar { get; }
        public CarModel BotCar { get; }
        public RaceResultsModel RaceResultsModel { get; }
        public RaceRewardsModel RaceRewards { get; private set; }
        public bool IsFinishing { get; private set; }
        public bool IsFinished { get; private set; }
        public float PlayerCarDistanceToFinish => DistanceMeters - PlayerCar.PassedMeters;

        public void Update(float deltaTime)
        {
            QuestionsModel.Update(deltaTime);
            PlayerCar.Update(deltaTime);
            BotCar.Update(deltaTime);

            if (IsFinishing == false && PlayerCarDistanceToFinish < 5 * PlayerCar.CurrentSpeedKmph * 0.01f)
            {
                IsFinishing = true;
                IsFinishingFlagChanged?.Invoke(IsFinishing);
            }

            if (IsFinished == false && PlayerCarDistanceToFinish <= -1f)
            {
                IsFinished = true;
                RaceResultsModel.SetResults(PlayerCar, BotCar, QuestionsModel);
                RaceRewards = new RaceRewardsModel(DistanceMeters, RaceResultsModel, _complexityData);
                IsFinishedFlagChanged?.Invoke(IsFinished);
            }
        }
    }
}