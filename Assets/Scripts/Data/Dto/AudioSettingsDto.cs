// ReSharper disable InconsistentNaming

using System;

namespace Data.Dto
{
    [Serializable]
    public struct AudioSettingsDto
    {
        public bool isSoundsMuted;
        public bool isMusicMuted;
        public float soundsVolume;
        public float musicVolume;

        public AudioSettingsDto( bool isSoundsMuted,bool isMusicMuted, float soundsVolume, float musicVolume)
        {
            this.isSoundsMuted = isSoundsMuted;
            this.isMusicMuted = isMusicMuted;
            this.soundsVolume = soundsVolume;
            this.musicVolume = musicVolume;
        }

        public static AudioSettingsDto FromDefault()
        {
            return new AudioSettingsDto(false, false, 0.5f, 0.5f);
        }
    }
}