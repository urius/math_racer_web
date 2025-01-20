using Infra.Instance;
using Model.RaceScene;
using Services;
using UnityEngine;
using Utils.P2PLib;
using View.Presenters;

namespace Controller.RaceScene
{
    public class NetRaceOpponentCarController : ControllerBase
    {
        private readonly IP2PRoomService _roomService = Instance.Get<IP2PRoomService>();
        
        private readonly CarModel _carModel;
        private readonly RaceCarPresenter _presenter;

        public NetRaceOpponentCarController(CarModel carModel, Transform targetTransform)
        {
            _carModel = carModel;
            _presenter = new RaceCarPresenter(carModel, targetTransform);
        }

        public override void Initialize()
        {
            _presenter.Present();
            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
            _presenter.Dispose();
        }

        private void Subscribe()
        {
            _roomService.MessageReceived += OnMessageReceived;
        }

        private void Unsubscribe()
        {
            _roomService.MessageReceived -= OnMessageReceived;
        }

        private void OnMessageReceived(IP2PConnection _, string message)
        {
            
        }
    }
}