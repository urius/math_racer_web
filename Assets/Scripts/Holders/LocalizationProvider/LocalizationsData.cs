using System;

namespace Holders.LocalizationProvider
{
    [Serializable]
    public struct LocalizationsData
    {
        public LocalizationItemData[] localizations;
    }
    
    [Serializable]
    public struct LocalizationItemData
    {
        public string key;
        public string en;
        public string ru;
    }
}