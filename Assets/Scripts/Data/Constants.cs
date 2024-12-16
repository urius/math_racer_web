namespace Data
{
    public class Constants
    {
        public const string BootstrapSceneName = "BootstrapScene";
        public const string MenuSceneName = "MenuScene";
        public const string RaceSceneName = "RaceScene";
        public const string NewLevelSceneName = "NewLevelScene";
        
        public const string OperatorPlus = "+";
        public const string OperatorMinus = "-";
        public const string OperatorMultiply = "*";
        public const string OperatorDivide = "/";
        public const char OperatorPlusChar = '+';
        public const char OperatorMinusChar = '-';
        public const char OperatorMultiplyChar = '*';
        public const char OperatorDivideChar = '/';
        
        public const string TextSpriteCash = "<sprite name=\"cash\">";
        public const string TextSpriteCrystal = "<sprite name=\"crystal\">";
        public const string TextSpriteAds = "<sprite name=\"ads\">";
        public const string TextCrystalLiteBlueColor = "#D1DFFF";
        public const string TextCrystalBlueColor = "#17B0FF";

        public const int FPS = 50;
        public const float KmphToMps = 1000f / 3600f;
        public const float KmphToMetersPerFrame = KmphToMps / FPS;
        public const float MetersInKm = 1000;
        
        public const int PrefabCarsOffset = 1000;
        public const int MaxCarSpeedKmph = 500;
        public const int MaxCarAcceleration = 50;

        public const string PrefabPathExhaust = "Prefabs/vfx/PS_Exhaust";
        public const string PrefabPathTurboBooster = "Prefabs/vfx/PS_TurboBooster";
        
        public const float TurboIndicatorShowHideDurationSec = 0.2f;
        public const int TurboIndicatorShowHideDurationMs = (int)(TurboIndicatorShowHideDurationSec * 1000);

        public const int SolveCostCrystals = 1;
    }
}