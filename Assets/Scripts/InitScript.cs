using Controller;
using Data;
using Holders;
using Holders.LocalizationProvider;
using Infra.CommandExecutor;
using Infra.EventBus;
using Infra.Instance;
using UnityEngine;
using View.UI;

public class InitScript : MonoBehaviour
{
    [SerializeField] private UILoadingOverlayView _loadingOverlayView;
    [SerializeField] private PrefabsHolderSo _prefabsHolderSo;
    [SerializeField] private UpdatesProvider _updatesProvider;
    [SerializeField] private LocalizationsHolderSo _localizationHolderSo;

    private RootController _rootController;
    
    private void Awake()
    {
        Application.targetFrameRate = Constants.FPS;
        
        DontDestroyOnLoad(gameObject);

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
        SetupInstance.From(_prefabsHolderSo).As<IPrefabHolder>();
        SetupInstance.From(_updatesProvider).As<IUpdatesProvider>();
        SetupInstance.From(_localizationHolderSo).As<ILocalizationProvider>();
        
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