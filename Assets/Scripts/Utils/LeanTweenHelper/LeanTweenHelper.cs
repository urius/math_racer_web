using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Utils.LeanTweenHelper
{
    public static class LeanTweenHelper
    {
        public static LTDescr FlyImagePrefabTo(
            GameObject prefab, Transform parentTransform, 
            Vector3 startWorldPosition, Vector3 targetWorldPosition, float duration, 
            bool needAlphaFadeIn = false, bool needAlphaFadeOut = false, float delay = 0)
        {
            var targetImageGo = Object.Instantiate(prefab, startWorldPosition, Quaternion.identity, parentTransform);
            var targetImage = targetImageGo.GetComponent<Image>();
            if (needAlphaFadeIn)
            {
                targetImage.color = SetAlpha(targetImage.color, 0);
            }

            var ltDescription = LeanTween.move(targetImageGo, targetWorldPosition, duration)
                .setEaseOutSine()
                .setDelay(delay);

            if (needAlphaFadeIn)
            {
                if (needAlphaFadeOut)
                {
                    ltDescription
                        .setOnUpdate(OnFlyAnimationAlphaFadeInFadeOutUpdate);
                }
                else
                {
                    ltDescription
                        .setOnUpdate(OnFlyAnimationAlphaFadeInUpdate);
                }
                ltDescription.setOnUpdateParam(targetImage);
            }
            else if (needAlphaFadeOut)
            {
                ltDescription
                    .setOnUpdate(OnFlyAnimationAlphaFadeOutUpdate);
                ltDescription.setOnUpdateParam(targetImage);
            }

            ltDescription.destroyOnComplete = true;

            return ltDescription;
        }

        public static void BounceX(RectTransform rectTransform, float deltaX, float duration1 = 0.3f, float duration2 = 0.6f)
        {
            var startPos = rectTransform.anchoredPosition.x;
            LeanTween.moveX(rectTransform, rectTransform.anchoredPosition.x + deltaX, duration1)
                .setEaseOutQuad()
                .setOnComplete(() => rectTransform.LeanMoveX(startPos, duration2).setEaseOutBounce());
        }

        public static UniTask BounceXAsync(RectTransform rectTransform, float deltaX, float duration1 = 0.3f, float duration2 = 0.6f)
        {
            return BounceXAsync(rectTransform, deltaX, CancellationToken.None, duration1, duration2);
        }

        public async static UniTask BounceXAsync(RectTransform rectTransform, float deltaX, CancellationToken stopToken, float duration1 = 0.3f, float duration2 = 0.6f)
        {
            var tcs = new UniTaskCompletionSource();
            void CancelAnimation()
            {
                LeanTween.cancel(rectTransform.gameObject, true);
            }

            using (stopToken.Register(CancelAnimation))
            {
                var startPos = rectTransform.anchoredPosition.x;
                var (task, tweenDescription) = MoveXAsync(rectTransform, rectTransform.anchoredPosition.x + deltaX, duration1);
                tweenDescription.setEaseOutQuad();
                await task;
                if (false == stopToken.IsCancellationRequested)
                {
                    (task, tweenDescription) = MoveXAsync(rectTransform, startPos, duration2);
                    tweenDescription.setEaseOutBounce();
                    await task;
                }
                rectTransform.anchoredPosition = new Vector2(startPos, rectTransform.anchoredPosition.y);
            }
            tcs.TrySetResult();
        }

        public static UniTask BounceYAsync(RectTransform rectTransform, float deltaY, float duration1 = 0.3f, float duration2 = 0.6f)
        {
            return BounceYAsync(rectTransform, deltaY, CancellationToken.None, duration1, duration2);
        }

        public static async UniTask BounceYAsync(RectTransform rectTransform, float deltaY, CancellationToken stopToken, float duration1 = 0.3f, float duration2 = 0.6f, bool ignoreTimeScale = false)
        {
            var tcs = new UniTaskCompletionSource();
            void CancelAnimation()
            {
                LeanTween.cancel(rectTransform.gameObject, true);
            }

            using (stopToken.Register(CancelAnimation))
            {
                var startPos = rectTransform.anchoredPosition.y;
                var (task, tweenDescription) = MoveYAsync(rectTransform, rectTransform.anchoredPosition.y + deltaY, duration1);
                tweenDescription.setEaseOutQuad().setIgnoreTimeScale(ignoreTimeScale);
                await task;
                if (false == stopToken.IsCancellationRequested)
                {
                    (task, tweenDescription) = MoveYAsync(rectTransform, startPos, duration2);
                    tweenDescription.setEaseOutBounce().setIgnoreTimeScale(ignoreTimeScale);
                    await task;
                }
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, startPos);
            }
            tcs.TrySetResult();
        }

        public static (UniTask task, LTDescr tweenDescription) MoveBounceAsync(RectTransform rectTransform, Vector3 to, float duration)
        {
            var tcs = new UniTaskCompletionSource();
            var tweenDescription = LeanTween.move(rectTransform, to, duration)
                .setEaseOutBounce()
                .setOnComplete(() => tcs.TrySetResult());

            return (tcs.Task, tweenDescription);
        }

        public static (UniTask task, LTDescr tweenDescription) MoveXAsync(RectTransform rectTransform, float to, float duration)
        {
            var tcs = new UniTaskCompletionSource();
            var tweenDescription = LeanTween.moveX(rectTransform, to, duration)
                .setOnComplete(() => tcs.TrySetResult());

            return (tcs.Task, tweenDescription);
        }

        public static (UniTask task, LTDescr tweenDescription) MoveYAsync(RectTransform rectTransform, float to, float duration)
        {
            var tcs = new UniTaskCompletionSource();
            var tweenDescription = LeanTween.moveY(rectTransform, to, duration)
                .setOnComplete(() => tcs.TrySetResult());

            return (tcs.Task, tweenDescription);
        }
        
        public static Color SetAlpha(Color color, float alpha)
        {
            var result = color;
            result.a = alpha;
            
            return result;
        }

        private static void OnFlyAnimationAlphaFadeInUpdate(float progress, object image)
        {
            ProcessAlphaFadeIn(progress, image);
        }

        private static void OnFlyAnimationAlphaFadeOutUpdate(float progress, object image)
        {
            ProcessAlphaFadeOut(progress, image);
        }

        private static void OnFlyAnimationAlphaFadeInFadeOutUpdate(float progress, object image)
        {
            if (ProcessAlphaFadeIn(progress, image) == false)
            {
                ProcessAlphaFadeOut(progress, image);
            }
        }

        private static bool ProcessAlphaFadeIn(float progress, object image)
        {
            const float alphaProgressThreshold = 0.2f;

            if (progress <= alphaProgressThreshold)
            {
                var alpha = progress / alphaProgressThreshold;

                var imageCasted = (Image)image;
                imageCasted.color = SetAlpha(imageCasted.color, alpha);
                
                return true;
            }
            
            return false;
        }

        private static bool ProcessAlphaFadeOut(float progress, object image)
        {
            const float alphaProgressThreshold = 0.2f;

            if (progress >= 1 - alphaProgressThreshold)
            {
                var alpha = (1 - progress) / alphaProgressThreshold;

                var imageCasted = (Image)image;
                imageCasted.color = SetAlpha(imageCasted.color, alpha);

                return true;
            }

            return false;
        }
    }
}
