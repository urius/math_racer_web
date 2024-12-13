using Cysharp.Threading.Tasks;
using Extensions;
using Infra.Instance;
using Model.RaceScene;
using Providers;
using UnityEngine;
using View;

namespace Controller.RaceScene
{
    public class RaceScenePlayerCarController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        
        private readonly CarModel _carModel;
        private readonly Transform _targetTransform;

        private float _targetSpeed;
        private CarView _carView;
        private RaceModel _raceModel;

        public RaceScenePlayerCarController(CarModel carModel, Transform targetTransform)
        {
            _carModel = carModel;
            _targetTransform = targetTransform;
        }
        
        public override void Initialize()
        {
            _raceModel = _modelsHolder.GetRaceModel();
            
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
            
            _raceModel.QuestionsModel.AnswerGiven += OnAnswerGiven;
            _raceModel.QuestionsModel.TurboActivated += OnTurboActivated;
            _carModel.TurboFlag.ValueChanged += OnTurboFlagValueChanged;
        }

        private void Unsubscribe()
        {
            _updatesProvider.GameplayUpdate -= OnGameplayUpdate;
            
            _raceModel.QuestionsModel.AnswerGiven -= OnAnswerGiven;
            _raceModel.QuestionsModel.TurboActivated -= OnTurboActivated;
            _carModel.TurboFlag.ValueChanged -= OnTurboFlagValueChanged;
        }

        private void OnTurboFlagValueChanged(bool _, bool value)
        {
            if (value)
            {
                _carView.ShowBoostVFX();
            }
            else
            {
                UniTask.Delay(500)
                    .ContinueWith(_carView.StopBoostVFX);
            }
        }

        private void OnTurboActivated()
        {
            _carModel.AccelerateTurbo();
        }

        private void OnAnswerGiven(int answerIndex, bool isRightAnswer)
        {
            if (isRightAnswer)
            {
                _carModel.Accelerate();
            }
            else
            {
                _carModel.Decelerate();
            }
        }

        private void OnGameplayUpdate()
        {
            UpdateCarView();
        }
        
        private void UpdateCarView()
        {
            var deltaWheelRotation = _carModel.CurrentUpdateMetersPassed * _carView.WheelRotationMultiplier;
            
            _carView.SetBodyRotation(_carModel.CurrentBodyRotation);
            _carView.SetXOffset(_carModel.XOffset);
            _carView.RotateWheels(deltaWheelRotation);
        }
    }
}