using System;

namespace Model.RaceScene
{
    public class QuestionsModel
    {
        public event Action RightAnswerGiven;
        public event Action WrongAnswerGiven;

        public virtual void DispatchRightAnswerGiven()
        {
            RightAnswerGiven?.Invoke();
        }

        public virtual void DispatchWrongAnswerGiven()
        {
            WrongAnswerGiven?.Invoke();
        }
    }
}