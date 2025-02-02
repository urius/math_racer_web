using System;
using Helpers;

namespace Model
{
    public class BankAdWatchesModel
    {
        public event Action AvailableWatchesCountUpdated;
        
        private const int MaxBankAdWatchesCountBeforeCooldown = 10;
        private const int BankAdWatchesCooldownSeconds = 30 * 60;
        
        public int AdWatchesRest => MaxBankAdWatchesCountBeforeCooldown - AdLimitedWatchesCountHelper.AdWatchesCount;
        public int AdWatchCooldownSecondsRest =>
            LastAdWatchTime + BankAdWatchesCooldownSeconds - DateTimeHelper.GetUtcNowTimestamp();

        private static int LastAdWatchTime => AdLimitedWatchesCountHelper.LastAdWatchTime;

        public void Update()
        {
            if (AdWatchesRest < MaxBankAdWatchesCountBeforeCooldown 
                && AdWatchCooldownSecondsRest < 0)
            {
                ResetWatchesCount();
            }
        }

        public void HandleAdWatched()
        {
            AdLimitedWatchesCountHelper.LastAdWatchTime = DateTimeHelper.GetUtcNowTimestamp();
            AdLimitedWatchesCountHelper.AdWatchesCount++;
            
            AvailableWatchesCountUpdated?.Invoke();
        }

        private void ResetWatchesCount()
        {
            AdLimitedWatchesCountHelper.AdWatchesCount = 0;
            
            AvailableWatchesCountUpdated?.Invoke();
        }
    }
}