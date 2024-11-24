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

        public RaceFinishController(Transform finishLineTransform, Transform playerCarTargetTransform)
        {
            _finishLineTransform = finishLineTransform;
            
            _playerCarContainerXPosition = playerCarTargetTransform.position.x;
        }

        public override void Initialize()
        {
            _raceModel = _modelsHolder.GetRaceModel();
            
            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _updatesProvider.GameplayUpdate += OnGameplayUpdate;

            _raceModel.IsFinishingFlagChanged += OnIsFinishingFlagChanged;
        }

        private void Unsubscribe()
        {
            _updatesProvider.GameplayUpdate -= OnGameplayUpdate;
            
            _raceModel.IsFinishingFlagChanged -= OnIsFinishingFlagChanged;
        }

        private void OnGameplayUpdate()
        {
            _finishLineTransform.SetXPosition(_playerCarContainerXPosition + _raceModel.PlayerCarDistanceToFinish);

            if (_raceModel.PlayerCarDistanceToFinish < -3)
            {
                Time.timeScale = 1f;
            }
        }

        private void OnIsFinishingFlagChanged(bool isFinishing)
        {
            if (isFinishing)
            {
                Time.timeScale = 0.1f;
            }
        }
    }
}