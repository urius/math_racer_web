using System;

namespace Utils.AudioManager
{
    public abstract class AudioSettingsModelBase : IAudioSettingsModel
    {
        public event Action<bool> SoundsMutedStateChanged;
        public event Action<bool> MusicMutedStateChanged;
        public event Action<float> SoundsVolumeChanged;
        public event Action<float> MusicVolumeChanged;
        
        public bool IsSoundsMuted { get; private set; }
        public bool IsMusicMuted { get; private set; }
        public float SoundsVolume { get; private set; }
        public float MusicVolume { get; private set; }

        public AudioSettingsModelBase(bool isSoundsMuted, bool isMusicMuted, float soundsVolume, float musicVolume)
        {
            IsSoundsMuted = isSoundsMuted;
            IsMusicMuted = isMusicMuted;
            SoundsVolume = soundsVolume;
            MusicVolume = musicVolume;
        }

        public void SetMusicVolume(float volume)
        {
            MusicVolume = volume;
            MusicVolumeChanged?.Invoke(MusicVolume);
        }
        
        public void SetSoundsVolume(float volume)
        {
            SoundsVolume = volume;
            SoundsVolumeChanged?.Invoke(SoundsVolume);
        }

        public void SetSoundsMuted(bool isMuted)
        {
            IsSoundsMuted = isMuted;
            SoundsMutedStateChanged?.Invoke(IsSoundsMuted);
        }

        public void SetMusicMuted(bool isMuted)
        {
            IsMusicMuted = isMuted;
            MusicMutedStateChanged?.Invoke(IsMusicMuted);
        }
    }
}