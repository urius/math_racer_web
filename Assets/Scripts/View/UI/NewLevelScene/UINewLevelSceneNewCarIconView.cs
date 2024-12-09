using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using View.Extensions;

namespace View.UI.NewLevelScene
{
    public class UINewLevelSceneNewCarIconView : MonoBehaviour
    {
        [SerializeField] private Image _image;

        private RectTransform _rectTransform;

        public RectTransform RectTransform => GetRectTransform();

        public void SetSprite(Sprite sprite)
        {
            _image.sprite = sprite;
        }

        public UniTask AnimateFadeIn(float delay = 0f)
        {
            _image.SetAlpha(0);
            var animationTask = _image.AnimateAlpha(1, 0.5f, out var ltDescr);
            ltDescr.setDelay(delay);

            return animationTask;
        }

        public void SetAlpha(float alpha)
        {
            _image.SetAlpha(alpha);
        }

        private RectTransform GetRectTransform()
        {
            _rectTransform ??= transform as RectTransform;
            return _rectTransform;
        }
    }
}