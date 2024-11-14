using Extensions;
using Holders;
using Infra.Instance;
using Model.RaceScene;
using UnityEngine;
using View;

namespace Controller.RaceScene
{
    public class RaceScenePlayerCarController : ControllerBase
    {
        private readonly IPrefabHolder _prefabHolder = Instance.Get<IPrefabHolder>();
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
            
            _carView = _prefabHolder.Instantiate<CarView>(_carModel.CarKey.ToPrefabKey(), _targetTransform);

            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _updatesProvider.GameplayUpdate += OnGameplayUpdate;
            
            _raceModel.QuestionsModel.AnswerGiven += OnAnswerGiven;
        }

        private void Unsubscribe()
        {
            _updatesProvider.GameplayUpdate -= OnGameplayUpdate;
            
            _raceModel.QuestionsModel.AnswerGiven -= OnAnswerGiven;
        }

        private void OnAnswerGiven(bool isRightAnswer)
        {
            if (isRightAnswer)
            {
                _carModel.Accelerate(_raceModel.QuestionsModel.TurboLevel);
            }
            else
            {
                _carModel.Decelerate();
            }
        }

        private void OnGameplayUpdate()
        {
            //Time.timeScale = 0.2f;
            
            _carModel.Update(Time.deltaTime);
            
            MoveCar(Time.deltaTime);
        }
        
        private void MoveCar(float deltaTime)
        {
            var deltaWheelRotation = deltaTime * _carModel.CurrentSpeedMetersPerSecond * _carView.WheelRotationMultiplier;
            
            _carView.SetBodyRotation(_carModel.CurrentBodyRotation);
            _carView.SetXOffset(_carModel.XOffset);
            _carView.RotateWheels(deltaWheelRotation);
        }
    }
}