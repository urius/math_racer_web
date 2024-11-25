using Extensions;
using Holders;
using Infra.Instance;
using Model.RaceScene;
using UnityEngine;
using View.UI.RaceScene;

namespace Controller.RaceScene
{
    public class RaceFinishController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();

        private readonly Transform _finishLineTransform;
        private readonly UIRaceSceneRootCanvasView _rootCanvasView;
        private readonly float _playerCarContainerXPosition;
        
        private RaceModel _raceModel;

        public RaceFinishController(
            Transform finishLineTransform,
            Transform playerCarTargetTransform,
            UIRaceSceneRootCanvasView rootCanvasView)
        {
            _finishLineTransform = finishLineTransform;
            _rootCanvasView = rootCanvasView;

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
            _raceModel.IsFinishedFlagChanged += OnIsFinishedFlagChanged;
        }

        private void Unsubscribe()
        {
            _updatesProvider.GameplayUpdate -= OnGameplayUpdate;
            
            _raceModel.IsFinishingFlagChanged -= OnIsFinishingFlagChanged;            
            _raceModel.IsFinishedFlagChanged -= OnIsFinishedFlagChanged;

        }

        private void OnIsFinishedFlagChanged(bool isFinished)
        {
            if (isFinished)
            {
                InitChildController(new RaceFinishOverlayViewController(_rootCanvasView.transform));
            }
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