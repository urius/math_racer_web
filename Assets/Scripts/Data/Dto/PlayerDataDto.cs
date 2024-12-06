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
        public int[] BoughtCars;

        public PlayerDataDto(int level,
            int complexityLevel,
            int moneyAmount,
            int goldAmount,
            int[] boughtCars)
        {
            Level = level;
            ComplexityLevel = complexityLevel;
            MoneyAmount = moneyAmount;
            GoldAmount = goldAmount;
            BoughtCars = boughtCars;
        }

        public static PlayerDataDto FromDefault()
        {
            return new PlayerDataDto(
                1,
                1,
                0,
                0,
                new[]
                {
                    (int)CarKey.Bug
                });
        }
    }
}