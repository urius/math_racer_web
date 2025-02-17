namespace Model
{
    public class MultiplayerAvailabilityData
    {
        public bool IsChecked = false;
        public bool IsMultiplayerAvailable = false;

        public void SetMultiplayerCheckResult(bool isAvailable)
        {
            IsChecked = true;
            IsMultiplayerAvailable = isAvailable;
        }
    }
}