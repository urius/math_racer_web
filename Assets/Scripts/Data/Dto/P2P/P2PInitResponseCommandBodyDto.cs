namespace Data.Dto.P2P
{
    public readonly struct P2PInitResponseCommandBodyDto
    {
        public readonly string JoinedPlayerName;
        public readonly CarKey JoinedCarKey;
        public readonly long HostTimeMs;

        public P2PInitResponseCommandBodyDto(string joinedPlayerName, CarKey joinedCarKey, long hostTimeMs)
        {
            JoinedPlayerName = joinedPlayerName;
            JoinedCarKey = joinedCarKey;
            HostTimeMs = hostTimeMs;
        }

        public static P2PInitResponseCommandBodyDto Parse(string body)
        {
            var splitted = body.Split(Constants.P2PBodyParamsSeparator);

            return new P2PInitResponseCommandBodyDto(
                splitted[0], (CarKey)int.Parse(splitted[1]), long.Parse(splitted[2]));
        }

        public override string ToString()
        {
            return string.Join(Constants.P2PBodyParamsSeparator,
                JoinedPlayerName, ((int)JoinedCarKey).ToString(), HostTimeMs.ToString());
        }
    }
}