using Cysharp.Threading.Tasks;
using Data.Dto;
using Infra.CommandExecutor;
using Infra.Instance;
using Providers;
using Utils;

namespace Controller.Commands
{
    public class InitPlayerModelCommand : IAsyncCommand
    {
        public UniTask ExecuteAsync()
        {
            var modelsHolder = Instance.Get<IModelsHolder>();
            
            var playerDataDto = PlayerDataDto.FromDefault();

            var playerModel = PlayerDataConverter.ToPlayerModel(playerDataDto);
            modelsHolder.SetPlayerModel(playerModel);
            
            return UniTask.CompletedTask;
        }
    }
}