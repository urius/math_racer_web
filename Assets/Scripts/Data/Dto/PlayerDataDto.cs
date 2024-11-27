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

        public PlayerDataDto(int level, int complexityLevel, int moneyAmount, int goldAmount)
        {
            Level = level;
            ComplexityLevel = complexityLevel;
            MoneyAmount = moneyAmount;
            GoldAmount = goldAmount;
        }

        public static PlayerDataDto FromDefault()
        {
            return new PlayerDataDto(1, 1, 0, 0);
        }
    }
}