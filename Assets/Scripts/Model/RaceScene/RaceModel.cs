using Data;

namespace Model.RaceScene
{
    public class RaceModel
    {
        public RaceModel(CarKey playerCarKey, ComplexityData complexityData)
        {
            QuestionsModel = new QuestionsModel(complexityData);
            PlayerCar = new CarModel(playerCarKey);
        }

        public QuestionsModel QuestionsModel { get; }
        public CarModel PlayerCar { get; }
    }
}