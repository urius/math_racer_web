using Extensions;
using Infra.Instance;
using Model.RaceScene;
using Providers;
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
        }

        private void Unsubscribe()
        {
            _updatesProvider.GameplayUpdate -= OnGameplayUpdate;
            _updatesProvider.GameplayHalfSecondPassed -= OnGameplayHalfSecondPassed;
        }

        private void OnGameplayUpdate()
        {
            UpdateCarView();
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
                var deltaTargetSpeed = _carModel.TargetSpeedKmph - _playerCarModel.TargetSpeedKmph;
                var actionProbabilityVar = (int)(deltaTargetSpeed + 2 * deltaPassedMeters);

                var accelerationProbability = actionProbabilityVar < 0 ? -actionProbabilityVar : 0;
                var decelerationProbability = actionProbabilityVar > 0 ? actionProbabilityVar : 0;
                
                ProcessAccelerateDecelerateByProbabilities(accelerationProbability, decelerationProbability);

                // Debug.Log("accelerationProbability: " + accelerationProbability + " decelerationProbability: " + decelerationProbability);
                // Debug.Log("deltaPassedMeters: " + deltaPassedMeters + " delta speed: " + deltaTargetSpeed);
                // Debug.Log("");
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
            _carModel.Accelerate();
        }

        private void Decelerate()
        {
            _carModel.Decelerate();
        }

        private void UpdateCarView()
        {
            var deltaWheelRotation = _carModel.CurrentUpdateMetersPassed * _carView.WheelRotationMultiplier;
            
            _carView.SetBodyRotation(_carModel.CurrentBodyRotation);
            _carView.RotateWheels(deltaWheelRotation);
            
            _carView.SetXOffset(_carModel.PassedMeters - _playerCarModel.PassedMeters);
        }
    }
}