using System;

namespace Data.Dto
{
    [Serializable]
    public struct AudioSettingsDto
    {
        public bool IsMusicMuted;
        public bool IsSoundsMuted;
        public float MusicVolume;
        public float SoundsVolume;

        public AudioSettingsDto(bool isMusicMuted, bool isSoundsMuted, float musicVolume, float soundsVolume)
        {
            IsMusicMuted = isMusicMuted;
            IsSoundsMuted = isSoundsMuted;
            MusicVolume = musicVolume;
            SoundsVolume = soundsVolume;
        }

        public static AudioSettingsDto FromDefault()
        {
            return new AudioSettingsDto(false, false, 0.5f, 0.5f);
        }
    }
}