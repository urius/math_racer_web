using Infra.CommandExecutor;
using Infra.Instance;
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
            }
            else
            {
                audioPlayer.UnmuteBy(nameof(PerformGamePauseCommand));
            }
        }
    }
}