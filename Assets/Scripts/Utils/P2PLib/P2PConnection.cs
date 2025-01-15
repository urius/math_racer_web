using System;
using Cysharp.Threading.Tasks;

namespace Utils.P2PLib
{
    public class P2PConnection : IP2PHostSideConnection, IP2PJoinSideConnection
    {
        public event Action<P2PConnection> ChanelOpened;
        public event Action<P2PConnection> ChanelClosed;
        public event Action<P2PConnection, string> MessageReceived;

        private readonly UniTaskCompletionSource _connectionEstablishedTcs = new();

        public P2PConnection(bool isHost, string channelLabel, string connectionLocalDescription)
        {
            IsHost = isHost;
            ChannelLabel = channelLabel;
            ConnectionLocalDescription = connectionLocalDescription;

            ConnectionState = P2PConnectionState.Connecting;
        }

        public UniTask ConnectionEstablishedTask => _connectionEstablishedTcs.Task;
        public bool IsHost { get; }
        public P2PConnectionState ConnectionState { get; private set; }
        public string ChannelLabel { get; private set; }
        public string ConnectionLocalDescription { get; private set; }
        
        public static async UniTask<IP2PHostSideConnection> HostInit()
        {
            return await P2PLibJsApi.Host();
        }

        public static async UniTask<IP2PJoinSideConnection> Join(string hostConnectionDescription)
        {
            return await P2PLibJsApi.Join(hostConnectionDescription);
        }

        public void HostComplete(string joinConnectionDescription)
        {
            P2PLibJsApi.CompleteHostConnection(joinConnectionDescription);
        }

        public void SendMessage(string msg)
        {
            P2PLibJsApi.SendTo(ChannelLabel, msg);
        }

        public void Close()
        {
            P2PLibJsApi.Close(ChannelLabel);
        }

        public void HandleChannelOpened()
        {
            ConnectionState = P2PConnectionState.Established;
            
            ChanelOpened?.Invoke(this);
            _connectionEstablishedTcs.TrySetResult();
        }

        public void HandleChannelClosed()
        {
            ConnectionState = P2PConnectionState.Closed;
            
            ChanelClosed?.Invoke(this);
        }

        public void HandleMessageReceived(string msg)
        {
            MessageReceived?.Invoke(this, msg);
        }
    }

    public enum P2PConnectionState
    {
        None = 0,
        Connecting,
        Established,
        Closed,
    }


    public interface IP2PConnection
    {
        public event Action<P2PConnection> ChanelOpened;
        public event Action<P2PConnection> ChanelClosed;
        public event Action<P2PConnection, string> MessageReceived;
        
        public UniTask ConnectionEstablishedTask { get; }
        public bool IsHost { get; }
        public string ChannelLabel { get; }
        public string ConnectionLocalDescription { get; }
        public P2PConnectionState ConnectionState { get; }

        public void SendMessage(string msg);
        public void Close();
    }

    public interface IP2PHostSideConnection : IP2PConnection
    {
        public void HostComplete(string joinConnectionDescription);
    }
    
    public interface IP2PJoinSideConnection : IP2PConnection
    {
    }
}