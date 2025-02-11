using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Utils.JSBridge
{
    public class JsBridge : MonoBehaviour, IJsBridge
    {
        public event Action<string> JsIncomingMessage; 
        
        [Space(25)]
        [SerializeField] private string _testJsMessage;

        [DllImport("__Internal")]
        private static extern void SendToJs(string str);

        public static JsBridge Instance { get; private set; }

        public void Awake()
        {
            Debug.Log("JsBridge Awaken");

            Instance = this;
        }

        public void JsCommandMessage(string payload)
        {
            JsIncomingMessage?.Invoke(payload);
        }

        public void SendCommandToJs(string command, object payload)
        {
            var dto = new UnityToJsCommonCommandDto()
            {
                command = command,
                payload = payload,
            };
            var dtoStr = JsonUtility.ToJson(dto);
            SendToJs(dtoStr);
        }

        public void SendCommandToJs(string command, string payload)
        {
            var dto = new UnityToJsSimpleCommandDto()
            {
                command = command,
                payload = payload,
            };
            var dtoStr = JsonUtility.ToJson(dto);
            SendToJs(dtoStr);
        }

        [ContextMenu("Imitate JsIncomingMessage")]
        private void ImitateSend()
        {
            JsIncomingMessage?.Invoke(_testJsMessage);
        }
    }

    [Serializable]
    public struct UnityToJsCommonCommandDto
    {
        public string command;
        public object payload;
    }
    
    [Serializable]
    public struct UnityToJsSimpleCommandDto
    {
        public string command;
        public string payload;
    }
    
    [Serializable]
    public struct JsToUnityCommonCommandDto
    {
        public string command;
        public object data;
    }
}