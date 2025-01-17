using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Utils.P2PLib
{
    public static class P2PLibJsApi
    {
        private const string DefaultIceServer = "stun:stun.l.google.com:19302"; 
            
        private static readonly LinkedList<UniTaskCompletionSource<P2PConnection>> HostNewConnectionTcsList = new();
        private static readonly LinkedList<UniTaskCompletionSource<P2PConnection>> JoinConnectionTcsList = new();
        private static readonly Dictionary<string, P2PConnection> P2PHostConnectionContexts = new();
        private static readonly Dictionary<string, P2PConnection> P2PJoinConnectionContexts = new();

        private static bool _isInitialized = false;
        
        [DllImport("__Internal")]
        public static extern void SetupIceServerUrl(string url);
        
        [DllImport("__Internal")]
        public static extern void SendToAll(string msg);
        
        [DllImport("__Internal")]
        public static extern void SendTo(string channelLabel, string msg);
        
        [DllImport("__Internal")]
        public static extern void Close(string channelLabel);
        
        [DllImport("__Internal")]
        private static extern void InitLib();

        [DllImport("__Internal")]
        private static extern void HostNewConnection(
            Action<string, string> callback,
            Action<string> onChannelOpenCallback,
            Action<string> onChannelCloseCallback,
            Action<string, string> onMessageReceivedCallback);
        
        [DllImport("__Internal")]
        private static extern void JoinConnection(
            string hostConnectionDescription,
            Action<string, string> callback,
            Action<string> onChannelOpenCallback,
            Action<string> onChannelCloseCallback,
            Action<string, string> onMessageReceivedCallback);
        
        [DllImport("__Internal")]
        public static extern void CompleteHostConnection(string joinConnectionDescription);

        public static UniTask<P2PConnection> Host()
        {
            InitIfNeeded();
            
            var tcs = new UniTaskCompletionSource<P2PConnection>();
            HostNewConnectionTcsList.AddLast(tcs);
            
            HostNewConnection(HostNewConnectionCallback, ChannelOpenCallback, ChannelCloseCallback, MessageReceivedCallback);

            return tcs.Task;
        }

        public static UniTask<P2PConnection> Join(string hostConnectionDescription)
        {
            InitIfNeeded();
            
            var tcs = new UniTaskCompletionSource<P2PConnection>();
            JoinConnectionTcsList.AddLast(tcs);

            JoinConnection(
                hostConnectionDescription,
                JoinNewConnectionCallback,
                ChannelOpenCallback,
                ChannelCloseCallback,
                MessageReceivedCallback);

            return tcs.Task;
        }

        private static void InitIfNeeded()
        {
            if (_isInitialized) return;
            
            InitLib();
            _isInitialized = true;
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void CSharpCallback(string message)
        {
            Debug.Log($"C# callback received \"{message}\"");
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void HostNewConnectionCallback(string channelLabel, string connectionLocalDescription)
        {
            Debug.Log(
                $"HostNewConnectionCallback received channelLabel:\"{channelLabel}\"\n connectionLocalDescription: \n{connectionLocalDescription}");
            
            var tcs = HostNewConnectionTcsList.First.Value;
            HostNewConnectionTcsList.RemoveFirst();

            var context = new P2PConnection(isHost: true, channelLabel, connectionLocalDescription);
            P2PHostConnectionContexts[channelLabel] = context;
            
            tcs.TrySetResult(context);
        }
        
        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void JoinNewConnectionCallback(string channelLabel, string connectionLocalDescription)
        {
            Debug.Log(
                $"JoinNewConnectionCallback received channelLabel:\"{channelLabel}\"\n connectionLocalDescription: \n{connectionLocalDescription}");
            
            var tcs = JoinConnectionTcsList.First.Value;
            JoinConnectionTcsList.RemoveFirst();

            var context = new P2PConnection(isHost: false, channelLabel, connectionLocalDescription);
            P2PJoinConnectionContexts[channelLabel] = context;
            
            tcs.TrySetResult(context);
        }
        
        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void ChannelOpenCallback(string channelLabel)
        {
            Debug.Log(
                $"ChannelOpenCallback received channelLabel:\"{channelLabel}\"");

            foreach (var connection in GetConnectionsByChannelLabel(channelLabel)) connection.HandleChannelOpened();
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void ChannelCloseCallback(string channelLabel)
        {
            Debug.Log(
                $"ChannelCloseCallback received channelLabel:\"{channelLabel}\"");
            
            foreach (var connection in GetConnectionsByChannelLabel(channelLabel)) connection.HandleChannelClosed();
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void MessageReceivedCallback(string channelLabel, string message)
        {
            //Debug.Log($"MessageReceivedCallback received channelLabel:\"{channelLabel}\",\nmessage: {message}");

            if (TruGetConnectionByChannelLabel(channelLabel, out var connection))
                connection.HandleMessageReceived(message);
        }

        private static IEnumerable<P2PConnection> GetConnectionsByChannelLabel(string channelLabel)
        {
            if (P2PHostConnectionContexts.TryGetValue(channelLabel, out var hostConnection))
            {
                yield return hostConnection;
            }
            
            if (P2PJoinConnectionContexts.TryGetValue(channelLabel, out var joinConnection))
            {
                yield return joinConnection;
            }
        }
        
        private static bool TruGetConnectionByChannelLabel(string channelLabel, out P2PConnection connection)
        {
            connection = null;
            return P2PHostConnectionContexts.TryGetValue(channelLabel, out connection) ||
                   P2PJoinConnectionContexts.TryGetValue(channelLabel, out connection);
        }
    }
}