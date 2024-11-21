using Extensions;
using Holders;
using Infra.Instance;
using Model.RaceScene;
using UnityEngine;

namespace Controller.RaceScene
{
    public class RaceFinishController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();

        private readonly Transform _finishLineTransform;
        private readonly float _playerCarContainerXPosition;
        
        private RaceModel _raceModel;
        private CarModel _playerCarModel;

        public RaceFinishController(Transform finishLineTransform, Transform playerCarTargetTransform)
        {
            _finishLineTransform = finishLineTransform;
            
            _playerCarContainerXPosition = playerCarTargetTransform.position.x;
        }

        public override void Initialize()
        {
            _raceModel = _modelsHolder.GetRaceModel();
            _playerCarModel = _raceModel.PlayerCar;
            
            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _updatesProvider.GameplayUpdate += OnGameplayUpdate;
        }

        private void Unsubscribe()
        {
            _updatesProvider.GameplayUpdate -= OnGameplayUpdate;
        }
        
        private void OnGameplayUpdate()
        {
            var distanceToFinish = _raceModel.DistanceMeters - _playerCarModel.PassedMeters;
            _finishLineTransform.SetXPosition(_playerCarContainerXPosition + distanceToFinish);

            if (distanceToFinish < 10)
            {
                Time.timeScale = 0.1f;
            }
        }
    }
}