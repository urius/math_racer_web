using Infra.Instance;
using Model.RaceScene;
using Providers;
using Services;
using UnityEngine;
using View.Presenters;

namespace Controller.RaceScene
{
    public class NetRacePlayerCarController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly IP2PRoomService _roomService = Instance.Get<IP2PRoomService>();
        
        private readonly CarModel _carModel;
        private readonly RaceCarPresenter _carPresenter;
        
        private RaceModel _raceModel;

        public NetRacePlayerCarController(CarModel carModel, Transform targetTransform)
        {
            _carModel = carModel;
            _carPresenter = new RaceCarPresenter(carModel, targetTransform);
        }

        public override void Initialize()
        {
            _raceModel = _modelsHolder.GetRaceModel();
            _carPresenter.Present();
            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
            _carPresenter.Dispose();
        }
        
        private void Subscribe()
        {
            _raceModel.QuestionsModel.AnswerGiven += OnAnswerGiven;
            _raceModel.QuestionsModel.TurboActivated += OnTurboActivated;
            _raceModel.IsFinishedFlagChanged += OnIsFinishedFlagChanged;
        }

        private void Unsubscribe()
        {
            _raceModel.QuestionsModel.AnswerGiven -= OnAnswerGiven;
            _raceModel.QuestionsModel.TurboActivated -= OnTurboActivated;
            _raceModel.IsFinishedFlagChanged -= OnIsFinishedFlagChanged;
        }

        private void OnAnswerGiven(int answerIndex, bool isRightAnswer)
        {
            if (isRightAnswer)
            {
                _roomService.SendAccelerate();
                _carModel.Accelerate();
            }
            else
            {
                _roomService.SendDecelerate();
                _carModel.Decelerate();
            }
        }

        private void OnTurboActivated()
        {
            _roomService.SendAccelerateTurbo();
            _carModel.AccelerateTurbo();
        }

        private void OnIsFinishedFlagChanged(bool flagValue)
        {
            if (flagValue)
            {
                var raceResultsModel = _raceModel.RaceResultsModel;
                _roomService.SendFinished(
                    raceResultsModel.PlayerSpeed,
                    raceResultsModel.RaceTimeSec,
                    raceResultsModel.RightAnswersCount,
                    raceResultsModel.WrongAnswersCount);
            }
        }
    }
}