using Infra.Instance;
using Model.RaceScene;
using Providers;
using UnityEngine;
using View;
using View.Presenters;
using Random = System.Random;

namespace Controller.RaceScene
{
    public class RaceSceneBotCarController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();

        private readonly CarModel _carModel;
        private readonly Random _random;
        private readonly RaceCarPresenter _carPresenter;

        private RaceModel _raceModel;
        private CarView _carView;
        private CarModel _playerCarModel;

        public RaceSceneBotCarController(CarModel carModel, Transform targetTransform)
        {
            _carModel = carModel;
            _random = new Random();
            _carPresenter = new RaceCarPresenter(carModel, targetTransform, muteSounds: true);
        }

        public override void Initialize()
        {
            _raceModel = _modelsHolder.GetRaceModel();
            _playerCarModel = _raceModel.PlayerCar;
            
            _carPresenter.Present();
            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
            _carPresenter.Dispose();
        }

        private void Subscribe()
        {
            _updatesProvider.GameplayHalfSecondPassed += OnGameplayHalfSecondPassed;
        }

        private void Unsubscribe()
        {
            _updatesProvider.GameplayHalfSecondPassed -= OnGameplayHalfSecondPassed;
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
    }
}