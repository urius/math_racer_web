using Data;
using Extensions;
using Holders;
using Infra.Instance;
using Model.RaceScene;
using UnityEngine;
using View;

namespace Controller.RaceScene
{
    public class RaceSceneCarController : ControllerBase
    {
        private const float BodyRotationMult = 50;

        private readonly IPrefabHolder _prefabHolder = Instance.Get<IPrefabHolder>();
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        
        private readonly CarModel _carModel;
        private readonly Transform _targetTransform;

        private float _targetSpeed;
        private CarView _carView;
        private RaceModel _raceModel;

        public RaceSceneCarController(CarModel carModel, Transform targetTransform)
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
            _updatesProvider.GameplayFixedUpdate += OnGameplayFixedUpdate;
            
            _raceModel.QuestionsModel.RightAnswerGiven += OnRightAnswerGiven;
            _raceModel.QuestionsModel.WrongAnswerGiven += OnWrongAnswerGiven;
        }

        private void Unsubscribe()
        {
            _updatesProvider.GameplayFixedUpdate -= OnGameplayFixedUpdate;
            
            _raceModel.QuestionsModel.RightAnswerGiven -= OnRightAnswerGiven;
            _raceModel.QuestionsModel.WrongAnswerGiven -= OnWrongAnswerGiven;
        }

        private void OnRightAnswerGiven()
        {
            _carModel.Accelerate();
        }

        private void OnWrongAnswerGiven()
        {
            _carModel.Decelerate();
        }

        private void OnGameplayFixedUpdate()
        {
            _carModel.Update();
            
            MoveCar();
        }
        
        private void MoveCar()
        {
            var speed = _carModel.CurrentSpeedKmph;
            var distancePassed = Constants.KmphToMetersPerFrame * speed;
            
            var deltaRotation = distancePassed * _carView.WheelRotationMultiplier;
            
            var deltaTargetSpeed = _carModel.TargetSpeedKmph - speed;
            var bodyRotation = -deltaTargetSpeed * 0.5f;
            _carView.SetBodyRotation(bodyRotation);
            
            _carView.RotateWheels(deltaRotation);
            
            Debug.Log("Delta target speed: " + deltaTargetSpeed);
        }
    }
}