using UnityEngine;
using View.Extensions;

namespace View.UI.RaceScene
{
    public class UINetRaceFinishOverlayView : UIRaceFinishOverlayViewBase
    {
        [SerializeField] private CanvasGroup _itemCaptionsCanvasGroup;
        [SerializeField] private UINetRaceFinishOverlayResultItemView[] _raceResultItems;
        
        public UINetRaceFinishOverlayResultItemView[] RaceResultItems => _raceResultItems;

        protected override void Awake()
        {
            base.Awake();

            _itemCaptionsCanvasGroup.alpha = 0;
            foreach (var resultItemView in _raceResultItems)
            {
                resultItemView.CanvasGroup.alpha = 0;
                resultItemView.SetVisible(false);
            }
        }

        protected override void AnimateShowResultsPhase()
        {
            const float showAnimationDurationSec = 0.3f;
            
            _itemCaptionsCanvasGroup.AnimateAlpha(1, showAnimationDurationSec, out var lastLTDescr);
            for (var i = 0; i < _raceResultItems.Length; i++)
            {
                var raceResultItemView = _raceResultItems[i];
                raceResultItemView.CanvasGroup.AnimateAlpha(1, showAnimationDurationSec, out lastLTDescr);
                lastLTDescr.setDelay((i + 1) * 0.1f);
            }

            lastLTDescr?.setOnComplete(AnimateShowRewardsPhase);
        }
    }
}