using Data;

namespace Model.RaceScene
{
    public class RaceModel
    {
        public RaceModel(CarKey playerCarKey)
        {
            QuestionsModel = new QuestionsModel();
            PlayerCar = new CarModel(playerCarKey);
        }

        public QuestionsModel QuestionsModel { get; }
        public CarModel PlayerCar { get; }
    }
}