using System;
using System.Text;
using Cysharp.Threading.Tasks;
using Data;
using Data.Dto;
using Infra.CommandExecutor;
using Infra.Instance;
using Providers;
using UnityEngine;
using Utils;
using Utils.GamePush;

namespace Controller.Commands
{
    public class InitPlayerModelCommand : IAsyncCommand
    {
        public UniTask ExecuteAsync()
        {
            var modelsHolder = Instance.Get<IModelsHolder>();

            var playerDataStrLoaded = GamePushWrapper.GetPlayerData(Constants.PlayerDataFieldKey);
            
            var playerDataDto = string.IsNullOrEmpty(playerDataStrLoaded) ? PlayerDataDto.FromDefault() : ConvertToDto(playerDataStrLoaded);

            var playerModel = PlayerDataConverter.ToPlayerModel(playerDataDto);
            playerModel.SetSocialData(GamePushWrapper.GetPlayerName());
            modelsHolder.SetPlayerModel(playerModel);
            
            return UniTask.CompletedTask;
        }

        private PlayerDataDto ConvertToDto(string playerDataStr)
        {
            var decodedStr = Decode(playerDataStr);
            var dto = JsonUtility.FromJson<PlayerDataDto>(decodedStr);

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