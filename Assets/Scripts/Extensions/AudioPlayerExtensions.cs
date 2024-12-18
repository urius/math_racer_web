using Data;
using Utils.AudioManager;

namespace Extensions
{
    public static class AudioPlayerExtensions
    {
        public static void PlaySound(this IAudioPlayer audioPlayer, SoundKey soundKey)
        {
            audioPlayer.PlaySound((int)soundKey);
        }
        
        public static void PlayButtonSound(this IAudioPlayer audioPlayer)
        {
            audioPlayer.PlaySound((int)SoundKey.Button_1);
        }
    }
}