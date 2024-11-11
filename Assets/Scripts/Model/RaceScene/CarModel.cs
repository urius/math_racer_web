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
        public float CurrentSpeedMetersPerSecond => CurrentSpeedKmph * Constants.KmphToMps;
        public float TargetSpeedKmph { get; private set; }
        public float CurrentBodyRotation { get; private set; }
        public float PassedMeters { get; private set; }
        public float CurrentUpdateMetersPassed { get; private set; }
        public float XOffset { get; private set; }

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

        public void Update(float deltaTime)
        {
            AdjustSpeed(deltaTime);
            UpdatePassedDistance(deltaTime);
            UpdateBodyRotation(deltaTime);
            UpdateXOffset(deltaTime);
        }

        private void UpdateXOffset(float deltaTime)
        {
            const float offsetMax = 1.5f;
            const float deltaSpeedKmBounds = 2;
            
            var deltaSpeedKmh = TargetSpeedKmph - CurrentSpeedKmph;
            var mult = Mathf.Abs(XOffset) > 0.8f * offsetMax ? 0.2f : 0.5f;
            var deltaOffset = mult * deltaTime;

            if (deltaSpeedKmh > deltaSpeedKmBounds)
            {
                XOffset += deltaOffset;
            }
            else if (deltaSpeedKmh < -deltaSpeedKmBounds)
            {
                XOffset -= deltaOffset;
            }
            else if (XOffset != 0)
            {
                XOffset *= 0.995f;
            }

            XOffset = Mathf.Clamp(XOffset, -offsetMax, offsetMax);
            if (Mathf.Abs(XOffset) < 0.001f)
            {
                XOffset = 0;
            }
        }

        private void UpdatePassedDistance(float deltaTime)
        {
            CurrentUpdateMetersPassed = CurrentSpeedMetersPerSecond * deltaTime;
            PassedMeters += CurrentUpdateMetersPassed;
        }

        private void AdjustSpeed(float deltaTime)
        {
            CurrentSpeedKmph = Mathf.Lerp(CurrentSpeedKmph, TargetSpeedKmph, deltaTime);

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

        private void UpdateBodyRotation(float deltaTime)
        {
            var addRotationValue = 20 * deltaTime;
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