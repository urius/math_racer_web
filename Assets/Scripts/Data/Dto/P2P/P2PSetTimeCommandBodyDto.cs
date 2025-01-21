namespace Data.Dto.P2P
{
    public readonly struct P2PSetTimeCommandBodyDto
    {
        public readonly long HostTimeMs;
        public readonly int PingMs;
        public readonly long EstimatedJoinSideTimeMs;

        public P2PSetTimeCommandBodyDto(long hostTimeMs, int pingMs)
        {
            HostTimeMs = hostTimeMs;
            PingMs = pingMs;
            EstimatedJoinSideTimeMs = HostTimeMs + PingMs;
        }
        
        public static P2PSetTimeCommandBodyDto Parse(string body)
        {
            var splitted = body.Split(Constants.P2PBodyParamsSeparator);

            return new P2PSetTimeCommandBodyDto(long.Parse(splitted[0]), int.Parse(splitted[1]));
        }

        public override string ToString()
        {
            const string sep = Constants.P2PBodyParamsSeparator;
            
            return $"{HostTimeMs.ToString()}{sep}{PingMs.ToString()}";
        }
    }
}