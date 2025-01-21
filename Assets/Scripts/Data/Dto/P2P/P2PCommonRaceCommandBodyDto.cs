namespace Data.Dto.P2P
{
    public readonly struct P2PCommonRaceCommandBodyDto
    {
        public readonly int Id;
        public readonly long TimestampMs;

        public P2PCommonRaceCommandBodyDto(int id, long timestampMs)
        {
            Id = id;
            TimestampMs = timestampMs;
        }
        
        public static P2PCommonRaceCommandBodyDto Parse(string body)
        {
            var splitted = body.Split(Constants.P2PBodyParamsSeparator);

            return new P2PCommonRaceCommandBodyDto(int.Parse(splitted[0]), long.Parse(splitted[1]));
        }

        public override string ToString()
        {
            const string sep = Constants.P2PBodyParamsSeparator;

            return $"{Id.ToString()}{sep}{TimestampMs.ToString()}";
        }
    }
}