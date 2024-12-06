using System.Collections.Generic;
using System.Linq;
using Data;

namespace Model
{
    public class PlayerModel
    {
        private readonly List<CarKey> _boughtCars;
        
        public PlayerModel(int level, int complexityLevel, int moneyAmount, int goldAmount, IEnumerable<int> boughtCars)
        {
            Level = level;
            ComplexityLevel = complexityLevel;
            MoneyAmount = moneyAmount;
            GoldAmount = goldAmount;
            
            _boughtCars = boughtCars.Select(c => (CarKey)c).ToList();
        }

        public int Level { get; private set; }
        public int ComplexityLevel { get; private set; }
        public int MoneyAmount { get; private set; }
        public int GoldAmount { get; private set; }
        public IReadOnlyList<CarKey> BoughtCars => _boughtCars;

        public int GetOverallComplexityPercent()
        {
            return ComplexityLevel * 10 + Level;
        }
    }
}