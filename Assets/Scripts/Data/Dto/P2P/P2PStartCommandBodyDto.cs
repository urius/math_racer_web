using System.Linq;

namespace Data.Dto.P2P
{
    public readonly struct P2PStartCommandBodyDto
    {
        public readonly P2PPlayerDataDto[] PlayerDataDtoList;

        public P2PStartCommandBodyDto(P2PPlayerDataDto[] playerDataDtoList)
        {
            PlayerDataDtoList = playerDataDtoList;
        }
        
        public static P2PStartCommandBodyDto Parse(string body)
        {
            var splitted = body.Split(Constants.P2PBodyParamsSeparator);

            var p2pPlayerDataDtoList = splitted[0]
                .Split(Constants.P2PBodyArraySeparator)
                .Select(P2PPlayerDataDto.Parse)
                .ToArray();
            
            return new P2PStartCommandBodyDto(p2pPlayerDataDtoList);
        }

        public override string ToString()
        {
            return string.Join(Constants.P2PBodyArraySeparator, PlayerDataDtoList.Select(dto => dto.ToString()));
        }
    }

    public readonly struct P2PPlayerDataDto
    {
        public readonly int Id;
        public readonly string Name;
        public readonly CarKey CarKey;
        public readonly int PositionIndex;

        private const string Separator = Constants.P2PBodyParamsSeparator2;

        public P2PPlayerDataDto(int id, CarKey carKey, int positionIndex, string name)
        {
            Id = id;
            CarKey = carKey;
            PositionIndex = positionIndex;
            Name = name;
        }
        
        public static P2PPlayerDataDto FromP2PPlayerData(P2PPlayerData playerData)
        {
            return new P2PPlayerDataDto(playerData.Id, playerData.CarKey, playerData.PositionIndex, playerData.PlayerName);
        }
        
        public static P2PPlayerDataDto Parse(string body)
        {
            var splitted = body.Split(Separator);

            return new P2PPlayerDataDto(
                int.Parse(splitted[0]), (CarKey)int.Parse(splitted[1]), int.Parse(splitted[2]), splitted[3]);
        }

        public override string ToString()
        {
            return string.Join(Separator,
                Id, ((int)CarKey).ToString(), PositionIndex.ToString(), Name);
        }
    }
}