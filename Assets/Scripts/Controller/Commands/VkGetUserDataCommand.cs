using System;
using Cysharp.Threading.Tasks;
using Infra.CommandExecutor;
using Infra.Instance;
using Services;
using UnityEngine;
using Utils.JSBridge;

namespace Controller.Commands
{
    public class VkGetUserDataCommand : IAsyncCommandWithResult<string>
    {
        private readonly IJsBridge _jsBridge = Instance.Get<IJsBridge>();
        private readonly IHandleJsMessageService _jsMessageService = Instance.Get<IHandleJsMessageService>();
        
        private readonly UniTaskCompletionSource<string> _tcs = new();

        public UniTask<string> ExecuteAsync()
        {
            _jsBridge.SendCommandToJs("GetData", null);

            _jsMessageService.UnhandledJsMessageReceived += OnUnhandledJsMessageReceived;

            return _tcs.Task;
        }

        private void OnUnhandledJsMessageReceived(string command, string message)
        {
            if (command == "GetDataResult")
            {
                _jsMessageService.UnhandledJsMessageReceived -= OnUnhandledJsMessageReceived;
                
                var getDataDto = JsonUtility.FromJson<GetUserDataJsCommandDto>(message);
                _tcs.TrySetResult(getDataDto.Data);
            }
        }
        
        [Serializable]
        private struct GetUserDataJsCommandDto
        {
            public string data;

            public string Data => data;
        }
    }
}