using System.Collections.Generic;
using System.Linq;
using Data;

namespace Model.RaceScene
{
    public class RaceResultsModel
    {
        public int PlayerSpeed { get; private set; }
        public bool IsFirst { get; private set; }
        public int QuestionsCount { get; private set; }
        public int RightAnswersCount { get; private set; }
        public int WrongAnswersCount { get; private set; }
        public int TurboBoostsCount { get; private set; }
        public float RaceTimeSec { get; private set; }
        public RaceRewardsModel RaceRewards { get; private set; }

        public void SetResults(
            CarModel playerCar,
            IEnumerable<CarModel> opponentCars,
            QuestionsModel questionsModel,
            float raceTimeSec, 
            int distanceMeters,
            ComplexityData complexityData)
        {
            PlayerSpeed = (int)playerCar.CurrentSpeedKmph;
            IsFirst = opponentCars.All(m => playerCar.PassedMeters > m.PassedMeters);
            QuestionsCount = questionsModel.QuestionsCount;
            RightAnswersCount = questionsModel.RightAnswersCountTotal;
            WrongAnswersCount = questionsModel.WrongAnswersCountTotal;
            TurboBoostsCount = questionsModel.TurboBoostsCount;
            RaceTimeSec = raceTimeSec;
            
            RaceRewards = new RaceRewardsModel(distanceMeters, this, complexityData);
        }

        public void ConsiderOpponentResult(float opponentRaceTimeSec)
        {
            if (IsFirst == false) return;

            IsFirst = opponentRaceTimeSec > RaceTimeSec;
            RaceRewards.UpdateRewards();
        }
    }
}