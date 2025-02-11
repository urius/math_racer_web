using System;

namespace Utils.JSBridge
{
    public interface IJsBridge
    {
        public event Action<string> JsIncomingMessage;
        
        public void SendCommandToJs(string command, object payload);
        public void SendCommandToJs(string command, string payload);
    }
}