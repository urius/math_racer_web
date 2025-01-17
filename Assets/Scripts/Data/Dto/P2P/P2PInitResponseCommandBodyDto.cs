namespace Data.Dto.P2P
{
    public readonly struct P2PInitResponseCommandBodyDto
    {
        public readonly CarKey JoinedCarKey;
        public readonly long HostTimeMs;

        public P2PInitResponseCommandBodyDto(CarKey joinedCarKey, long hostTimeMs)
        {
            JoinedCarKey = joinedCarKey;
            HostTimeMs = hostTimeMs;
        }

        public static P2PInitResponseCommandBodyDto Parse(string body)
        {
            var splitted = body.Split(Constants.P2PBodyParamsSeparator);

            return new P2PInitResponseCommandBodyDto((CarKey)int.Parse(splitted[0]), long.Parse(splitted[1]));
        }

        public override string ToString()
        {
            return $"{(int)JoinedCarKey}{Constants.P2PBodyParamsSeparator}{HostTimeMs.ToString()}";
        }
    }
}