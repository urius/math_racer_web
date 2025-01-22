namespace Model.RaceScene
{
    public class NetOpponentRaceResult
    {
        public readonly int Speed;
        public readonly int RaceTimeMs;
        public readonly int RightAnswersCount;
        public readonly int WrongAnswersCount;

        public NetOpponentRaceResult(int speed, int raceTimeMs, int rightAnswersCount, int wrongAnswersCount)
        {
            Speed = speed;
            RaceTimeMs = raceTimeMs;
            RightAnswersCount = rightAnswersCount;
            WrongAnswersCount = wrongAnswersCount;
        }
    }
}