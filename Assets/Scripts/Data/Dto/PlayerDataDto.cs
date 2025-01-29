// ReSharper disable InconsistentNaming

using System;

namespace Data.Dto
{
    [Serializable]
    public struct PlayerDataDto
    {
        public int expAmount;
        public int complexityLevel;
        public int cashAmount;
        public int goldAmount;
        public int currentCar;
        public int[] boughtCars;
        public AudioSettingsDto audioSettings;
        public int lastActiveDaysConsecutiveOnBonusTake;

        public PlayerDataDto(int expAmount,
            int complexityLevel,
            int cashAmount,
            int goldAmount,
            int currentCar,
            int[] boughtCars,
            AudioSettingsDto audioSettings,
            int lastActiveDaysConsecutiveOnBonusTake)
        {
            this.expAmount = expAmount;
            this.complexityLevel = complexityLevel;
            this.cashAmount = cashAmount;
            this.goldAmount = goldAmount;
            this.currentCar = currentCar;
            this.boughtCars = boughtCars;
            this.audioSettings = audioSettings;
            this.lastActiveDaysConsecutiveOnBonusTake = lastActiveDaysConsecutiveOnBonusTake;
        }

        public static PlayerDataDto FromDefault()
        {
            return new PlayerDataDto(
                0,
                1,
                0,
                0,
                (int)CarKey.Bug,
                new[]
                {
                    (int)CarKey.Bug
                },
                AudioSettingsDto.FromDefault(),
                -1);
        }
    }
}