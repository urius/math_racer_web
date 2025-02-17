using Data;
using Utils.ReactiveValue;

namespace Model
{
    public class SessionDataModel
    {
        public readonly ReactiveFlag IsBankPopupOpened = new();
        public readonly ReactiveValue<BankProductsData> BankProductsData = new();
        public readonly PlayerSocialData SocialData = new();
        public readonly BankAdWatchesModel BankAdWatches = new();
        public readonly MultiplayerAvailabilityData MultiplayerAvailabilityData = new();

        public RequestSceneParamsBase RequestSceneParams;
    }
}