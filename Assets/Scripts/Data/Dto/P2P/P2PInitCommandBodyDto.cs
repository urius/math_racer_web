namespace Data.Dto.P2P
{
    public readonly struct P2PInitCommandBodyDto
    {
        public readonly int HostId;
        public readonly CarKey HostCarKey;
        public readonly int JoinId;
        public readonly long HostTimeMs;

        public P2PInitCommandBodyDto(int hostId, CarKey hostCarKey, int joinId, long hostTimeMs)
        {
            HostId = hostId;
            HostCarKey = hostCarKey;
            JoinId = joinId;
            HostTimeMs = hostTimeMs;
        }

        public static P2PInitCommandBodyDto Parse(string body)
        {
            var splitted = body.Split(Constants.P2PBodyParamsSeparator);

            return new P2PInitCommandBodyDto(
                int.Parse(splitted[0]),
                (CarKey)int.Parse(splitted[1]),
                int.Parse(splitted[2]),
                long.Parse(splitted[3]));
        }

        public override string ToString()
        {
            const string sep = Constants.P2PBodyParamsSeparator;
            
            return $"{HostId}{sep}{(int)HostCarKey}{sep}{JoinId}{sep}{HostTimeMs.ToString()}";
        }
    }
}