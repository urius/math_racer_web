namespace Model
{
    public class PlayerModel
    {
        public PlayerModel(int level, int complexityLevel, int moneyAmount, int goldAmount)
        {
            Level = level;
            ComplexityLevel = complexityLevel;
            MoneyAmount = moneyAmount;
            GoldAmount = goldAmount;
        }

        public int Level { get; private set; }
        public int ComplexityLevel { get; private set; }
        public int MoneyAmount { get; private set; }
        public int GoldAmount { get; private set; }

        public int GetOverallComplexityPercent()
        {
            return ComplexityLevel * 10 + Level;
        }
    }
}