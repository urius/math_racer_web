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

        public float CurrentSpeed { get; private set; }
        public float TargetSpeed { get; private set; }

        public void Accelerate()
        {
            TargetSpeed += 5;
            if (TargetSpeed > 200)
            {
                TargetSpeed = 200;
            }
        }

        public void Decelerate()
        {
            TargetSpeed -= 2;
            if (TargetSpeed < 0)
            {
                TargetSpeed = 0;
            }
        }

        public void Update()
        {
            AdjustSpeed();
        }

        private void AdjustSpeed()
        {
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, TargetSpeed, 0.05f);
        }
    }
}