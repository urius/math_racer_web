namespace Model.RaceScene
{
    public class NetOpponentRaceResult
    {
        public readonly int Speed;
        public readonly int RaceTime;

        public NetOpponentRaceResult(int speed, int raceTime)
        {
            Speed = speed;
            RaceTime = raceTime;
        }
    }
}