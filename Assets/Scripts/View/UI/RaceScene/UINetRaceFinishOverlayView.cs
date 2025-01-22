using UnityEngine;
using View.Extensions;

namespace View.UI.RaceScene
{
    public class UINetRaceFinishOverlayView : UIRaceFinishOverlayViewBase
    {
        [SerializeField] private UINetRaceFinishOverlayResultItemView[] _raceResultItems;
        
        public UINetRaceFinishOverlayResultItemView[] RaceResultItems => _raceResultItems;

        protected override void Awake()
        {
            base.Awake();

            foreach (var resultItemView in _raceResultItems)
            {
                resultItemView.CanvasGroup.alpha = 0;
            }
        }

        protected override void AnimateShowResultsPhase()
        {
            LTDescr lastLTDescr = null;
            for (var i = 0; i < _raceResultItems.Length; i++)
            {
                var raceResultItemView = _raceResultItems[i];
                raceResultItemView.CanvasGroup.AnimateAlpha(1, 0.3f, out lastLTDescr);
                lastLTDescr.setDelay(i * 0.1f);
            }

            lastLTDescr?.setOnComplete(AnimateShowRewardsPhase);
        }
    }
}