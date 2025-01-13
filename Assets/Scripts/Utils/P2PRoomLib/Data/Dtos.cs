//ReSharper disable inconsistent naming

using System;

namespace Utils.P2PRoomLib.Data
{
    [Serializable]
    public class P2PGenericResponseDto<T>
    {
        public int success;
        public T data;
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
    }
    
    [Serializable]
    public struct P2PRoomChannelDto
    {
        public int id;
        public string channel_key;
        public string join_key;
        public string connection_state;
    }
    
    [Serializable]
    public struct P2PConnectToRoomDataDto
    {
        public string join_key;
    }
}