using Cysharp.Threading.Tasks;
using Data;
using Extensions;
using Infra.Instance;
using Model.RaceScene;
using Providers;
using UnityEngine;
using Utils.AudioManager;
using View;

namespace Controller.RaceScene
{
    public class RaceScenePlayerCarController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        
        private readonly CarModel _carModel;
        private readonly Transform _targetTransform;
        private readonly SoundKey[] _accelerationSounds = new[]
        {
            SoundKey.Acceleration_1,
            SoundKey.Acceleration_2,
            SoundKey.Acceleration_3,
            SoundKey.Acceleration_4,
            SoundKey.Acceleration_5
        };
        private readonly SoundKey[] _brakesSounds = new[]
        {
            SoundKey.Brakes_1,
            SoundKey.Brakes_2,
            SoundKey.Brakes_3,
        };

        private float _targetSpeed;
        private CarView _carView;
        private RaceModel _raceModel;

        public RaceScenePlayerCarController(CarModel carModel, Transform targetTransform)
        {
            _carModel = carModel;
            _targetTransform = targetTransform;
        }
        
        public override void Initialize()
        {
            _raceModel = _modelsHolder.GetRaceModel();
            
            _carView = Instantiate<CarView>(_carModel.CarKey.ToPrefabKey(), _targetTransform);

            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
            
            Destroy(_carView);
            _carView = null;
        }

        private void Subscribe()
        {
            _updatesProvider.GameplayUpdate += OnGameplayUpdate;
            
            _raceModel.QuestionsModel.AnswerGiven += OnAnswerGiven;
            _raceModel.QuestionsModel.TurboActivated += OnTurboActivated;
            _carModel.TurboFlag.ValueChanged += OnTurboFlagValueChanged;
        }

        private void Unsubscribe()
        {
            _updatesProvider.GameplayUpdate -= OnGameplayUpdate;
            
            _raceModel.QuestionsModel.AnswerGiven -= OnAnswerGiven;
            _raceModel.QuestionsModel.TurboActivated -= OnTurboActivated;
            _carModel.TurboFlag.ValueChanged -= OnTurboFlagValueChanged;
        }

        private void OnTurboFlagValueChanged(bool value, bool _)
        {
            if (value)
            {
                _carView.ShowBoostVFX();
                _audioPlayer.PlaySound(SoundKey.Turbo);
            }
            else
            {
                UniTask.Delay(800)
                    .ContinueWith(_carView.StopBoostVFX);
            }
        }

        private void OnTurboActivated()
        {
            _carModel.AccelerateTurbo();
        }

        private void OnAnswerGiven(int answerIndex, bool isRightAnswer)
        {
            if (isRightAnswer)
            {
                _carModel.Accelerate();
                
                PlayAccelerationSound();
            }
            else
            {
                _carModel.Decelerate();

                if (_raceModel.QuestionsModel.RightAnswersCountTotal > 0)
                {
                    PlayBrakesSound();
                }
            }
        }

        private void PlayAccelerationSound()
        {
            var index = Random.Range(0, _accelerationSounds.Length);
            var sound = _accelerationSounds[index];
            _audioPlayer.PlaySound(sound);
        }

        private void PlayBrakesSound()
        {
            var index = Random.Range(0, _brakesSounds.Length);
            var sound = _brakesSounds[index];
            _audioPlayer.PlaySound(sound);
        }

        private void OnGameplayUpdate()
        {
            UpdateCarView();
        }
        
        private void UpdateCarView()
        {
            var deltaWheelRotation = _carModel.CurrentUpdateMetersPassed * _carView.WheelRotationMultiplier;
            
            _carView.SetBodyRotation(_carModel.CurrentBodyRotation);
            _carView.SetXOffset(_carModel.XOffset);
            _carView.RotateWheels(deltaWheelRotation);
        }
    }
}