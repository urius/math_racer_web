using Data;

namespace Extensions
{
    public static class CarKeyExtensions
    {
        public static PrefabKey ToPrefabKey(this CarKey carKey)
        {
            return (PrefabKey)((int)carKey + 1000);
        }
    }
}