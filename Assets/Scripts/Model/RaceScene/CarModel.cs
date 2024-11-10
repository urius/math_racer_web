using Data;
using UnityEngine;

namespace Model.RaceScene
{
    public class CarModel
    {
        public const int MaxSpeed = 250;
        
        public readonly CarKey CarKey;

        private float _targetBodyRotation;

        public CarModel(CarKey carKey)
        {
            CarKey = carKey;
        }

        public float CurrentSpeedKmph { get; private set; }
        public float CurrentSpeedMetersPerFrame => CurrentSpeedKmph * Constants.KmphToMetersPerFrame;
        public float TargetSpeedKmph { get; private set; }
        public float CurrentBodyRotation { get; private set; }       
        public float PassedMeters { get; private set; }

        public void Accelerate()
        {
            TargetSpeedKmph += GetAcceleration();
            if (TargetSpeedKmph > MaxSpeed)
            {
                TargetSpeedKmph = MaxSpeed;
            }

            UpdateTargetBodyRotation();
        }

        public void Decelerate()
        {
            TargetSpeedKmph -= 5;
            if (TargetSpeedKmph < 0)
            {
                TargetSpeedKmph = 0;
            }
        }

        public void Update()
        {
            AdjustSpeed();
            UpdateBodyRotation();
            UpdatePassedDistance();
        }

        private void UpdatePassedDistance()
        {
            PassedMeters += CurrentSpeedMetersPerFrame;
        }

        private void AdjustSpeed()
        {
            CurrentSpeedKmph = Mathf.Lerp(CurrentSpeedKmph, TargetSpeedKmph, 0.032f);

            UpdateTargetBodyRotation();
        }

        private int GetAcceleration()
        {
            return TargetSpeedKmph <= 0 ? 20 : 15;
        }

        private void UpdateTargetBodyRotation()
        {
            _targetBodyRotation = -0.5f * (TargetSpeedKmph - CurrentSpeedKmph);
        }

        private void UpdateBodyRotation()
        {
            const float addRotationValue = 0.25f;
            var deltaRotation = _targetBodyRotation - CurrentBodyRotation;
            
            switch (deltaRotation)
            {
                case > 0:
                    CurrentBodyRotation += Mathf.Min(deltaRotation, addRotationValue);
                    break;
                case < 0:
                    CurrentBodyRotation -= Mathf.Min(-deltaRotation, addRotationValue);
                    break;
            }
        }
    }
}