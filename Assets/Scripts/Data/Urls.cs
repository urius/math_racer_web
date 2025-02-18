namespace Data
{
    public abstract class Urls
    {
        public static string P2PRoomsServiceUrl => $"{HostUrl}/math_racer/p2p_rooms/index.php";
        public static string HostUrl { get; private set; } = "https://twin-pixel.ru";

        public static void SetHostUrl(string hostUrl)
        {
            HostUrl = hostUrl;
        }
    }
}