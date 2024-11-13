using System;

namespace Data.Dto
{
    [Serializable]
    public struct PlayerDataDto
    {
        public int Level;
        public int ComplexityLevel;
        public int MoneyAmount;

        public PlayerDataDto(int level, int complexityLevel, int moneyAmount)
        {
            Level = level;
            ComplexityLevel = complexityLevel;
            MoneyAmount = moneyAmount;
        }

        public static PlayerDataDto FromDefault()
        {
            return new PlayerDataDto(1, 1, 0);
        }
    }
}