using Utils.AudioManager;

namespace Model
{
    public class AudioSettingsModel : AudioSettingsModelBase
    {
        public AudioSettingsModel(
            bool isSoundsMuted,
            bool isMusicMuted,
            float soundsVolume,
            float musicVolume) 
            : base(isSoundsMuted,
            isMusicMuted,
            soundsVolume,
            musicVolume)
        {
        }
    }
}