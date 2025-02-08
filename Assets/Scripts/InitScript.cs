using System;
using Controller;
using Controller.Commands;
using Cysharp.Threading.Tasks;
using Data;
using GamePush;
using Infra.CommandExecutor;
using Infra.EventBus;
using Infra.Instance;
using Providers;
using Providers.LocalizationProvider;
using Services;
using UnityEngine;
using Utils.AudioManager;
using Utils.GamePush;
using Utils.JSBridge;
using View.UI;

public class InitScript : MonoBehaviour
{
    [SerializeField] private UILoadingOverlayView _loadingOverlayView;
    [SerializeField] private PrefabsHolderSo _prefabsHolderSo;
    [SerializeField] private UpdatesProvider _updatesProvider;
    [SerializeField] private LocalizationsHolderSo _localizationHolderSo;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private AudioClipsProviderSo _audioClipsProviderSo;
    [SerializeField] private JsBridge _jsBridge;

    private RootController _rootController;
    private UniTask<bool> _gpInitTask;
    
    private void Awake()
    {
        //LevelPointsHelper.TestLevelPointsHelper();
        Subscribe();
        
        Application.targetFrameRate = Constants.FPS;
        
        DontDestroyOnLoad(gameObject);
        
        _gpInitTask = GamePushWrapper.Init(RequestPauseDelegate);
        _prefabsHolderSo.Init();
        InitSounds();
        
        SetupInstances();
    }

    private void Start()
    {
        _gpInitTask.ContinueWith(OnGPReady);
    }

    private void InitSounds()
    {
        var soundKeys = Enum.GetValues(typeof(SoundKey));
        foreach (int i in soundKeys)
        {
            if (i > 0)
            {
                var sound = _audioClipsProviderSo.GetSoundByKey((SoundKey)i);
                _audioManager.SetupSound(i, sound);
            }
        }
    }

    private void OnJsIncomingMessage(string message)
    {
        Debug.Log("OnJsIncomingMessage, message: " + message);

        var messageDto = JsonUtility.FromJson<JsToUnityCommonCommandDto>(message);
        switch (messageDto.command)
        {
            case "SetHost":
            {
                var setHostDto = JsonUtility.FromJson<SetHostJsCommandDto>(message);
                Urls.SetHostUrl(setHostDto.HostUrl);
                break;
            }
            case "SetUserId":
            {
                var setUserIdDto = JsonUtility.FromJson<SetUserIdJsCommandDto>(message);
                var sessionDataModel = Instance.Get<IModelsHolder>().GetSessionDataModel();
                sessionDataModel.SocialData.SetSocialId(setUserIdDto.UserId);
                break;
            }
            case "RequestPause":
            {
                var requestPauseDto = JsonUtility.FromJson<RequestPauseJsCommandDto>(message);
                RequestPauseDelegate(requestPauseDto.NeedPause);
                break;
            }
        }
    }

    private void OnGPReady(bool initSuccess)
    {
        if (initSuccess)
        {
            Debug.Log("GP player id: " + GamePushWrapper.GetPlayerId());
            Debug.Log("GP player Name: " + GamePushWrapper.GetPlayerName());
            Debug.Log("GP language: " + GP_Language.Current());
            Debug.Log("GetActiveDaysConsecutive: " + GP_Player.GetActiveDaysConsecutive());

#if UNITY_EDITOR
            _localizationHolderSo.SetLocaleLang("ru");
#else
            _localizationHolderSo.SetLocaleLang(GamePushWrapper.GetLanguageShortDescription());
#endif
        }
        else
        {
            Debug.Log("GP Init failed. Try to continue without" );
            
            _localizationHolderSo.SetLocaleLang("ru");
        }
        
        InitRootController();
    }

    private void InitRootController()
    {
        _rootController = new RootController(_loadingOverlayView);
        
        _rootController.Initialize();
    }

    private void SetupInstances()
    {
        var carDataProvider = new CarDataProvider(_prefabsHolderSo);
        SetupInstance.From(carDataProvider).As<ICarDataProvider>();
        SetupInstance.From(_prefabsHolderSo).As<IPrefabHolder>();
        SetupInstance.From(_updatesProvider).As<IUpdatesProvider>();
        SetupInstance.From(_localizationHolderSo).As<ILocalizationProvider>();
        SetupInstance.From(_audioManager).As<IAudioPlayer>();
        SetupInstance.From(_audioClipsProviderSo).As<IAudioClipsProvider>();
        
        SetupNewInstance<CommandExecutor, ICommandExecutor>();
        SetupNewInstance<EventBus, IEventBus>();
        SetupNewInstance<ModelsHolder, IModelsHolder>();
        SetupNewInstance<ComplexityDataProvider, IComplexityDataProvider>();
        SetupNewInstance<P2PRoomService, IP2PRoomService>();
    }
    
    private void Map<TEvent, TCommand>()
        where TCommand : ICommand<TEvent>, new()
    {
        //_eventCommandMapper.Map<TEvent, TCommand>();
    }

    private static TInstance SetupNewInstance<TInstance, TInterface>()
        where TInstance : class, TInterface, new() 
        where TInterface : class
    {
        var instance = new TInstance();
        SetupInstance.From(instance).As<TInterface>();

        return instance;
    }
    
    private static TInstance SetupNewInstance<TInstance, TInterface1, TInterface2>()
        where TInstance : class, TInterface1 , TInterface2, new() 
        where TInterface1 : class
        where TInterface2 : class
    {
        var instance = new TInstance();
        SetupInstance.From(instance)
            .As<TInterface1>()
            .As<TInterface2>();

        return instance;
    }

    private static void RequestPauseDelegate(bool needPause)
    {
        Instance.Get<ICommandExecutor>().Execute<PerformGamePauseCommand, bool>(needPause);
    }

    private void Subscribe()
    {
        _jsBridge.JsIncomingMessage += OnJsIncomingMessage;
    }
    
    [Serializable]
    private struct SetHostJsCommandDto
    {
        public string data;

        public string HostUrl => data;
    }
    
    [Serializable]
    private struct SetUserIdJsCommandDto
    {
        public string data;

        public string UserId => data;
    }
    
    [Serializable]
    private struct RequestPauseJsCommandDto
    {
        public bool data;

        public bool NeedPause => data;
    }
}