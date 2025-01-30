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

namespace Controller.Commands
{
    public class SavePlayerDataCommand : ICommand
    {
        public void Execute()
        {
            var playerModel = Instance.Get<IModelsHolder>().GetPlayerModel();
            var eventBus = Instance.Get<IEventBus>();

            var playerDataDto = PlayerDataConverter.ToPlayerDataDto(playerModel);

            var playerDataJson = JsonUtility.ToJson(playerDataDto);
            var playerDataJsonB64 = Encode(playerDataJson);

            GamePushWrapper.SavePlayerData(Constants.PlayerDataFieldKey, playerDataJsonB64);
            
            Debug.Log("DATA SAVED");
            
            eventBus.Dispatch(new DataSavedEvent());
        }
        
        private static string Encode(string input)
        {
            var bytesToEncode = Encoding.UTF8.GetBytes(input);
            var encodedText = Convert.ToBase64String(bytesToEncode);
            
            return encodedText;
        }
    }
}