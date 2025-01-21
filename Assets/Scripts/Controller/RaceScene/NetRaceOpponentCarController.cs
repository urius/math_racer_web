using Infra.Instance;
using Model.RaceScene;
using Services;
using UnityEngine;
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
            _presenter = new RaceCarPresenter(carModel, targetTransform, muteSounds: true);
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
            _roomService.AccelerateReceived += OnAccelerateReceived;
            _roomService.AccelerateTurboReceived += OnAccelerateTurboReceived;
            _roomService.DecelerateReceived += OnDecelerateReceived;
        }

        private void Unsubscribe()
        {
            _roomService.AccelerateReceived -= OnAccelerateReceived;
            _roomService.AccelerateTurboReceived -= OnAccelerateTurboReceived;
            _roomService.DecelerateReceived -= OnDecelerateReceived;
        }

        private void OnAccelerateReceived(int id, long timestamp)
        {
            if (_carModel.NetId == id)
            {
                _carModel.Accelerate();
            }
        }

        private void OnAccelerateTurboReceived(int id, long timestamp)
        {
            if (_carModel.NetId == id)
            {
                _carModel.AccelerateTurbo();
            }
        }

        private void OnDecelerateReceived(int id, long timestamp)
        {
            if (_carModel.NetId == id)
            {
                _carModel.Decelerate();
            }
        }
    }
}