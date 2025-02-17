using Utils.GamePush;

namespace Data
{
    public static class PlatformSettings
    {
        public static bool IsLeaderBoardEnabled => GamePushWrapper.IsVKPlatform;
    }
}