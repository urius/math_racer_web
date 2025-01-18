using Infra.Instance;
using Model.RaceScene;
using Providers;
using UnityEngine;
using View.Presenters;

namespace Controller.RaceScene
{
    public class RaceScenePlayerCarController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        
        private readonly CarModel _carModel;
        private readonly RaceCarPresenter _carPresenter;

        private float _targetSpeed;
        private RaceModel _raceModel;

        public RaceScenePlayerCarController(CarModel carModel, Transform targetTransform)
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
        }

        private void Unsubscribe()
        {
            _raceModel.QuestionsModel.AnswerGiven -= OnAnswerGiven;
            _raceModel.QuestionsModel.TurboActivated -= OnTurboActivated;
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
            }
            else
            {
                _carModel.Decelerate();
            }
        }
    }
}