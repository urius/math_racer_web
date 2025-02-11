using System;
using System.Text;
using Cysharp.Threading.Tasks;
using Data;
using Data.Dto;
using Helpers;
using Infra.CommandExecutor;
using Infra.Instance;
using Model;
using Providers;
using UnityEngine;
using Utils;
using Utils.GamePush;

namespace Controller.Commands
{
    public class InitPlayerModelCommand : IAsyncCommand
    {
        public async UniTask ExecuteAsync()
        {
            var modelsHolder = Instance.Get<IModelsHolder>();

            string playerDataStrLoaded = null;
            
            if (GamePushWrapper.IsVKPlatform)
            {
                var commandExecutor = Instance.Get<ICommandExecutor>();
                
                playerDataStrLoaded = await commandExecutor.ExecuteAsync<VkGetUserDataCommand, string>();
            }

            if (string.IsNullOrEmpty(playerDataStrLoaded))
            {
                playerDataStrLoaded = GamePushWrapper.GetPlayerData(Constants.PlayerDataFieldKey);
            }
            
            var playerDataDto = string.IsNullOrEmpty(playerDataStrLoaded) ? PlayerDataDto.FromDefault() : ConvertToDto(playerDataStrLoaded);

            var playerModel = PlayerDataConverter.ToPlayerModel(playerDataDto);

            UpdateTime(playerModel);
            
            modelsHolder.SetPlayerModel(playerModel);
        }

        private static void UpdateTime(PlayerModel playerModel)
        {
            playerModel.SetCurrentStartTimestamp(DateTimeHelper.GetUtcNowTimestamp());
            
            var prevTs = playerModel.PreviousStartUtcTimestamp;
            var currentTs = playerModel.CurrentStartUtcTimestamp;

            if (DateTimeHelper.IsSameDays(prevTs, currentTs)) return;

            playerModel.ResetDailyGiftTakenFlag();
            if (DateTimeHelper.IsNextDay(prevTs, currentTs))
            {
                playerModel.IncrementSequentialDaysPlaying();
            }
            else
            {
                playerModel.ResetSequentialDaysPlaying();
            }
        }

        private PlayerDataDto ConvertToDto(string playerDataStr)
        {
            var decodedStr = Decode(playerDataStr);
            var dto = JsonUtility.FromJson<PlayerDataDto>(decodedStr);
            
// #if UNITY_EDITOR //debug hook
//             dto.currentCar = (int)CarKey.Police;
// #endif
            return dto;
        }

        private static string Decode(string base64Str)
        {
            var decodedBytes = Convert.FromBase64String(base64Str);
            var decodedText = Encoding.UTF8.GetString(decodedBytes);

            return decodedText;
        }
    }
}