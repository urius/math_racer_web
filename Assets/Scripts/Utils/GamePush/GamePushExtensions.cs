using GamePush;

namespace Utils.GamePush
{
    public static class GamePushExtensions
    {
        public static string ToLanguageShortStringRepresentation(this Language language)
        {
            return language switch
            {
                Language.Russian => "ru",
                Language.English => "en",
                _ => "en"
            };
        }
    }
}