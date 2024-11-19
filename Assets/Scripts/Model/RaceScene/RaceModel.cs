using Data;

namespace Model.RaceScene
{
    public class RaceModel
    {
        public RaceModel(CarKey playerCarKey, CarKey botCarKey, ComplexityData complexityData)
        {
            QuestionsModel = new QuestionsModel(complexityData);
            PlayerCar = new CarModel(playerCarKey);
            BotCar = new CarModel(botCarKey);
        }

        public QuestionsModel QuestionsModel { get; }
        public CarModel PlayerCar { get; }
        public CarModel BotCar { get; }
    }
}