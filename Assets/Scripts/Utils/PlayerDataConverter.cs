using Data.Dto;
using Model;

namespace Utils
{
    public static class PlayerDataConverter
    {
        public static PlayerModel ToPlayerModel(PlayerDataDto playerDataDto)
        {
            return new PlayerModel(
                playerDataDto.ExpAmount,
                playerDataDto.ComplexityLevel,
                playerDataDto.MoneyAmount,
                playerDataDto.GoldAmount,
                playerDataDto.CurrentCar,
                playerDataDto.BoughtCars);
        }
    }
}