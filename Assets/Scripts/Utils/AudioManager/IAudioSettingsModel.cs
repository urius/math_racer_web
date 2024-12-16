using System;

namespace Utils.AudioManager
{
    public interface IAudioSettingsModel
    {
        public event Action<bool> SoundsMutedStateChanged;
        public event Action<bool> MusicMutedStateChanged;
        public event Action<float> SoundsVolumeChanged;
        public event Action<float> MusicVolumeChanged;
        
        public bool IsSoundsMuted { get; }
        public bool IsMusicMuted { get; }
        
        public float SoundsVolume { get; }
        public float MusicVolume { get; }
    }
}