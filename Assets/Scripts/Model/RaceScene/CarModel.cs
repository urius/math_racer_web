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
        
        public readonly int NetId;
        public readonly CarKey CarKey;
        public readonly int PositionIndex;
        public readonly ReactiveFlag TurboFlag = new(initialValue: false);

        private readonly int _maxSpeed;
        private readonly int _acceleration;
        private readonly int _deceleration;
        
        private float _targetBodyRotation;
        private bool _suppressEventsFlag;
        private float _distanceToPlayerCar;
        private float _xOffsetLocal;

        public CarModel(CarRaceData carRaceData)
        {
            CarKey = carRaceData.CarKey;
            PositionIndex = carRaceData.CarPositionIndex;
            NetId = carRaceData.Id;
            
            _maxSpeed = carRaceData.MaxSpeed;
            _acceleration = carRaceData.Acceleration;
            _deceleration = (int)(_acceleration * 0.3f);
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
            if (TurboFlag.Value) return;
            
            TargetSpeedKmph += GetAcceleration();
            if (TargetSpeedKmph > _maxSpeed)
            {
                TargetSpeedKmph = _maxSpeed;
            }

            UpdateTargetBodyRotation();

            AccelerateHappened?.Invoke();
        }

        public void AccelerateTurbo()
        {
            TargetSpeedKmph += 2 * GetAcceleration();
            UpdateTargetBodyRotation();

            TurboFlag.Value = true;
        }

        public void Decelerate()
        {
            StopTurbo();
            
            TargetSpeedKmph -= _deceleration;
            if (TargetSpeedKmph < 0)
            {
                TargetSpeedKmph = 0;
            }
            
            DecelerateHappened?.Invoke();
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
            TargetSpeedKmph = Mathf.Min(TargetSpeedKmph, _maxSpeed);
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
            var deltaSpeed = deltaTime * _acceleration;
            var isTurbo = TurboFlag.Value;
            
            if (CurrentSpeedKmph < TargetSpeedKmph)
            {
                var turboMult = TurboFlag.Value ? 10 : 1;
                CurrentSpeedKmph += deltaSpeed * turboMult;
                
                if (CurrentSpeedKmph > TargetSpeedKmph)
                {
                    CurrentSpeedKmph = TargetSpeedKmph;
                }
            }
            else if (CurrentSpeedKmph > TargetSpeedKmph 
                     && isTurbo == false)
            {
                CurrentSpeedKmph -= deltaSpeed;
                if (CurrentSpeedKmph < TargetSpeedKmph)
                {
                    CurrentSpeedKmph = TargetSpeedKmph;
                }
            }

            if (isTurbo && CurrentSpeedKmph >= TargetSpeedKmph)
            {
                StopTurbo();
            }

            UpdateTargetBodyRotation();
        }

        private int GetAcceleration()
        {
            return TargetSpeedKmph <= 0 ? (int)(_acceleration * 1.5f) : _acceleration;
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

        public void SetDistanceToPlayerCar(float distanceToPlayerCar)
        {
            _distanceToPlayerCar = distanceToPlayerCar;
        }
    }
}