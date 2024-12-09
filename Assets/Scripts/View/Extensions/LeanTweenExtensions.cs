using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

namespace View.Extensions
{
    public static class LeanTweenExtensions
    {
        public static UniTask AnimateAlpha(this TMP_Text tmpText, float to, float time, out LTDescr ltDescr)
        {
            ltDescr = LeanTween
                .value(tmpText.gameObject, tmpText.SetTextAlpha, tmpText.color.a, to, time);

            return ltDescr.ToUniTask();
        }
        
        public static UniTask AnimateAlpha(this TMP_Text tmpText, float to, float time)
        {
            return AnimateAlpha(tmpText, to, time, out _);
        }
        
        public static UniTask AnimateAlpha(this Image image, float to, float time, out LTDescr ltDescr)
        {
            ltDescr = LeanTween
                .value(image.gameObject, image.SetImageAlpha, image.color.a, to, time);

            return ltDescr.ToUniTask();
        }
        
        public static UniTask AnimateAlpha(this Image image, float to, float time)
        {
            return AnimateAlpha(image, to, time, out _);
        }

        public static UniTask ToUniTask(this LTDescr ltDescr)
        {
            var tcs = new UniTaskCompletionSource();
            ltDescr.setOnComplete(() => tcs.TrySetResult());

            return tcs.Task;
        }

        private static void SetImageAlpha(this Image image, float alpha)
        {
            var color = image.color;
            color.a = alpha;
            image.color = color;
        }

        private static void SetTextAlpha(this TMP_Text text, float alpha)
        {
            var textColor = text.color;
            textColor.a = alpha;
            text.color = textColor;
        }
    }
}