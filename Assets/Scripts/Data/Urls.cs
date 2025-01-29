namespace Data
{
    public abstract class Urls
    {
        private static string _hostUrl = "http://localhost";
        
        public static string P2PRoomsServiceUrl => $"{_hostUrl}/math_racer/p2p_rooms/index.php";

        public static void SetHostUrl(string hostUrl)
        {
            _hostUrl = hostUrl;
        }
    }
}