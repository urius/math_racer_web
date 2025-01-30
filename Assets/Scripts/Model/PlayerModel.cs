using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Helpers;
using UnityEngine;
using Utils.CryptoValue;

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
        
        public readonly AudioSettingsModel AudioSettingsModel;
        
        private readonly List<CarKey> _boughtCars;

        private readonly CryptoInt _expAmountCrypto = new();
        private readonly CryptoInt _cashAmountCrypto = new();
        private readonly CryptoInt _goldAmountCrypto = new();

        public PlayerModel(int expAmount,
            int complexityLevel,
            int cashAmount,
            int goldAmount,
            int currentCar,
            IEnumerable<int> boughtCars,
            AudioSettingsModel audioSettingsModel,
            int lastActiveDaysConsecutiveOnBonusTake)
        {
            AudioSettingsModel = audioSettingsModel;
            SetExpAmount(expAmount);
            ComplexityLevel = complexityLevel;
            CashAmount = cashAmount;
            GoldAmount = goldAmount;
            LastActiveDaysConsecutiveOnBonusTake = lastActiveDaysConsecutiveOnBonusTake;
            CurrentCar = (CarKey)currentCar;
            
            _boughtCars = boughtCars.Select(c => (CarKey)c).ToList();
        }

        public int ExpAmount => _expAmountCrypto.Value;
        public int Level { get; private set; }
        public int ComplexityLevel { get; private set; }
        public int CashAmount
        {
            get => _cashAmountCrypto.Value;
            private set => _cashAmountCrypto.Value = value;
        }
        public int GoldAmount
        {
            get => _goldAmountCrypto.Value;
            private set => _goldAmountCrypto.Value = value;
        }

        public int LastActiveDaysConsecutiveOnBonusTake { get; private set; }
        public int CrystalsAmount => GoldAmount;
        public CarKey CurrentCar { get; private set; }
        public IReadOnlyList<CarKey> BoughtCars => _boughtCars;

        public bool TrySpend(int moneyAmount)
        {
            return moneyAmount > 0 ? TrySpendCash(moneyAmount) : TrySpendGold(Mathf.Abs(moneyAmount));
        }

        public void AddCash(int cashToAdd)
        {
            var cashAmountBeforeAdd = CashAmount;
            if (cashAmountBeforeAdd + cashToAdd < 0)
            {
                cashToAdd = -cashAmountBeforeAdd;
            }

            CashAmount = cashAmountBeforeAdd + cashToAdd;
            CashAmountChanged?.Invoke(cashToAdd);
        }

        public void AddGold(int goldToAdd)
        {
            var goldAmountBeforeAdd = GoldAmount;
            if (goldAmountBeforeAdd + goldToAdd < 0)
            {
                goldToAdd = -goldAmountBeforeAdd;
            }

            GoldAmount = goldAmountBeforeAdd + goldToAdd;
            GoldAmountChanged?.Invoke(goldToAdd);
        }

        public bool TrySpendCash(int cashSpentAmount)
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

        public bool TrySpendGold(int goldSpentAmount)
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

        public void IncreaseComplexityLevel()
        {
            ComplexityLevel++;
        }


        public void DecreaseComplexityLevel()
        {
            if (ComplexityLevel <= 0)
            {
                ComplexityLevel = 0;
                return;
            }
            ComplexityLevel--;
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
            _expAmountCrypto.Value = expAmount;
            Level = LevelPointsHelper.GetLevelByExpPointsAmount(expAmount);
            
            ExpAmountChanged?.Invoke(delta);
        }
    }
}