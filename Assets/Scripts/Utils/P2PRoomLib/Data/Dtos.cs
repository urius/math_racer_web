//ReSharper disable inconsistent naming

using System;

namespace Utils.P2PRoomLib.Data
{
    [Serializable]
    public class P2PGenericResponseDto<T> : IP2PBaseResponse
    {
        public int success;
        public int error_code;
        public int error_message;
        public T data;

        public bool IsNoError => success > 0 && error_code == 0;
        public int ErrorCode => error_code;
        public T Data => data;
    }
    
    [Serializable]
    public class P2PCreateRoomResponseDto : P2PGenericResponseDto<P2PCreateRoomDataDto>
    {
    }
    
    [Serializable]
    public class P2PReserveFreeChannelResponseDto : P2PGenericResponseDto<P2PRoomChannelDto>
    {
    }
    
    [Serializable]
    public class P2PAddHostChannelResponseDto : P2PGenericResponseDto<P2PRoomChannelDto[]>
    {
    }
    
    [Serializable]
    public class P2PGetJoiningResponseDto : P2PGenericResponseDto<P2PRoomChannelDto[]>
    {
    }
    
    [Serializable]
    public class P2PConnectToRoomResponseDto : P2PGenericResponseDto<P2PConnectToRoomDataDto>
    {
    }
    
    [Serializable]
    public struct P2PCreateRoomDataDto
    {
        public int room_id;
        
        public int RoomId => room_id;
    }
    
    [Serializable]
    public struct P2PRoomChannelDto
    {
        public int id;
        public string channel_key;
        public string join_key;
        public string connection_state;

        public int Id => id;
        public string ChannelKey => channel_key;
        public string JoinKey => join_key;
    }
    
    [Serializable]
    public struct P2PConnectToRoomDataDto
    {
        public string join_key;
    }

    public interface IP2PBaseResponse
    {
        public bool IsNoError { get; }
        public int ErrorCode { get; }
    }
}