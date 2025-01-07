using Cysharp.Threading.Tasks;
using Data;
using Events;
using Infra.CommandExecutor;
using Infra.EventBus;
using Infra.Instance;
using Providers;
using Utils.GamePush;

namespace Controller.Commands
{
    public class ConsumeProductCommand : IAsyncCommandWithResult<bool, string>
    {
        public UniTask<bool> ExecuteAsync(string productTag)
        {
            var playerModel = Instance.Get<IModelsHolder>().GetPlayerModel();
            var eventBus = Instance.Get<IEventBus>();
            
            var splittedTag = productTag.Split("_");
            var productType = splittedTag[0];
            var productAmount = int.Parse(splittedTag[1]);

            if (productType == Constants.ProductTypeGold)
            {
                playerModel.AddGold(productAmount);
            }
            else
            {
                playerModel.AddCash(productAmount);
            }

            eventBus.Dispatch(new RequestSaveDataEvent());
            
            return GamePushWrapper.ConsumeProduct(productTag);
        }
    }
}