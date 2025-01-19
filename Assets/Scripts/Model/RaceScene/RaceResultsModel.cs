using System.Collections.Generic;
using System.Linq;

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


        public void SetResults(CarModel playerCar, IEnumerable<CarModel> opponentCars, QuestionsModel questionsModel)
        {
            PlayerSpeed = (int)playerCar.CurrentSpeedKmph;
            IsFirst = opponentCars.All(m => playerCar.PassedMeters > m.PassedMeters);
            QuestionsCount = questionsModel.QuestionsCount;
            RightAnswersCount = questionsModel.RightAnswersCountTotal;
            WrongAnswersCount = questionsModel.WrongAnswersCountTotal;
            TurboBoostsCount = questionsModel.TurboBoostsCount;
        }
    }
}