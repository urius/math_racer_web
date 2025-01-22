using Data;
using UnityEngine;
using View.UI.RaceScene;

namespace Controller.RaceScene
{
    public class NetRaceFinishOverlayViewController : RaceFinishOverlayViewControllerBase
    {
        private UINetRaceFinishOverlayView _finishOverlayView;
        
        public NetRaceFinishOverlayViewController(Transform targetTransform) : base(targetTransform)
        {
        }

        protected override UIRaceFinishOverlayViewBase InstantiateView()
        {
            _finishOverlayView = Instantiate<UINetRaceFinishOverlayView>(PrefabKey.UINetFinishOverlay, TargetTransform);

            return _finishOverlayView;
        }

        protected override void SetupView()
        {
            base.SetupView();
            
        }
    }
}