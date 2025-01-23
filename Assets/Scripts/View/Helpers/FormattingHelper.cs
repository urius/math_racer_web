namespace Utils
{
    public static class FormattingHelper
    {
        public static string ToSeparatedTimeFormat(int timeSeconds)
        {
            var hours = timeSeconds / 3600;
            var restSeconds = timeSeconds % 3600;
            var minutes = restSeconds / 60;
            restSeconds %= 60;
            
            return hours > 0 ? $"{GetTwoDigitsString(hours)}:{GetTwoDigitsString(minutes)}:{GetTwoDigitsString(restSeconds)}" : $"{GetTwoDigitsString(minutes)}:{GetTwoDigitsString(restSeconds)}";
        }

        public static string ToCommaSeparatedNumber(int amount)
        {
            return $"{amount:n0}";
        }

        private static string GetTwoDigitsString(int value)
        {
            return value < 10 ? $"0{value}" : value.ToString();
        }

        public static string ToTimeFormatMinSec(int seconds)
        {
            var minutes = seconds / 60;
            var remainingSeconds = seconds % 60;

            var formattedTime = $"{minutes:D2}:{remainingSeconds:D2}";

            return formattedTime;
        }
        
        public static string ToTimeFormatMinSecMs(int milliseconds)
        {
            var totalSeconds = milliseconds / 1000;
            var minutes = totalSeconds / 60;
            var remainingSeconds = totalSeconds % 60;
            var remainingMilliseconds = milliseconds % 1000;

            var formattedTime = $"{minutes:D2}:{remainingSeconds:D2}:{remainingMilliseconds:D3}";

            return formattedTime;
        }
    }
}
