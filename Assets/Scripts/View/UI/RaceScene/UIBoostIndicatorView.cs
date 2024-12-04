using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using UnityEngine.UI;
using View.Extensions;

namespace View.UI.RaceScene
{
    public class UIBoostIndicatorView : MonoBehaviour
    {
        [SerializeField] private Image _indicatorLamp;
        [SerializeField] private Color _greenColor;
        [SerializeField] private Color _redColor;
        [SerializeField] private Color _yellowColor;
        
        private float _defaultLampAlpha;

        private void Awake()
        {
            _defaultLampAlpha = _indicatorLamp.color.a;
            
            SetAlpha(0);
        }

        public void ToGreenColor()
        {
            _indicatorLamp.color = _greenColor;
        }

        public void ToRedColor()
        {
            _indicatorLamp.color = _redColor;
        }

        public void ToYellowColor()
        {
            _indicatorLamp.color = _yellowColor;
        }

        public void SetAlpha(float alpha)
        {
            _indicatorLamp.SetAlpha(alpha);
        }

        public UniTask FadeIn()
        {
            CancelTweens();
            
            var tcs = new UniTaskCompletionSource();

            LeanTween.value(gameObject, _indicatorLamp.SetAlpha, 0, _defaultLampAlpha, Constants.TurboIndicatorShowHideDurationSec)
                .setOnComplete(() => tcs.TrySetResult());

            return tcs.Task;
        }
        
        public UniTask FadeOut(float delaySec = 0)
        {
            CancelTweens();
            
            var tcs = new UniTaskCompletionSource();

            LeanTween.value(gameObject, _indicatorLamp.SetAlpha, _indicatorLamp.color.a, 0, Constants.TurboIndicatorShowHideDurationSec)
                .setDelay(delaySec)
                .setOnComplete(() => tcs.TrySetResult());

            return tcs.Task;
        }

        private void CancelTweens()
        {
            LeanTween.cancel(gameObject);
        }
    }
}