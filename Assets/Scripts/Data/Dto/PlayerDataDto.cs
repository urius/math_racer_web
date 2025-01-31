// ReSharper disable InconsistentNaming

using System;
using UnityEngine.Serialization;

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
        public int previousStartUtcTimestamp;
        public int currentStartUtcTimestamp;
        public int sequentialDaysPlaying;

        public PlayerDataDto(int expAmount,
            int complexityLevel,
            int cashAmount,
            int goldAmount,
            int currentCar,
            int[] boughtCars,
            AudioSettingsDto audioSettings,
            int previousStartUtcTimestamp,
            int currentStartUtcTimestamp,
            int sequentialDaysPlaying)
        {
            this.expAmount = expAmount;
            this.complexityLevel = complexityLevel;
            this.cashAmount = cashAmount;
            this.goldAmount = goldAmount;
            this.currentCar = currentCar;
            this.boughtCars = boughtCars;
            this.audioSettings = audioSettings;
            this.previousStartUtcTimestamp = previousStartUtcTimestamp;
            this.currentStartUtcTimestamp = currentStartUtcTimestamp;
            this.sequentialDaysPlaying = sequentialDaysPlaying;
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
                0,
                0,
                1);
        }
    }
}