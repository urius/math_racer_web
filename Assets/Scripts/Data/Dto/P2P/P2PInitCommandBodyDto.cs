namespace Data.Dto.P2P
{
    public readonly struct P2PInitCommandBodyDto
    {
        public readonly int HostId;
        public readonly int JoinId;
        public readonly long HostTimeMs;

        public P2PInitCommandBodyDto(int hostId, int joinId, long hostTimeMs)
        {
            HostId = hostId;
            JoinId = joinId;
            HostTimeMs = hostTimeMs;
        }

        public static P2PInitCommandBodyDto Parse(string body)
        {
            var splitted = body.Split(Constants.P2PBodyParamsSeparator);

            return new P2PInitCommandBodyDto(
                int.Parse(splitted[0]),
                int.Parse(splitted[1]),
                long.Parse(splitted[2]));
        }

        public override string ToString()
        {
            return string.Join(Constants.P2PBodyParamsSeparator,
                HostId, JoinId, HostTimeMs);
        }
    }
}