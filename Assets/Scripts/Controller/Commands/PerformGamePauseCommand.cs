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
            Debug.Log("PerformGamePauseCommand, needPause: " + needPause);
            
            var audioPlayer = Instance.Get<IAudioPlayer>();

            if (needPause)
            {
                audioPlayer.MuteBy(nameof(PerformGamePauseCommand));
            }
            else
            {
                audioPlayer.UnmuteBy(nameof(PerformGamePauseCommand));
            }
        }
    }
}