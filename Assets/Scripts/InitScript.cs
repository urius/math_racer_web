using Controller;
using Data;
using Infra.CommandExecutor;
using Infra.EventBus;
using Infra.Instance;
using Providers;
using Providers.LocalizationProvider;
using UnityEngine;
using Utils.AudioManager;
using View.UI;

public class InitScript : MonoBehaviour
{
    [SerializeField] private UILoadingOverlayView _loadingOverlayView;
    [SerializeField] private PrefabsHolderSo _prefabsHolderSo;
    [SerializeField] private UpdatesProvider _updatesProvider;
    [SerializeField] private LocalizationsHolderSo _localizationHolderSo;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private AudioClipsProviderSo _audioClipsProviderSo;

    private RootController _rootController;
    
    private void Awake()
    {
        //LevelPointsHelper.TestLevelPointsHelper();
        
        Application.targetFrameRate = Constants.FPS;
        
        DontDestroyOnLoad(gameObject);

        _prefabsHolderSo.Init();
        
        SetupInstances();
    }

    private void Start()
    {
        _localizationHolderSo.SetLocaleLang("en");
        
        InitRootControllers();
    }

    private void InitRootControllers()
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
}