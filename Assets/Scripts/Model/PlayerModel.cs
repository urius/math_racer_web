using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Helpers;
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
        public event Action<int> ExpAmountChanged;
        
        private readonly List<CarKey> _boughtCars;

        public PlayerModel(
            int expAmount,
            int complexityLevel,
            int cashAmount,
            int goldAmount,
            int currentCar,
            IEnumerable<int> boughtCars)
        {
            SetExpAmount(expAmount);
            ComplexityLevel = complexityLevel;
            CashAmount = cashAmount;
            GoldAmount = goldAmount;
            CurrentCar = (CarKey)currentCar;
            
            _boughtCars = boughtCars.Select(c => (CarKey)c).ToList();
        }

        public int ExpAmount { get; private set; }
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

        public void AddCash(int cashToAdd)
        {
            if (CashAmount + cashToAdd < 0)
            {
                cashToAdd = -CashAmount;
            }

            CashAmount += cashToAdd;
            CashAmountChanged?.Invoke(cashToAdd);
        }

        public void AddGold(int goldToAdd)
        {
            if (GoldAmount + goldToAdd < 0)
            {
                goldToAdd = -GoldAmount;
            }

            GoldAmount += goldToAdd;
            GoldAmountChanged?.Invoke(goldToAdd);
        }

        private bool TrySpendCash(int cashSpentAmount)
        {
            if (CashAmount >= cashSpentAmount)
            {
                AddCash(-cashSpentAmount);
                
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
                AddGold(-goldSpentAmount);
                
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
        
        public void AddExpAmount(int amountToAdd)
        {
            SetExpAmount(ExpAmount + amountToAdd);
        }

        public void SetExpAmount(int expAmount)
        {
            var delta = expAmount - ExpAmount;
            ExpAmount = expAmount;
            Level = LevelPointsHelper.GetLevelByExpPointsAmount(ExpAmount);
            
            ExpAmountChanged?.Invoke(delta);
        }
    }
}