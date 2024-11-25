using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using View.Extensions;
using View.UI.Common;

namespace View.UI.RaceScene
{
    public class UIRaceFinishOverlayView : MonoBehaviour
    {
        [SerializeField] private Image _bgImage;
        
        [SerializeField] private TMP_Text _finishText;
        [SerializeField] private Image _finishImageLeft;
        [SerializeField] private Image _finishImageRight;
        
        [SerializeField] private TMP_Text _placeText;
        [SerializeField] private TMP_Text _statsKeysText;
        [SerializeField] private TMP_Text _statsValuesText;
        [SerializeField] private TMP_Text _rewardsText;
        
        [SerializeField] private UITextButtonView _doubleRewardsButtonView;
        [SerializeField] private UITextButtonView _continueButtonView;

        public UITextButtonView DoubleRewardsButtonView => _doubleRewardsButtonView;
        public UITextButtonView ContinueButtonView => _continueButtonView;

        private ShowAnimationContext _showAnimationContext;
        private float _bgTargetAlpha;
        private Vector2 _finishTextTargetPos;
        private Vector2 _finishImageLeftTargetPos;
        private Vector2 _finishImageRightTargetPos;
        private Vector2 _doubleRewardsButtonTargetPos;
        private Vector2 _continueButtonTargetPos;

        private void Awake()
        {
            _bgTargetAlpha = _bgImage.color.a;
            _finishTextTargetPos = _finishText.rectTransform.anchoredPosition;
            _finishImageLeftTargetPos = _finishImageLeft.rectTransform.anchoredPosition;
            _finishImageRightTargetPos = _finishImageRight.rectTransform.anchoredPosition;
            _doubleRewardsButtonTargetPos = _doubleRewardsButtonView.RectTransform.anchoredPosition;
            _continueButtonTargetPos = _continueButtonView.RectTransform.anchoredPosition;
                
            _bgImage.SetAlpha(0);
            
            _finishText.SetAlpha(0);
            _finishImageLeft.SetAlpha(0);
            _finishImageRight.SetAlpha(0);
            
            _placeText.SetAlpha(0);
            _statsKeysText.SetAlpha(0);
            _statsValuesText.SetAlpha(0);
            _rewardsText.SetAlpha(0);
            
            _doubleRewardsButtonView.SetVisibility(false);
            _continueButtonView.SetVisibility(false);
        }

        public void SetFinishText(string text)
        {
            _finishText.text = text;
        }

        public void SetPlaceText(string text)
        {
            _placeText.text = text;
        }

        public void SetStatsKeysText(string text)
        {
            _statsKeysText.text = text;
        }

        public void SetStatsValuesText(string text)
        {
            _statsValuesText.text = text;
        }

        public void SetRewardTexts(string cashRewardText, string goldRewardText = null)
        {
            _rewardsText.text = cashRewardText + " " + goldRewardText;
        }

        public UniTask AnimateShow(bool showFirstPlaceText)
        {
            _showAnimationContext = new ShowAnimationContext(showFirstPlaceText, new UniTaskCompletionSource());
            
            LeanTween
                .value(_bgImage.gameObject, a => _bgImage.SetAlpha(a), 0, _bgTargetAlpha, 0.5f)
                .setOnComplete(AnimateShowFinishTextPhase);

            return _showAnimationContext.ShowTask;
        }

        private void AnimateShowFinishTextPhase()
        {
            _finishText.rectTransform.anchoredPosition = new Vector2(0, 100);

            _finishImageLeft.rectTransform.anchoredPosition = new Vector2(-400, _finishImageLeftTargetPos.y);
            _finishImageRight.rectTransform.anchoredPosition = new Vector2(400, _finishImageRightTargetPos.y);

            _finishText.rectTransform.LeanMove((Vector3)_finishTextTargetPos, 0.6f)
                .setOnUpdate(_finishText.SetAlpha)
                .setEaseOutQuad();
            
            const float imagesShowDuration = 0.8f;
            _finishImageLeft.rectTransform.LeanMove((Vector3)_finishImageLeftTargetPos, imagesShowDuration)
                .setOnUpdate(_finishImageLeft.SetAlpha)
                .setEaseOutBack();
            _finishImageRight.rectTransform.LeanMove((Vector3)_finishImageRightTargetPos, imagesShowDuration)
                .setOnUpdate(_finishImageRight.SetAlpha)
                .setEaseOutBack()
                .setOnComplete(AnimateShowPlaceTextPhase);
        }

        private void AnimateShowPlaceTextPhase()
        {
            var placeTextGo = _placeText.gameObject;
            placeTextGo.SetActive(_showAnimationContext.NeedShowFirstPlaceText);

            AnimateTextAlpha(_placeText, 1, 0.4f)
                .setOnComplete(AnimateShowStatsPhase);
        }

        private void AnimateShowStatsPhase()
        {
            const float statsTextAppearTime = 0.4f;
            var delay = _showAnimationContext.NeedShowFirstPlaceText ? 0.5f : 0f;
            
            AnimateTextAlpha(_statsKeysText, 1, statsTextAppearTime)
                .setDelay(delay);
            AnimateTextAlpha(_statsValuesText, 1, statsTextAppearTime)
                .setDelay(delay)
                .setOnComplete(AnimateShowRewardsPhase);
        }

        private void AnimateShowRewardsPhase()
        {
            AnimateTextAlpha(_rewardsText, 1, 0.4f)
                .setDelay(0.3f)
                .setOnComplete(AnimateShowButtonsPhase);
        }

        private void AnimateShowButtonsPhase()
        {
            _doubleRewardsButtonView.SetAnchoredPosition(new Vector2(_doubleRewardsButtonTargetPos.x, -100));
            _continueButtonView.SetAnchoredPosition(new Vector2(_continueButtonTargetPos.x, -100));
            
            _doubleRewardsButtonView.SetVisibility(true);
            _continueButtonView.SetVisibility(true);

            const float delay = 0.7f;

            _doubleRewardsButtonView.RectTransform
                .LeanMove((Vector3)_doubleRewardsButtonTargetPos, 1f)
                .setDelay(delay)
                .setEaseOutBack();

            _continueButtonView.RectTransform
                .LeanMove((Vector3)_continueButtonTargetPos, 1f)
                .setDelay(delay + 0.5f)
                .setEaseOutBack()
                .setOnComplete(_showAnimationContext.SetCompleted);
        }

        private LTDescr AnimateTextAlpha(TMP_Text tmpText, float to, float time)
        {
            return LeanTween.value(tmpText.gameObject, tmpText.SetAlpha, tmpText.color.a, to, time);
        }

        private class ShowAnimationContext
        {
            public readonly bool NeedShowFirstPlaceText;
            
            private readonly UniTaskCompletionSource _tcs;

            public ShowAnimationContext(bool needShowFirstPlaceText, UniTaskCompletionSource tcs)
            {
                NeedShowFirstPlaceText = needShowFirstPlaceText;
                _tcs = tcs;
            }

            public UniTask ShowTask => _tcs.Task;

            public void SetCompleted()
            {
                _tcs.TrySetResult();
            }
        }
    }
}