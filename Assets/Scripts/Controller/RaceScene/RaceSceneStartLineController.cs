using Extensions;
using Infra.Instance;
using Model.RaceScene;
using Providers;
using UnityEngine;
using View.Gameplay.Race;

namespace Controller.RaceScene
{
    public class RaceSceneStartLineController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();

        private readonly Transform _startLineTransform;
        private readonly TrafficLightView _trafficLightView;
        
        private QuestionsModel _questionsModel;
        private int _currentLightIndex = 0;
        private CarModel _playerCarModel;

        public RaceSceneStartLineController(Transform startLineTransform, TrafficLightView trafficLightView)
        {
            _startLineTransform = startLineTransform;
            _trafficLightView = trafficLightView;
        }

        public override void Initialize()
        {
            var raceModel = _modelsHolder.GetRaceModel();
            _questionsModel = raceModel.QuestionsModel;
            _playerCarModel = raceModel.PlayerCar;

            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _questionsModel.AnswerGiven += OnAnswerGiven;
            _updatesProvider.GameplayUpdate += OnGameplayUpdate;
        }

        private void Unsubscribe()
        {
            _questionsModel.AnswerGiven -= OnAnswerGiven;
            _updatesProvider.GameplayUpdate -= OnGameplayUpdate;
            
            _startLineTransform.gameObject.SetActive(false);
        }

        private void OnGameplayUpdate()
        {
            var distancePassed = _playerCarModel.CurrentUpdateMetersPassed;

            _startLineTransform.MoveX(-distancePassed);

            if (_startLineTransform.transform.position.x < -20)
            {
                RequestDispose();
            }
        }

        private void OnAnswerGiven(int _, bool isRight)
        {
            if (_questionsModel.QuestionsCount > 1) return;
            if (_currentLightIndex >= _trafficLightView.LightsCount) return;
            
            if (isRight == false)
            {
                _trafficLightView.SetLightRed(_currentLightIndex);
            }
            else
            {
                for (var i = _currentLightIndex; i < _trafficLightView.LightsCount; i++)
                {
                    _trafficLightView.SetLightGreen(i);
                }
            }

            _currentLightIndex++;
        }
    }
}