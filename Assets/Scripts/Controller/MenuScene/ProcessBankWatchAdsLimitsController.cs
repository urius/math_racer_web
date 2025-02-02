using Infra.Instance;
using Model;
using Providers;

namespace Controller.MenuScene
{
    public class ProcessBankWatchAdsLimitsController : ControllerBase
    {
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        
        private readonly BankAdWatchesModel _bankAdWatchesModel;

        public ProcessBankWatchAdsLimitsController(BankAdWatchesModel bankAdWatchesModel)
        {
            _bankAdWatchesModel = bankAdWatchesModel;
        }

        public override void Initialize()
        {
            _bankAdWatchesModel.Update();

            _updatesProvider.GameplaySecondPassed += OnGameplaySecondPassed;
        }

        public override void DisposeInternal()
        {
            _updatesProvider.GameplaySecondPassed -= OnGameplaySecondPassed;
        }

        private void OnGameplaySecondPassed()
        {
            _bankAdWatchesModel.Update();
        }
    }
}