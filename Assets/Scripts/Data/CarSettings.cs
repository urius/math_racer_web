using UnityEngine;

namespace Data
{
    public struct CarSettings
    {
        public readonly CarKey CarKey;
        public readonly Sprite IconSprite;
        public readonly int Price;
        public readonly int UnlockLevel;
        public readonly int Acceleration;
        public readonly float AccelerationPercent;
        public readonly int MaxSpeed;
        public readonly float MaxSpeedPercent;

        public CarSettings(CarKey carKey, Sprite iconSprite, int price, int unlockLevel, int acceleration, int maxSpeed)
        {
            CarKey = carKey;
            IconSprite = iconSprite;
            Price = price;
            UnlockLevel = unlockLevel;
            Acceleration = acceleration;
            AccelerationPercent = Mathf.Min(1f, (float)acceleration / Constants.MaxCarAcceleration);
            MaxSpeed = maxSpeed;
            MaxSpeedPercent = Mathf.Min(1f, (float)MaxSpeed / Constants.MaxCarSpeedKmph);
        }
    }
}