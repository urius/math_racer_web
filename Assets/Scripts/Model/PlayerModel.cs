namespace Model
{
    public class PlayerModel
    {
        public PlayerModel(int level, int complexityLevel, int moneyAmount)
        {
            Level = level;
            ComplexityLevel = complexityLevel;
            MoneyAmount = moneyAmount;
        }

        public int Level { get; private set; }
        public int ComplexityLevel { get; private set; }
        public int MoneyAmount { get; private set; }
    }
}