using Cysharp.Threading.Tasks;
using Data;
using Extensions;
using Infra.Instance;
using Model.RaceScene;
using Providers;
using UnityEngine;
using Utils.AudioManager;

namespace View.Presenters
{
    public class RaceCarPresenter : PresenterBase
    {
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        
        private readonly CarModel _carModel;
        private readonly Transform _targetTransform;
        private readonly bool _muteSounds;

        private readonly SoundKey[] _accelerationSounds = {
            SoundKey.Acceleration_1,
            SoundKey.Acceleration_2,
            SoundKey.Acceleration_3,
            SoundKey.Acceleration_4,
            SoundKey.Acceleration_5
        };
        private readonly SoundKey[] _brakesSounds = {
            SoundKey.Brakes_1,
            SoundKey.Brakes_2,
            SoundKey.Brakes_3,
        };

        private CarView _carView;

        public RaceCarPresenter(CarModel carModel, Transform targetTransform, bool muteSounds = false)
        {
            _carModel = carModel;
            _targetTransform = targetTransform;
            _muteSounds = muteSounds;
        }
        
        public override void Present()
        {
            _carView = Instantiate<CarView>(_carModel.CarKey.ToPrefabKey(), _targetTransform);
            
            Subscribe();
        }

        public override void Dispose()
        {
            Unsubscribe();
            
            Destroy(_carView);
            _carView = null;
        }

        private void Subscribe()
        {
            _updatesProvider.GameplayUpdate += OnGameplayUpdate;

            _carModel.AccelerateHappened += OnAccelerate;
            _carModel.DecelerateHappened += OnDecelerate;
            _carModel.TurboFlag.ValueChanged += OnTurboFlagValueChanged;
        }

        private void Unsubscribe()
        {
            _updatesProvider.GameplayUpdate -= OnGameplayUpdate;

            _carModel.AccelerateHappened -= OnAccelerate;
            _carModel.DecelerateHappened -= OnDecelerate;
            _carModel.TurboFlag.ValueChanged -= OnTurboFlagValueChanged;
        }

        private void OnAccelerate()
        {
            PlayAccelerationSound();
        }

        private void OnDecelerate()
        {
            if (_carModel.CurrentSpeedKmph > 0)
            {
                PlayBrakesSound();
            }
        }

        private void PlayAccelerationSound()
        {
            var index = Random.Range(0, _accelerationSounds.Length);
            var sound = _accelerationSounds[index];
            PlaySound(sound);
        }

        private void PlayBrakesSound()
        {
            var index = Random.Range(0, _brakesSounds.Length);
            var sound = _brakesSounds[index];
            PlaySound(sound);
        }

        private void OnTurboFlagValueChanged(bool value, bool _)
        {
            if (value)
            {
                _carView.ShowBoostVFX();
                PlaySound(SoundKey.Turbo);
            }
            else
            {
                UniTask.Delay(800)
                    .ContinueWith(_carView.StopBoostVFX);
            }
        }

        private void PlaySound(SoundKey key)
        {
            if (_muteSounds) return;
            _audioPlayer.PlaySound(key);
        }

        private void OnGameplayUpdate()
        {
            UpdateCarView();
        }
        
        private void UpdateCarView()
        {
            var deltaWheelRotation = _carModel.CurrentUpdateMetersPassed * _carView.WheelRotationMultiplier;
            
            _carView.SetBodyRotation(_carModel.CurrentBodyRotation);
            _carView.RotateWheels(deltaWheelRotation);
            
            _carView.SetXOffset(_carModel.XOffset);
        }
    }
}