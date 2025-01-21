namespace Data.Dto.P2P
{
    public readonly struct P2PPingCommandBodyDto
    {
        public readonly long HostTimeMs;
        public readonly long JoinTimeMs;

        public P2PPingCommandBodyDto(long hostTimeMs, long joinTimeMs = 0)
        {
            HostTimeMs = hostTimeMs;
            JoinTimeMs = joinTimeMs;
        }
        
        public static P2PPingCommandBodyDto Parse(string body)
        {
            var splitted = body.Split(Constants.P2PBodyParamsSeparator);

            return new P2PPingCommandBodyDto(long.Parse(splitted[0]), long.Parse(splitted[1]));
        }

        public override string ToString()
        {
            const string sep = Constants.P2PBodyParamsSeparator;

            return $"{HostTimeMs.ToString()}{sep}{JoinTimeMs.ToString()}";
        }
    }
}