using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

namespace Model
{
    public class PlayerModel
    {
        public event Action<CarKey> CurrentCarUpdated;
        public event Action<int> CashAmountChanged;
        public event Action<int> InsufficientCash;
        public event Action<int> GoldAmountChanged;
        public event Action<int> InsufficientGold;
        
        private readonly List<CarKey> _boughtCars;

        public PlayerModel(
            int level,
            int complexityLevel,
            int cashAmount,
            int goldAmount,
            int currentCar,
            IEnumerable<int> boughtCars)
        {
            Level = level;
            ComplexityLevel = complexityLevel;
            CashAmount = cashAmount;
            GoldAmount = goldAmount;
            CurrentCar = (CarKey)currentCar;
            
            _boughtCars = boughtCars.Select(c => (CarKey)c).ToList();
        }

        public int Level { get; private set; }
        public int ComplexityLevel { get; private set; }
        public int CashAmount { get; private set; }
        public int GoldAmount { get; private set; }
        public CarKey CurrentCar { get; private set; }
        public IReadOnlyList<CarKey> BoughtCars => _boughtCars;

        public bool TrySpend(int moneyAmount)
        {
            return moneyAmount > 0 ? TrySpendCash(moneyAmount) : TrySpendGold(Mathf.Abs(moneyAmount));
        }

        private bool TrySpendCash(int cashSpentAmount)
        {
            if (CashAmount >= cashSpentAmount)
            {
                CashAmount -= cashSpentAmount;
                CashAmountChanged?.Invoke(-cashSpentAmount);
                
                return true;
            }
            else
            {
                InsufficientCash?.Invoke(cashSpentAmount - CashAmount);

                return false;
            }
        }

        private bool TrySpendGold(int goldSpentAmount)
        {
            if (GoldAmount >= goldSpentAmount)
            {
                GoldAmount -= goldSpentAmount;
                GoldAmountChanged?.Invoke(-goldSpentAmount);
                
                return true;
            }
            else
            {
                InsufficientGold?.Invoke(goldSpentAmount - GoldAmount);

                return false;
            }
        }

        public int GetOverallComplexityPercent()
        {
            return ComplexityLevel * 10 + Level;
        }

        public bool IsCarBought(CarKey carKey)
        {
            return _boughtCars.Contains(carKey);
        }

        public void SetCurrentCar(CarKey carKey)
        {
            if (CurrentCar == carKey) return;

            CurrentCar = carKey;
            CurrentCarUpdated?.Invoke(CurrentCar);
        }

        public void AddBoughtCar(CarKey carKey)
        {
            _boughtCars.Add(carKey);
        }
    }
}