using TMPro;
using UnityEngine;
using View.Extensions;

namespace View.UI.RaceScene
{
    public class UIRaceFinishOverlayView : UIRaceFinishOverlayViewBase
    {
        [SerializeField] private TMP_Text _statsKeysText;
        [SerializeField] private TMP_Text _statsValuesText;

        protected override void Awake()
        {
            base.Awake();
            
            _statsKeysText.SetAlpha(0);
            _statsValuesText.SetAlpha(0);
        }

        public void SetStatsKeysText(string text)
        {
            _statsKeysText.text = text;
        }

        public void SetStatsValuesText(string text)
        {
            _statsValuesText.text = text;
        }

        protected override void AnimateShowResultsPhase()
        {
            const float statsTextAppearTime = 0.4f;
            var delay = _showAnimationContext.NeedShowFirstPlaceText ? 0.5f : 0f;
            
            AnimateTextAlpha(_statsKeysText, 1, statsTextAppearTime)
                .setDelay(delay);
            AnimateTextAlpha(_statsValuesText, 1, statsTextAppearTime)
                .setDelay(delay)
                .setOnComplete(AnimateShowRewardsPhase);
        }
    }
}