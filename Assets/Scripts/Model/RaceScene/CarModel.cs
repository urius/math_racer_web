using Data;
using UnityEngine;

namespace Model.RaceScene
{
    public class CarModel
    {
        public readonly CarKey CarKey;

        public CarModel(CarKey carKey)
        {
            CarKey = carKey;
        }

        public float CurrentSpeedKmph { get; private set; }
        public float TargetSpeedKmph { get; private set; }

        public void Accelerate()
        {
            TargetSpeedKmph += 5;
            if (TargetSpeedKmph > 200)
            {
                TargetSpeedKmph = 200;
            }
        }

        public void Decelerate()
        {
            TargetSpeedKmph -= 2;
            if (TargetSpeedKmph < 0)
            {
                TargetSpeedKmph = 0;
            }
        }

        public void Update()
        {
            AdjustSpeed();
        }

        private void AdjustSpeed()
        {
            CurrentSpeedKmph = Mathf.Lerp(CurrentSpeedKmph, TargetSpeedKmph, 0.05f);
        }
    }
}