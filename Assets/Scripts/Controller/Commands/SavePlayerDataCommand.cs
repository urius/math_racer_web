using System;
using System.Text;
using Data;
using Infra.CommandExecutor;
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

            var playerDataDto = PlayerDataConverter.ToPlayerDataDto(playerModel);

            var playerDataJson = JsonUtility.ToJson(playerDataDto);
            var playerDataJsonB64 = Encode(playerDataJson);

            GamePushWrapper.SavePlayerData(Constants.PlayerDataFieldKey, playerDataJsonB64);
            
            Debug.Log("DATA SAVED");
        }
        
        private static string Encode(string input)
        {
            var bytesToEncode = Encoding.UTF8.GetBytes(input);
            var encodedText = Convert.ToBase64String(bytesToEncode);
            
            return encodedText;
        }
    }
}