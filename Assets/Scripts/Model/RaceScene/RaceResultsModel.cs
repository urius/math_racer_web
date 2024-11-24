namespace Model.RaceScene
{
    public class RaceResultsModel
    {
        public float PlayerSpeed { get; private set; }
        public float BotSpeed { get; private set; }
        public bool IsFirst { get; private set; }
        public int QuestionsCount { get; private set; }
        public int RightAnswersCount { get; private set; }
        public int WrongAnswersCount { get; private set; }


        public void SetResults(CarModel playerCar, CarModel botCar, QuestionsModel questionsModel)
        {
            PlayerSpeed = playerCar.CurrentSpeedKmph;
            BotSpeed = botCar.CurrentSpeedKmph;
            IsFirst = playerCar.PassedMeters > botCar.PassedMeters;
            QuestionsCount = questionsModel.QuestionsCount;
            RightAnswersCount = questionsModel.RightAnswersCount;
            WrongAnswersCount = questionsModel.WrongAnswersCount;
        }
    }
}