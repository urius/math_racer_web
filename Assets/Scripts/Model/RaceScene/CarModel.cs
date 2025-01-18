using System;
using Data;
using UnityEngine;
using Utils.ReactiveValue;

namespace Model.RaceScene
{
    public class CarModel
    {
        public event Action AccelerateHappened;
        public event Action DecelerateHappened;
        
        public const int MaxSpeed = 250;
        
        public readonly CarKey CarKey;
        public readonly ReactiveFlag TurboFlag = new(initialValue: false);
        
        private float _targetBodyRotation;
        private float _turboTargetSpeed;
        private bool _suppressEventsFlag;
        private float _distanceToPlayerCar;
        private float _xOffsetLocal;

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
        public float XOffset => _xOffsetLocal - _distanceToPlayerCar;

        public void Accelerate()
        {
            TargetSpeedKmph += GetAcceleration();
            if (TargetSpeedKmph > MaxSpeed)
            {
                TargetSpeedKmph = MaxSpeed;
            }

            UpdateTargetBodyRotation();

            Dispatch(AccelerateHappened);
        }

        public void AccelerateTurbo()
        {
            _suppressEventsFlag = true;
            Accelerate();
            Accelerate();
            _suppressEventsFlag = false;

            _turboTargetSpeed = TargetSpeedKmph;
            TurboFlag.Value = true;
        }

        public void Decelerate()
        {
            StopTurbo();
            
            TargetSpeedKmph -= 5;
            if (TargetSpeedKmph < 0)
            {
                TargetSpeedKmph = 0;
            }
            
            Dispatch(DecelerateHappened);
        }

        public void Update(float deltaTime)
        {
            AdjustSpeed(deltaTime);
            UpdatePassedDistance(deltaTime);
            UpdateBodyRotation(deltaTime);
            UpdateXOffset(deltaTime);
        }

        private void StopTurbo()
        {
            _turboTargetSpeed = 0;
            TurboFlag.Value = false;
        }

        private void UpdateXOffset(float deltaTime)
        {
            const float offsetMax = 1.5f;
            const float deltaSpeedKmBounds = 2;
            
            var deltaSpeedKmh = TargetSpeedKmph - CurrentSpeedKmph;
            var mult = Mathf.Abs(_xOffsetLocal) > 0.8f * offsetMax ? 0.2f : 0.5f;
            var deltaOffset = mult * deltaTime;

            if (deltaSpeedKmh > deltaSpeedKmBounds)
            {
                _xOffsetLocal += deltaOffset;
            }
            else if (deltaSpeedKmh < -deltaSpeedKmBounds)
            {
                _xOffsetLocal -= deltaOffset;
            }
            else if (_xOffsetLocal != 0)
            {
                _xOffsetLocal *= 0.995f;
            }

            _xOffsetLocal = Mathf.Clamp(_xOffsetLocal, -offsetMax, offsetMax);
            if (Mathf.Abs(_xOffsetLocal) < 0.001f)
            {
                _xOffsetLocal = 0;
            }
        }

        private void UpdatePassedDistance(float deltaTime)
        {
            CurrentUpdateMetersPassed = CurrentSpeedMetersPerSecond * deltaTime;
            PassedMeters += CurrentUpdateMetersPassed;
        }

        private void AdjustSpeed(float deltaTime)
        {
            var deltaSpeed = deltaTime * 6;
            
            if (CurrentSpeedKmph < TargetSpeedKmph)
            {
                var turboMult = TurboFlag.Value ? 10 : 1;
                CurrentSpeedKmph += deltaSpeed * turboMult;

                if (CurrentSpeedKmph >= _turboTargetSpeed)
                {
                    StopTurbo();
                }
                
                if (CurrentSpeedKmph > TargetSpeedKmph)
                {
                    CurrentSpeedKmph = TargetSpeedKmph;
                }
            }
            else if (CurrentSpeedKmph > TargetSpeedKmph)
            {
                CurrentSpeedKmph -= deltaSpeed;
                if (CurrentSpeedKmph < TargetSpeedKmph)
                {
                    CurrentSpeedKmph = TargetSpeedKmph;
                }
            }

            UpdateTargetBodyRotation();
        }

        private int GetAcceleration()
        {
            return (TargetSpeedKmph <= 0 ? 15 : 10);
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

        private void Dispatch(Action action)
        {
            if (_suppressEventsFlag == false)
            {
                action?.Invoke();
            }
        }

        public void SetDistanceToPlayerCar(float distanceToPlayerCar)
        {
            _distanceToPlayerCar = distanceToPlayerCar;
        }
    }
}