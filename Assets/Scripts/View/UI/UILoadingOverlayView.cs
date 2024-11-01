using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI
{
    public class UILoadingOverlayView : MonoBehaviour
    {
        private const float OverlayFadeDuration = 0.2f; 
            
        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _image;

        private UniTaskCompletionSource _fadeInTcs;
        private UniTaskCompletionSource _fadeOutTcs;
        
        public UniTask ShowLoadingOverlay()
        {
            if (_fadeInTcs == null)
            {
                CancelCurrentTweens();
                
                _fadeInTcs = new UniTaskCompletionSource();

                _canvasGroup.gameObject.SetActive(true);
                _canvasGroup.LeanAlpha(1, OverlayFadeDuration)
                    .setOnComplete(OnShowLoadingOverlayComplete);
            }

            return _fadeInTcs.Task;
        }
        
        public UniTask HideLoadingOverlay()
        {
            if (_fadeOutTcs == null)
            {
                CancelCurrentTweens();
        
                _fadeOutTcs = new UniTaskCompletionSource();

                _canvasGroup.LeanAlpha(0, OverlayFadeDuration)
                    .setOnComplete(OnHideLoadingOverlayComplete);
            }

            return _fadeOutTcs.Task;
        }

        private void OnShowLoadingOverlayComplete()
        {
            _fadeInTcs?.TrySetResult();
            _fadeInTcs = null;
        }

        private void OnHideLoadingOverlayComplete()
        {
            _canvasGroup.gameObject.SetActive(false);
            
            _fadeOutTcs?.TrySetResult();
            _fadeOutTcs = null;
        }

        private void CancelCurrentTweens()
        {
            LeanTween.cancel(_canvasGroup.gameObject, callOnComplete: true);
        }
    }
}