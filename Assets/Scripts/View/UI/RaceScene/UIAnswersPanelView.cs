using System;
using UnityEngine;

namespace View.UI.RaceScene
{
    public class UIAnswersPanelView : MonoBehaviour
    {
        public event Action<int> AnswerClicked;
            
        [SerializeField] private UIAnswerButtonView[] _answerButtonViews;

        private void Awake()
        {
            foreach (var answerButtonView in _answerButtonViews)
            {
                answerButtonView.Clicked += OnAnswerClicked;
            }
        }

        private void OnDestroy()
        {
            foreach (var answerButtonView in _answerButtonViews)
            {
                answerButtonView.Clicked -= OnAnswerClicked;
            }
        }

        public void SetAnswerValue(int index, float value)
        {
            _answerButtonViews[index].SetText(value.ToString());
        }

        public void SetAnswersAmount(int amount)
        {
            for (var i = 0; i < _answerButtonViews.Length; i++)
            {
                SetAnswerButtonVisibility(i, i < amount);
            }
        }

        public void SetAnswerInteractable(int index, bool isInteractable)
        {
            _answerButtonViews[index].SetInteractable(isInteractable);
        }

        public void SetAnswerButtonVisibility(int index, bool isVisible)
        {
            _answerButtonViews[index].gameObject.SetActive(isVisible);
            _answerButtonViews[index].SetInteractable(true);
        }

        private void OnAnswerClicked(UIAnswerButtonView view)
        {
            var clickedIndex = Array.IndexOf(_answerButtonViews, view);
            
            AnswerClicked?.Invoke(clickedIndex);
        }
    }
}