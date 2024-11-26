namespace Model.RaceScene
{
    public class RaceResultsModel
    {
        public int PlayerSpeed { get; private set; }
        public int BotSpeed { get; private set; }
        public bool IsFirst { get; private set; }
        public int QuestionsCount { get; private set; }
        public int RightAnswersCount { get; private set; }
        public int WrongAnswersCount { get; private set; }


        public void SetResults(CarModel playerCar, CarModel botCar, QuestionsModel questionsModel)
        {
            PlayerSpeed = (int)playerCar.CurrentSpeedKmph;
            BotSpeed = (int)botCar.CurrentSpeedKmph;
            IsFirst = playerCar.PassedMeters > botCar.PassedMeters;
            QuestionsCount = questionsModel.QuestionsCount;
            RightAnswersCount = questionsModel.RightAnswersCountTotal;
            WrongAnswersCount = questionsModel.WrongAnswersCountTotal;
        }
    }
}