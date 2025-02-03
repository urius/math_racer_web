using Data;
using Extensions;
using Infra.Instance;
using Model;
using Providers;
using View;
using View.UI.MenuScene;

namespace Controller.MenuScene
{
    public class MenuSceneBackgroundViewController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        
        private readonly UIMenuSceneRootView _rootView;
        
        private PlayerModel _playerModel;
        private CarView _carView;

        public MenuSceneBackgroundViewController(UIMenuSceneRootView rootView)
        {
            _rootView = rootView;
        }

        public override void Initialize()
        {
            _playerModel = _modelsHolder.GetPlayerModel();

            DisplayPlayerCar();

            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _playerModel.CurrentCarUpdated += OnCurrentCarUpdated;
        }

        private void Unsubscribe()
        {
            _playerModel.CurrentCarUpdated -= OnCurrentCarUpdated;
        }

        private void OnCurrentCarUpdated(CarKey carKey)
        {
            DisplayPlayerCar();
        }

        private void DisplayPlayerCar()
        {
            _rootView.ClearPlayerCarContainerChildren();
            
            var carPrefabKey = _playerModel.CurrentCar.ToPrefabKey();
            _carView = Instantiate<CarView>(carPrefabKey, _rootView.PlayerCarContainerTransform);
            
            _carView.ShowExhaustVFX();
        }
    }
}