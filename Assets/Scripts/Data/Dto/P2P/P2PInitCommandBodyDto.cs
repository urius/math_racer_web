namespace Data.Dto.P2P
{
    public readonly struct P2PInitCommandBodyDto
    {
        public readonly CarKey HostCarKey;
        public readonly int PositionIndex;
        public readonly long HostTimeMs;

        public P2PInitCommandBodyDto(CarKey hostCarKey, int positionIndex, long hostTimeMs)
        {
            HostCarKey = hostCarKey;
            PositionIndex = positionIndex;
            HostTimeMs = hostTimeMs;
        }

        public static P2PInitCommandBodyDto Parse(string body)
        {
            var splitted = body.Split(Constants.P2PBodyParamsSeparator);

            return new P2PInitCommandBodyDto((CarKey)int.Parse(splitted[0]), int.Parse(splitted[1]), long.Parse(splitted[2]));
        }

        public override string ToString()
        {
            const string sep = Constants.P2PBodyParamsSeparator;
            
            return $"{(int)HostCarKey}{sep}{PositionIndex}{sep}{HostTimeMs.ToString()}";
        }
    }
}