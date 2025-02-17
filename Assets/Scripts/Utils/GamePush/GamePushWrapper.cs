using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GamePush;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utils.GamePush
{
    public class GamePushWrapper
    {
        public static readonly GamePushWrapper Instance = new GamePushWrapper();

        private const string PlayerFallbackNameFieldKey = "player_name_fallback";
            
        private readonly UniTaskCompletionSource<bool> _initTcs = new();

        private UniTaskCompletionSource<bool> _rewardedAdsTcs;
        private UniTaskCompletionSource<bool> _preloaderAdsTcs;
        private Action<bool> _requestPauseAction;
        private UniTaskCompletionSource<FetchProductData[]> _fetchProductsTcs;
        private UniTaskCompletionSource<FetchPurchaseData[]> _fetchPurchasesTcs;
        private UniTaskCompletionSource<bool> _purchaseTcs;
        private UniTaskCompletionSource<bool> _consumeTcs;
        private UniTaskCompletionSource<bool> _inviteTcs;

        public static UniTask<FetchPurchaseData[]> FetchPurchasesTask =>
            Instance._fetchPurchasesTcs?.Task ?? UniTask.FromResult(Array.Empty<FetchPurchaseData>());

        public static bool IsGPInit => GP_Init.isReady;
        public static bool IsRewardedAdsShowInProgress => Instance._rewardedAdsTcs != null;
        public static bool IsVKPlatform => PlatformType == Platform.VK;
        public static bool IsYandexPlatform => PlatformType == Platform.YANDEX;
        public static bool IsRewardedAdsAvailable => CanShowRewardedAds();

        private static Platform PlatformType => IsGPInit ? GP_Platform.Type() : Platform.NONE;

        public static UniTask<bool> Init(Action<bool> requestPauseAction)
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
#if !UNITY_STANDALONE_OSX && !UNITY_EDITOR
            if (IsGPInit && GP_Player.IsStub() == false)
            {
                return GP_Player.GetString(fieldName);
            }
#endif
            return PlayerPrefs.GetString(fieldName);
        }

        public static string GetPlayerId()
        {
#if !UNITY_STANDALONE_OSX
            if (IsGPInit)
            {
                return GP_Player.GetID().ToString();
            }
#endif
            return "undefined_id";
        }
        
        public static string GetPlayerName()
        {
            Debug.Log("GetPlayerName, IsGPInit:" + IsGPInit);
            
#if !UNITY_STANDALONE_OSX
            if (IsGPInit)
            {
                var name = GP_Player.GetName();
                if (string.IsNullOrEmpty(name))
                {
                    name = "Empty player name";
                }

                return name;
            }
#endif

            return GetOrCreatePlayerPref(PlayerFallbackNameFieldKey, "Player_" + (int)(Random.value * 1000));
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
#if !UNITY_STANDALONE_OSX && !UNITY_EDITOR
            if (IsGPInit)
            {
                GP_Player.Set(fieldName, value);

                if (needSync)
                {
                    SyncPlayerData();
                }

                return;
            }
#endif
            PlayerPrefs.SetString(fieldName, value);
        }

        public static void SavePlayerData(string fieldName, long value, bool needSync = true)
        {
            SavePlayerData(fieldName, value.ToString(), needSync);
        }

        public static void SyncPlayerData()
        {
#if !UNITY_STANDALONE_OSX && !UNITY_EDITOR
            if (IsGPInit)
            {
                GP_Player.Sync();
            }
#endif
        }

        public static void ResetPlayer()
        {
#if !UNITY_STANDALONE_OSX && !UNITY_EDITOR
            if (IsGPInit)
            {
                GP_Player.ResetPlayer();
                SyncPlayerData();
            }
#endif
            PlayerPrefs.DeleteAll();
        }

        public static string GetLanguage()
        {
#if !UNITY_STANDALONE_OSX
            if (IsGPInit)
            {
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
            }
#endif
            return "en";
        }

        public static bool IsPaymentsAvailable()
        {
#if !UNITY_STANDALONE_OSX && !UNITY_EDITOR
            return IsGPInit && GP_Payments.IsPaymentsAvailable();
#endif
            return false;
        }

        public static bool CanShowRewardedAds()
        {
#if !UNITY_STANDALONE_OSX && !UNITY_EDITOR
            return IsGPInit && GP_Ads.IsRewardedAvailable();
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

        public static UniTask<bool> Invite()
        {
            return Instance.InviteInternal();
        }

        public static bool IsInviteAvailable()
        {
            return IsGPInit && GP_Socials.IsSupportsNativeInvite();
        }

        private UniTask<bool> InviteInternal()
        {
            _inviteTcs?.TrySetResult(false);
            _inviteTcs = new UniTaskCompletionSource<bool>();

            GP_Socials.OnInvite -= OnInvite;
            GP_Socials.OnInvite += OnInvite;
            
            GP_Socials.Invite();

            return _inviteTcs.Task;
        }

        private void OnInvite(bool isSuccess)
        {
            if (_inviteTcs != null)
            {
                _inviteTcs.TrySetResult(isSuccess);
                _inviteTcs = null;
            }
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
            if (_rewardedAdsTcs == null)
            {
                _rewardedAdsTcs = new UniTaskCompletionSource<bool>();
#if !UNITY_STANDALONE_OSX && !UNITY_EDITOR
                GP_Ads.ShowRewarded(onRewardedReward:RewardedAdsRewardedResultHandler, onRewardedClose:RewardedAdsClosedResultHandler);
#else
                await UniTask.Delay(500, DelayType.UnscaledDeltaTime);
                _rewardedAdsTcs.TrySetResult(true);
#endif
            }

            var result = await _rewardedAdsTcs.Task;
            
            _rewardedAdsTcs = null;
            
            return result;
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

        private UniTask<bool> InitInternal(Action<bool> requestPauseAction)
        {
            _requestPauseAction = requestPauseAction;
            
#if !UNITY_STANDALONE_OSX
            if (GP_Init.isReady)
            {
                _initTcs.TrySetResult(true);
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
            return UniTask.FromResult(true);
        }


        private void OnGpInitReady()
        {
#if !UNITY_STANDALONE_OSX
            GP_Init.OnError -= OnGpInitError;
            GP_Init.OnReady -= OnGpInitReady;
#endif
            
            _initTcs.TrySetResult(true);
        }

        private void OnGpInitError()
        {
            LogError("Game Push init error!");
            
            _initTcs.TrySetResult(false);
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

        private static string GetOrCreatePlayerPref(string playerPrefsKey, string defaultValue)
        {
            var result = PlayerPrefs.GetString(playerPrefsKey);
            
            Debug.Log("GetPlayerName, result1:" + result);
            
            if (string.IsNullOrEmpty(result))
            {
                result = defaultValue;
                PlayerPrefs.SetString(playerPrefsKey, result);
            }
            
            Debug.Log("GetPlayerName, result2:" + result);
            
            return result;
        }
    }
}