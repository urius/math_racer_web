namespace Data.Dto.P2P
{
    public readonly struct P2PPlayerFinishedCommandBodyDto
    {
        public readonly int NetId;
        public readonly int Speed;
        public readonly int RaceTimeMs;

        public P2PPlayerFinishedCommandBodyDto(int netId, int speed, int raceTimeMs)
        {
            NetId = netId;
            Speed = speed;
            RaceTimeMs = raceTimeMs;
        }

        public static P2PPlayerFinishedCommandBodyDto Parse(string body)
        {
            var splitted = body.Split(Constants.P2PBodyParamsSeparator);

            return new P2PPlayerFinishedCommandBodyDto(int.Parse(splitted[0]), int.Parse(splitted[1]), int.Parse(splitted[2]));
        }

        public override string ToString()
        {
            const string sep = Constants.P2PBodyParamsSeparator;

            return $"{NetId}{sep}{Speed}{sep}{RaceTimeMs}";
        }
    }
}