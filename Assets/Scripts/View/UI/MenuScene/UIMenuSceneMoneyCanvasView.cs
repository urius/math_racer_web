using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using View.Extensions;

namespace View.UI.MenuScene
{
    public class UIMenuSceneMoneyCanvasView : MonoBehaviour
    {
        public event Action OpenBankButtonClicked;
        
        private const float AnimationDurationSec = 0.4f;
        private const float BlinkDurationSec = 0.8f;
            
        [SerializeField] private TMP_Text _cashText;
        [SerializeField] private TMP_Text _goldText;
        [SerializeField] private Button[] _openBankButtons;
        [SerializeField] private CanvasGroup[] _bankButtonsCanvasGroups;
        
        private int _currentCashAmount;
        private int _currentGoldAmount;
        private Color _defaultGoldTextColor;
        private Vector2 _defaultGoldTextPosition;
        private Color _defaultCashTextColor;
        private Vector2 _defaultCashTextPosition;

        private void Awake()
        {
            _defaultGoldTextColor = _goldText.color;
            _defaultGoldTextPosition = _goldText.rectTransform.anchoredPosition;
            _defaultCashTextColor = _cashText.color;
            _defaultCashTextPosition = _cashText.rectTransform.anchoredPosition;

            foreach (var openBankButton in _openBankButtons)
            {
                openBankButton.onClick.AddListener(OnOpenBankClicked);
            }
        }

        private void OnDestroy()
        {
            foreach (var openBankButton in _openBankButtons)
            {
                openBankButton.onClick.RemoveAllListeners();
            }
        }

        public void SetCashAmount(int cashAmount)
        {
            _currentCashAmount = cashAmount;
            _cashText.SetText(cashAmount.ToPriceView2());
        }

        public void SetGoldAmount(int goldAmount)
        {
            _currentGoldAmount = goldAmount;
            _goldText.SetText(goldAmount.ToGoldView2());
        }

        public UniTask AnimateCashAmount(int targetCashAmount)
        {
            CancelTweenOn(_cashText);

            var tcs = new UniTaskCompletionSource();

            LeanTween
                .value(_cashText.gameObject, OnAnimateCashTweenUpdate, _currentCashAmount, targetCashAmount,
                    AnimationDurationSec)
                .setOnComplete(() => tcs.TrySetResult());

            return tcs.Task;
        }

        public UniTask AnimateGoldAmount(int targetGoldAmount)
        {
            CancelTweenOn(_goldText);

            var tcs = new UniTaskCompletionSource();

            LeanTween
                .value(_goldText.gameObject, OnAnimateGoldTweenUpdate, _currentGoldAmount, targetGoldAmount,
                    AnimationDurationSec)
                .setOnComplete(() => tcs.TrySetResult());

            return tcs.Task;
        }

        public void AnimateGoldRedBlink()
        {
            CancelTweenOn(_goldText);

            LeanTween
                .value(_goldText.gameObject, OnAnimateGoldRedBlinkTweenUpdate,0, 10 * BlinkDurationSec, BlinkDurationSec)
                .setOnComplete(() =>
                {
                    _goldText.color = _defaultGoldTextColor;
                    _goldText.rectTransform.anchoredPosition = _defaultGoldTextPosition;
                });
        }

        public void AnimateCashRedBlink()
        {
            CancelTweenOn(_cashText);

            LeanTween
                .value(_cashText.gameObject, OnAnimateCashRedBlinkTweenUpdate,0, 10 * BlinkDurationSec, BlinkDurationSec)
                .setOnComplete(() =>
                {
                    _cashText.color = _defaultCashTextColor;
                    _cashText.rectTransform.anchoredPosition = _defaultCashTextPosition;
                });
        }

        public void ShowAddGoldButtons()
        {
            foreach (var bankButtonsCanvasGroup in _bankButtonsCanvasGroups)
            {
                bankButtonsCanvasGroup.LeanAlpha(1, 0.3f).setEaseOutQuad();
            }
        }

        public void HideAddGoldButtons()
        {
            foreach (var bankButtonsCanvasGroup in _bankButtonsCanvasGroups)
            {
                bankButtonsCanvasGroup.LeanAlpha(0, 0.3f).setEaseOutQuad();
            }
        }

        private void OnAnimateGoldRedBlinkTweenUpdate(float progress)
        {
            var color = Color.Lerp(_defaultGoldTextColor, Color.red, Mathf.PingPong(progress, 1f));
            _goldText.color = color;

            var xPositionOffset = GetXPositionPingPongOffset(progress);
            _goldText.rectTransform.anchoredPosition =
                new Vector2(_defaultGoldTextPosition.x + xPositionOffset, _defaultGoldTextPosition.y);
        }

        private void OnAnimateCashRedBlinkTweenUpdate(float progress)
        {
            var color = Color.Lerp(_defaultCashTextColor, Color.red, Mathf.PingPong(progress, 1f));
            _cashText.color = color;

            var xPositionOffset = GetXPositionPingPongOffset(progress);
            _cashText.rectTransform.anchoredPosition =
                new Vector2(_defaultCashTextPosition.x + xPositionOffset, _defaultCashTextPosition.y);
        }

        private static float GetXPositionPingPongOffset(float progress)
        {
            return Mathf.PingPong(progress * 8.8f, 10) - 5;
        }

        private void OnAnimateCashTweenUpdate(float value)
        {
            SetCashAmount((int)value);
        }

        private void OnAnimateGoldTweenUpdate(float value)
        {
            SetGoldAmount((int)value);
        }

        private void CancelTweenOn(Component tmpText)
        {
            LeanTween.cancel(tmpText.gameObject, callOnComplete: true);
        }

        private void OnOpenBankClicked()
        {
            OpenBankButtonClicked?.Invoke();
        }
    }
}