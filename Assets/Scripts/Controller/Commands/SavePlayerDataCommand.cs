using System;
using System.Text;
using Data;
using Events;
using Infra.CommandExecutor;
using Infra.EventBus;
using Infra.Instance;
using Providers;
using UnityEngine;
using Utils;
using Utils.GamePush;
using Utils.JSBridge;

namespace Controller.Commands
{
    public class SavePlayerDataCommand : ICommand
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IJsBridge _jsBridge = Instance.Get<IJsBridge>();
        
        public void Execute()
        {
            var playerModel = _modelsHolder.GetPlayerModel();

            var playerDataDto = PlayerDataConverter.ToPlayerDataDto(playerModel);

            var playerDataJson = JsonUtility.ToJson(playerDataDto);
            var playerDataJsonB64 = Encode(playerDataJson);

            if (GamePushWrapper.IsVKPlatform)
            {
                _jsBridge.SendCommandToJs("SaveData", playerDataJsonB64);
            }
            else
            {
                GamePushWrapper.SavePlayerData(Constants.PlayerDataFieldKey, playerDataJsonB64);
            }
            
            Debug.Log("DATA SAVED");
            
            _eventBus.Dispatch(new DataSavedEvent());
        }
        
        private static string Encode(string input)
        {
            var bytesToEncode = Encoding.UTF8.GetBytes(input);
            var encodedText = Convert.ToBase64String(bytesToEncode);
            
            return encodedText;
        }
    }
}