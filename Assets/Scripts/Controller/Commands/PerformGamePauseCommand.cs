using Data;
using Infra.CommandExecutor;
using Infra.Instance;
using UnityEngine;
using Utils.AudioManager;

namespace Controller.Commands
{
    public struct PerformGamePauseCommand : ICommand<bool>
    {
        public void Execute(bool needPause)
        {
            var audioPlayer = Instance.Get<IAudioPlayer>();

            if (needPause)
            {
                audioPlayer.MuteBy(nameof(PerformGamePauseCommand));
                Application.targetFrameRate = 1;
            }
            else
            {
                Application.targetFrameRate = Constants.FPS;
                audioPlayer.UnmuteBy(nameof(PerformGamePauseCommand));
            }
        }
    }
}