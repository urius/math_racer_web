using UnityEngine;

namespace Helpers
{
    public static class AdLimitedWatchesCountHelper
    {
        private const string BankAdvertWatchesCountKey = "limited_advert_watches_count";
        private const string BankAdvertWatchTimeKey = "limited_advert_watch_time";

        private static int _bankAdvertWatchesCount = -1;
        private static int _bankAdvertWatchTime = -1;

        public static int AdWatchesCount
        {
            get
            {
                if (_bankAdvertWatchesCount < 0)
                {
                    _bankAdvertWatchesCount = PlayerPrefs.GetInt(BankAdvertWatchesCountKey, 0);
                }

                return _bankAdvertWatchesCount;
            }
            set
            {
                _bankAdvertWatchesCount = value;
                PlayerPrefs.SetInt(BankAdvertWatchesCountKey, _bankAdvertWatchesCount);
            }
        }

        public static int LastAdWatchTime
        {
            get
            {
                if (_bankAdvertWatchTime < 0)
                {
                    _bankAdvertWatchTime = PlayerPrefs.GetInt(BankAdvertWatchTimeKey, 0);
                }

                return _bankAdvertWatchTime;
            }
            set
            {
                _bankAdvertWatchTime = value;
                PlayerPrefs.SetInt(BankAdvertWatchTimeKey, _bankAdvertWatchTime);
            }
        }
    }
}