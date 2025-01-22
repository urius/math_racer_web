namespace Data.Dto.P2P
{
    public readonly struct P2PPlayerFinishedCommandBodyDto
    {
        public readonly int NetId;
        public readonly int Speed;
        public readonly int RaceTimeMs;
        public readonly int RightAnswersCount;
        public readonly int WrongAnswersCount;

        public P2PPlayerFinishedCommandBodyDto(int netId, int speed, int raceTimeMs, int rightAnswersCount, int wrongAnswersCount)
        {
            NetId = netId;
            Speed = speed;
            RaceTimeMs = raceTimeMs;
            RightAnswersCount = rightAnswersCount;
            WrongAnswersCount = wrongAnswersCount;
        }

        public static P2PPlayerFinishedCommandBodyDto Parse(string body)
        {
            var splitted = body.Split(Constants.P2PBodyParamsSeparator);

            return new P2PPlayerFinishedCommandBodyDto(
                int.Parse(splitted[0]),
                int.Parse(splitted[1]),
                int.Parse(splitted[2]),
                int.Parse(splitted[3]),
                int.Parse(splitted[4]));
        }

        public override string ToString()
        {
            const string sep = Constants.P2PBodyParamsSeparator;

            return $"{NetId}{sep}{Speed}{sep}{RaceTimeMs}{sep}{RightAnswersCount}{sep}{WrongAnswersCount}";
        }
    }
}