using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GamePush;
using UnityEngine;

namespace Utils.GamePush
{
    public class GamePushWrapper
    {
        public static readonly GamePushWrapper Instance = new GamePushWrapper();

        private readonly UniTaskCompletionSource _initTcs = new UniTaskCompletionSource();

        private UniTaskCompletionSource<bool> _rewardedAdsTcs;
        private UniTaskCompletionSource<bool> _preloaderAdsTcs;
        private Action<bool> _requestPauseAction;
        private UniTaskCompletionSource<FetchProductData[]> _fetchProductsTcs;
        private UniTaskCompletionSource<FetchPurchaseData[]> _fetchPurchasesTcs;
        private UniTaskCompletionSource<bool> _purchaseTcs;
        private UniTaskCompletionSource<bool> _consumeTcs;

        public static UniTask<FetchPurchaseData[]> FetchPurchasesTask =>
            Instance._fetchPurchasesTcs?.Task ?? UniTask.FromResult(Array.Empty<FetchPurchaseData>());

        public static UniTask Init(Action<bool> requestPauseAction)
        {
            return Instance.InitInternal(requestPauseAction);
        }

        public static async UniTask<bool> ShowPreloaderAds()
        {
            Log($"{nameof(ShowPreloaderAds)} start");
            
            RequestPause(true);

            var showPreloaderAdsResult = await Instance.ShowPreloaderAdsInternal();
            
            RequestPause(false);
            
            Log($"{nameof(ShowPreloaderAds)} finish with result {showPreloaderAdsResult}");

            return showPreloaderAdsResult;
        }

        public static async UniTask<bool> ShowRewardedAds()
        {
            RequestPause(true);

            var showAdsResult = await Instance.ShowRewardedAdsInternal();
            
            RequestPause(false);

            return showAdsResult;
        }

        public static void ShowStickyBanner()
        {
            Log($"{nameof(ShowStickyBanner)}");
            
#if !UNITY_STANDALONE_OSX
            if (GP_Ads.IsStickyAvailable())
            {
                GP_Ads.ShowSticky();
            }
            else
            {
                LogWarning($"{nameof(ShowStickyBanner)} sticky is not available");
            }
#endif
        }

        public static string GetPlayerData(string fieldName)
        {
#if !UNITY_STANDALONE_OSX
            if (GP_Player.IsStub() == false)
            {
                return GP_Player.GetString(fieldName);
            }
#endif
#if UNITY_EDITOR
            return PlayerPrefs.GetString(fieldName);
#endif
            return null;
        }

        public static string GetPlayerId()
        {
#if !UNITY_STANDALONE_OSX
            return GP_Player.GetID().ToString();
#endif
            return "undefined_id";
        }

        public static string GetLanguageShortDescription()
        {
            var language = GP_Language.Current();

            return language switch
            {
                Language.Russian => "ru",
                Language.English => "en",
                _ => "en"
            };
        }

        public static void SavePlayerData(string fieldName, string value, bool needSync = true)
        {
#if !UNITY_STANDALONE_OSX
            GP_Player.Set(fieldName, value);
            
            if (needSync)
            {
                SyncPlayerData();
            }
#endif
#if UNITY_EDITOR
            PlayerPrefs.SetString(fieldName, value);
#endif
        }

        public static void SavePlayerData(string fieldName, long value, bool needSync = true)
        {
            SavePlayerData(fieldName, value.ToString(), needSync);
        }

        public static void SyncPlayerData()
        {
#if !UNITY_STANDALONE_OSX
            GP_Player.Sync();
#endif
        }

        public static void ResetPlayer()
        {
#if !UNITY_STANDALONE_OSX
            GP_Player.ResetPlayer();
            SyncPlayerData();
#endif
#if UNITY_EDITOR
            PlayerPrefs.DeleteAll();
#endif
        }

        public static string GetLanguage()
        {
#if !UNITY_STANDALONE_OSX
            var gpCurrentLanguage = GP_Language.Current();

            switch (gpCurrentLanguage)
            {
                case Language.Russian:
                    return "ru";
                case Language.English:
                case Language.Turkish:
                case Language.French:
                case Language.Italian:
                case Language.German:
                case Language.Spanish:
                case Language.Chineese:
                case Language.Portuguese:
                case Language.Korean:
                case Language.Japanese:
                case Language.Arab:
                case Language.Hindi:
                case Language.Indonesian:
                default:
                    return "en";
            }
#endif
            return "en";
        }

        public static bool IsPaymentsAvailable()
        {
#if !UNITY_STANDALONE_OSX && !UNITY_EDITOR
            return GP_Payments.IsPaymentsAvailable();
#endif
            return false;
        }

        public static bool CanShowRewardedAds()
        {
#if !UNITY_STANDALONE_OSX && !UNITY_EDITOR
            return GP_Ads.IsRewardedAvailable();
#endif
            return true;
        }

        public static UniTask<FetchProductData[]> FetchProducts()
        {
            if (IsPaymentsAvailable() == false)
            {
                LogWarning($"{nameof(FetchProducts)}: Payments are not available");

                return UniTask.FromResult(Array.Empty<FetchProductData>());
            }

            return Instance.FetchProductsInternal();
        }
        
        public static async UniTask<bool> PurchaseProduct(string idOrTag)
        {
            RequestPause(true);
            
            var purchaseResult = await Instance.PurchaseProductInternal(idOrTag);
            
            RequestPause(false);

            return purchaseResult;
        }
        
        public static UniTask<bool> ConsumeProduct(string idOrTag)
        {
            return Instance.ConsumeProductInternal(idOrTag);
        }

        private UniTask<bool> ConsumeProductInternal(string idOrTag)
        {
            _consumeTcs = new UniTaskCompletionSource<bool>();
            
            GP_Payments.Consume(idOrTag, OnConsumeSuccess, OnConsumeError);

            return _consumeTcs.Task;
        }

        private void OnConsumeSuccess(string idOrTag)
        {
            Log("PRODUCT CONSUME SUCCESS: " + idOrTag);
            
            _consumeTcs.TrySetResult(true);
        }

        private void OnConsumeError()
        {
            Log("PRODUCT CONSUME ERROR");
            
            _consumeTcs.TrySetResult(false);
        }

        private UniTask<bool> PurchaseProductInternal(string idOrTag)
        {
            _purchaseTcs = new UniTaskCompletionSource<bool>();
            
            GP_Payments.Purchase(idOrTag, OnPurchaseSuccess, OnPurchaseError);

            return _purchaseTcs.Task;
        }

        private void OnPurchaseSuccess(string idOrTag)
        {
            _purchaseTcs.TrySetResult(true);
        }

        private void OnPurchaseError()
        {
            _purchaseTcs.TrySetResult(false);
        }

        private UniTask<FetchProductData[]> FetchProductsInternal()
        {
            _fetchProductsTcs = new UniTaskCompletionSource<FetchProductData[]>();
            _fetchPurchasesTcs = new UniTaskCompletionSource<FetchPurchaseData[]>();
            
            GP_Payments.OnFetchProducts -= OnFetchProducts;
            GP_Payments.OnFetchProducts += OnFetchProducts;
            GP_Payments.OnFetchProductsError -= OnFetchProductsError;
            GP_Payments.OnFetchProductsError += OnFetchProductsError;
            GP_Payments.OnFetchPlayerPurchases -= OnFetchPlayerPurchases;
            GP_Payments.OnFetchPlayerPurchases += OnFetchPlayerPurchases;
            
            GP_Payments.Fetch();

            return _fetchProductsTcs.Task;
        }

        private void OnFetchProducts(List<FetchProducts> products)
        {
            var mappedProducts = products.Select(p => new FetchProductData(p)).ToArray();
            
            _fetchProductsTcs.TrySetResult(mappedProducts);
        }

        private void OnFetchProductsError()
        {
            LogError("Fetch products error!");
            
            _fetchProductsTcs.TrySetResult(Array.Empty<FetchProductData>());
        }

        private void OnFetchPlayerPurchases(List<FetchPlayerPurchases> purchases)
        {
            Log($"{nameof(OnFetchPlayerPurchases)} count: {purchases.Count}");
            
            var mappedPurchases = purchases.Select(p => new FetchPurchaseData(p)).ToArray();

            _fetchPurchasesTcs.TrySetResult(mappedPurchases);
        }

        private async UniTask<bool> ShowRewardedAdsInternal()
        {
#if !UNITY_STANDALONE_OSX && !UNITY_EDITOR
            _rewardedAdsTcs = new UniTaskCompletionSource<bool>();
            GP_Ads.ShowRewarded(onRewardedReward:RewardedAdsRewardedResultHandler, onRewardedClose:RewardedAdsClosedResultHandler);

            return await _rewardedAdsTcs.Task;
#endif
            await UniTask.Delay(500, DelayType.UnscaledDeltaTime);
            
            return true;
        }
        
        private async UniTask<bool> ShowPreloaderAdsInternal()
        {
#if !UNITY_STANDALONE_OSX && !UNITY_EDITOR
            _preloaderAdsTcs = new UniTaskCompletionSource<bool>();
            if (GP_Ads.IsPreloaderAvailable())
            {
                GP_Ads.ShowPreloader(onPreloaderClose: PreloaderAdsCloseHandler);
            }
            else
            {
                _preloaderAdsTcs.TrySetResult(false);
                
                LogError($"{nameof(ShowPreloaderAdsInternal)} ads is not available");
            }

            return await _preloaderAdsTcs.Task;
#endif
            await UniTask.Delay(500, DelayType.UnscaledDeltaTime);
            
            return true;
        }

        private UniTask InitInternal(Action<bool> requestPauseAction)
        {
            _requestPauseAction = requestPauseAction;
            
#if !UNITY_STANDALONE_OSX
            if (GP_Init.isReady)
            {
                _initTcs.TrySetResult();
            }
            else
            {
                GP_Init.OnReady -= OnGpInitReady;
                GP_Init.OnReady += OnGpInitReady;
                GP_Init.OnError -= OnGpInitError;
                GP_Init.OnError += OnGpInitError;
            }

            return _initTcs.Task;
#endif
            return UniTask.CompletedTask;
        }


        private void OnGpInitReady()
        {
#if !UNITY_STANDALONE_OSX
            GP_Init.OnError -= OnGpInitError;
            GP_Init.OnReady -= OnGpInitReady;
#endif
            
            _initTcs.TrySetResult();
        }

        private void OnGpInitError()
        {
            LogError("Game Push init error!");
        }

        private static void RequestPause(bool enable)
        {
            Instance._requestPauseAction?.Invoke(enable);
        }

        private void RewardedAdsRewardedResultHandler(string idOrTag)
        {
            _rewardedAdsTcs.TrySetResult(true);
        }

        private void RewardedAdsClosedResultHandler(bool success)
        {
            _rewardedAdsTcs.TrySetResult(success);
        }

        private void PreloaderAdsCloseHandler(bool result)
        {
            _preloaderAdsTcs.TrySetResult(result);
        }

        private static void Log(string message)
        {
            Debug.Log(GetLogMessageFormat(message));
        }

        private static void LogError(string message)
        {
            Debug.LogError(GetLogMessageFormat(message));
        }

        private static void LogWarning(string message)
        {
            Debug.LogWarning(GetLogMessageFormat(message));
        }

        private static string GetLogMessageFormat(string message)
        {
            return $"[{nameof(GamePushWrapper)}]: {message}";
        }
    }
}