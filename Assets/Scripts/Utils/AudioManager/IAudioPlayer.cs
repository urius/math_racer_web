using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Utils.AudioManager
{
    public interface IAudioPlayer
    {
        public void SetSettings(IAudioSettingsModel audioSettingsModel);

        public void PlaySound(int soundId);

        public UniTask FadeInAndPlayMusicAsync(CancellationToken stopToken, AudioClip clip, float duration = 0.5f);
        public UniTask FadeOutAndStopMusicAsync(CancellationToken stopToken, float duration = 0.5f);
        
        void MuteBy(string muteRequesterId);
        void UnmuteBy(string unmuteRequesterId);
    }
}