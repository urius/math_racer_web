using Cysharp.Threading.Tasks;
using Data;
using Events;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Providers;
using Utils.JSBridge;
using Utils.WebRequestSender;

namespace Controller.PlatformSpecific
{
    public class PlatformVKSpecificLogicController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IJsBridge _jsBridge = Instance.Get<IJsBridge>();

        private readonly PlayerModel _playerModel;
        private readonly SessionDataModel _sessionDataModel;

        private int _lastExpAmountSet;
        private int _lastLevelSet;
        private string _playerVkId;

        public PlatformVKSpecificLogicController()
        {
            _playerModel = _modelsHolder.GetPlayerModel();
            _sessionDataModel = _modelsHolder.GetSessionDataModel();
        }

        private static string BackendServiceUrl => $"{Urls.HostUrl}/math_racer/VK/index.php";

        public override void Initialize()
        {
            _lastExpAmountSet = _playerModel.ExpAmount;
            _lastLevelSet = _playerModel.Level;
            
            _playerVkId = _sessionDataModel.SocialData.SocialId;

            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _eventBus.Subscribe<DataSavedEvent>(OnDataSaved);
            _eventBus.Subscribe<UILeaderBoardButtonClickedEvent>(OnLeaderBoardButtonClickedEvent);
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<DataSavedEvent>(OnDataSaved);
            _eventBus.Unsubscribe<UILeaderBoardButtonClickedEvent>(OnLeaderBoardButtonClickedEvent);
        }

        private void OnLeaderBoardButtonClickedEvent(UILeaderBoardButtonClickedEvent e)
        {
            _jsBridge.SendCommandToJs("ShowLeaderBoard", null);
        }

        private void OnDataSaved(DataSavedEvent e)
        {
            SaveDataOnPlatform().Forget();
        }

        private async UniTaskVoid SaveDataOnPlatform()
        {
            if (_playerModel.ExpAmount > _lastExpAmountSet)
            {
                _lastExpAmountSet = _playerModel.ExpAmount;

                await WebRequestsSender.GetAsync(
                    $"{BackendServiceUrl}?command=set_points&user_id={_playerVkId}&points={_playerModel.ExpAmount}");
            }

            if (_playerModel.Level > _lastLevelSet)
            {
                _lastLevelSet = _playerModel.Level;

                await WebRequestsSender.GetAsync(
                    $"{BackendServiceUrl}?command=set_level&user_id={_playerVkId}&level={_playerModel.Level}");
            }
        }
    }
}