using Cysharp.Threading.Tasks;
using Data;
using Holders;
using Infra.Instance;
using Model.RaceScene;
using UnityEngine;
using View.UI.RaceScene;

namespace Controller.RaceScene
{
    public class RaceFinishOverlayViewController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        
        private readonly Transform _targetTransform;
        private RaceModel _raceModel;
        private RaceResultsModel _raceResultsModel;
        private UIRaceFinishOverlayView _finishOverlayView;

        public RaceFinishOverlayViewController(Transform targetTransform)
        {
            _targetTransform = targetTransform;
        }

        public override void Initialize()
        {
            _raceModel = _modelsHolder.GetRaceModel();
            _raceResultsModel = _raceModel.RaceResultsModel;
            
            InitView().Forget();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
        }

        private async UniTaskVoid InitView()
        {
            _finishOverlayView = Instantiate<UIRaceFinishOverlayView>(PrefabKey.UIFinishOverlay, _targetTransform);
            
            await _finishOverlayView.AnimateShow(_raceResultsModel.IsFirst);

            Subscribe();
        }

        private void Subscribe()
        {
            _finishOverlayView.ContinueButtonView.ButtonClicked += OnContinueClicked;
            _finishOverlayView.DoubleRewardsButtonView.ButtonClicked += OnDoubleRewardsClicked;
        }

        private void Unsubscribe()
        {
            _finishOverlayView.ContinueButtonView.ButtonClicked -= OnContinueClicked;
            _finishOverlayView.DoubleRewardsButtonView.ButtonClicked -= OnDoubleRewardsClicked;
        }

        private void OnContinueClicked()
        {
            
        }

        private void OnDoubleRewardsClicked()
        {
            
        }
    }
}