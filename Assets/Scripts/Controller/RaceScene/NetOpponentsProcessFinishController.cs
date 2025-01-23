using Infra.Instance;
using Model.RaceScene;
using Services;

namespace Controller.RaceScene
{
    public class NetOpponentsProcessFinishController : ControllerBase
    {
        private readonly IP2PRoomService _roomService = Instance.Get<IP2PRoomService>();
        
        private readonly NetRaceModel _netRaceModel;

        public NetOpponentsProcessFinishController(NetRaceModel netRaceModel)
        {
            _netRaceModel = netRaceModel;
        }

        public override void Initialize()
        {
            _roomService.OpponentFinishedReceived += OnOpponentFinishedReceived;
        }

        public override void DisposeInternal()
        {
            _roomService.OpponentFinishedReceived -= OnOpponentFinishedReceived;
        }

        private void OnOpponentFinishedReceived(OpponentFinishedReceivedEventPayload payload)
        {
            _netRaceModel.SetOpponentResult(payload.NetId,
                new NetOpponentRaceResult(
                    payload.Speed, payload.RaceTimeMs, payload.RightAnswersCount, payload.WrongAnswersCount));
        }
    }
}