using System;
using Extensions;
using Holders;
using Infra.Instance;
using Model.RaceScene;
using UnityEngine;
using View;
using Random = System.Random;

namespace Controller.RaceScene
{
    public class RaceSceneBotCarController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();

        private readonly CarModel _carModel;
        private readonly Transform _targetTransform;
        private readonly Random _random;

        private RaceModel _raceModel;
        private CarView _carView;
        private CarModel _playerCarModel;
        private int _lastChangeSpeedSecondsPassed;

        public RaceSceneBotCarController(CarModel carModel, Transform targetTransform)
        {
            _carModel = carModel;
            _targetTransform = targetTransform;

            _random = new Random();
        }

        public override void Initialize()
        {
            _raceModel = _modelsHolder.GetRaceModel();
            _playerCarModel = _raceModel.PlayerCar;
            
            _carView = Instantiate<CarView>(_carModel.CarKey.ToPrefabKey(), _targetTransform);

            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
            
            Destroy(_carView);
            _carView = null;
        }

        private void Subscribe()
        {
            _updatesProvider.GameplayUpdate += OnGameplayUpdate;
            _updatesProvider.GameplayHalfSecondPassed += OnGameplayHalfSecondPassed;
            _updatesProvider.GameplaySecondPassed += OnGameplaySecondPassed;
        }

        private void Unsubscribe()
        {
            _updatesProvider.GameplayUpdate -= OnGameplayUpdate;
            _updatesProvider.GameplayHalfSecondPassed -= OnGameplayHalfSecondPassed;
            _updatesProvider.GameplaySecondPassed -= OnGameplaySecondPassed;
        }

        private void OnGameplaySecondPassed()
        {
            _lastChangeSpeedSecondsPassed++;
        }

        private void OnGameplayUpdate()
        {
            _carModel.Update(Time.deltaTime);

            UpdateCarView(Time.deltaTime);
        }

        private void OnGameplayHalfSecondPassed()
        {
            if (_carModel.TargetSpeedKmph <= 0)
            {
                if (_playerCarModel.CurrentSpeedKmph > 0 && _random.Next(100) < 60)
                {
                    Accelerate();
                }
            }
            else
            {
                var deltaPassedMeters = _carModel.PassedMeters - _playerCarModel.PassedMeters;
                if (deltaPassedMeters < 0 
                    && _lastChangeSpeedSecondsPassed > Math.Max(1, 3 - Math.Abs(deltaPassedMeters)))
                {
                    switch (deltaPassedMeters)
                    {
                        case > -1:
                            ProcessAccelerateDecelerateByProbabilities(5, 5);
                            break;
                        case > -2:
                            ProcessAccelerateDecelerateByProbabilities(10, 3);
                            break;
                        case > -4:
                            ProcessAccelerateDecelerateByProbabilities(35, 2);
                            break;
                        case > -10:
                            ProcessAccelerateDecelerateByProbabilities(60, 1);
                            break;
                        default:
                            ProcessAccelerateDecelerateByProbabilities(80, 0);
                            break;
                    }
                }
                else if (deltaPassedMeters > 0 
                         && _lastChangeSpeedSecondsPassed > Math.Min(deltaPassedMeters, 2))
                {
                    switch (deltaPassedMeters)
                    {
                        case < 1:
                            ProcessAccelerateDecelerateByProbabilities(5, 5);
                            break;
                        case < 3:
                            ProcessAccelerateDecelerateByProbabilities(4, 7);
                            break;
                        case < 5:
                            ProcessAccelerateDecelerateByProbabilities(3, 10);
                            break;
                        case < 10:
                            ProcessAccelerateDecelerateByProbabilities(2, 20);
                            break;
                        case < 20:
                            ProcessAccelerateDecelerateByProbabilities(1, 40);
                            break;
                        default:
                            ProcessAccelerateDecelerateByProbabilities(1, 70);
                            break;
                    }
                }

                Debug.Log("deltaPassedMeters: " + deltaPassedMeters);
            }
        }

        private void ProcessAccelerateDecelerateByProbabilities(int accelerateProbabilityPercent, int decelerateProbabilityPercent)
        {
            if (_random.Next(100) < accelerateProbabilityPercent)
            {
                Accelerate();
            }
            else if (_random.Next(100) < decelerateProbabilityPercent)
            {
                Decelerate();
            }
        }

        private void Accelerate()
        {
            _lastChangeSpeedSecondsPassed = 0;
            
            _carModel.Accelerate(0);
        }

        private void Decelerate()
        {
            _lastChangeSpeedSecondsPassed = 0;
            
            _carModel.Decelerate();
        }

        private void UpdateCarView(float deltaTime)
        {
            var deltaWheelRotation = deltaTime * _carModel.CurrentSpeedMetersPerSecond * _carView.WheelRotationMultiplier;
            
            _carView.SetBodyRotation(_carModel.CurrentBodyRotation);
            _carView.RotateWheels(deltaWheelRotation);
            
            _carView.SetXOffset(_carModel.PassedMeters - _playerCarModel.PassedMeters);
        }
    }
}