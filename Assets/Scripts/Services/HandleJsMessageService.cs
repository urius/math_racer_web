using System;
using Controller.Commands;
using Data;
using Infra.CommandExecutor;
using Infra.Instance;
using Providers;
using UnityEngine;
using Utils.JSBridge;

namespace Services
{
    public class HandleJsMessageService : IHandleJsMessageService
    {
        public event Action<string, string> UnhandledJsMessageReceived;
        
        private readonly IJsBridge _jsBridge = Instance.Get<IJsBridge>();
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly ICommandExecutor _commandExecutor = Instance.Get<ICommandExecutor>();

        public HandleJsMessageService()
        {
            _jsBridge.JsIncomingMessage += OnJsIncomingMessage;
        }

        private void OnJsIncomingMessage(string message)
        {
            Debug.Log("OnJsIncomingMessage, message: " + message);

            var messageDto = JsonUtility.FromJson<JsToUnityCommonCommandDto>(message);
            switch (messageDto.command)
            {
                case "SetHost":
                {
                    var setHostDto = JsonUtility.FromJson<SetHostJsCommandDto>(message);
                    Urls.SetHostUrl(setHostDto.HostUrl);
                    break;
                }
                case "SetUserId":
                {
                    var setUserIdDto = JsonUtility.FromJson<SetUserIdJsCommandDto>(message);
                    var sessionDataModel = _modelsHolder.GetSessionDataModel();
                    sessionDataModel.SocialData.SetSocialId(setUserIdDto.UserId);
                    break;
                }
                case "RequestPause":
                {
                    var requestPauseDto = JsonUtility.FromJson<RequestPauseJsCommandDto>(message);
                    _commandExecutor.Execute<PerformGamePauseCommand, bool>(requestPauseDto.NeedPause);
                    break;
                }
                default:
                    UnhandledJsMessageReceived?.Invoke(messageDto.command, message);
                    break;
            }
        }

        [Serializable]
        private struct SetHostJsCommandDto
        {
            public string data;

            public string HostUrl => data;
        }

        [Serializable]
        private struct SetUserIdJsCommandDto
        {
            public string data;

            public string UserId => data;
        }

        [Serializable]
        private struct RequestPauseJsCommandDto
        {
            public bool data;

            public bool NeedPause => data;
        }
    }

    public interface IHandleJsMessageService
    {
        public event Action<string, string> UnhandledJsMessageReceived;
    }
}