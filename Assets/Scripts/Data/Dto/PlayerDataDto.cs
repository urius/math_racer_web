using System;

namespace Data.Dto
{
    [Serializable]
    public struct PlayerDataDto
    {
        public int Level;
        public int ComplexityLevel;
        public int MoneyAmount;
        public int GoldAmount;
        public int CurrentCar;
        public int[] BoughtCars;

        public PlayerDataDto(int level,
            int complexityLevel,
            int moneyAmount,
            int goldAmount,
            int currentCar,
            int[] boughtCars)
        {
            Level = level;
            ComplexityLevel = complexityLevel;
            MoneyAmount = moneyAmount;
            GoldAmount = goldAmount;
            CurrentCar = currentCar;
            BoughtCars = boughtCars;
        }

        public static PlayerDataDto FromDefault()
        {
            return new PlayerDataDto(
                1,
                1,
                1000,
                0,
                (int)CarKey.Bug,
                new[]
                {
                    (int)CarKey.Bug
                });
        }
    }
}