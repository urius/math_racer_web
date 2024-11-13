using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI.RaceScene
{
    public class UIAnswerButtonView : MonoBehaviour
    {
        public event Action<UIAnswerButtonView> Clicked;
        
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Image _bgImage;
        [SerializeField] private Color _rightAnswerColor;
        [SerializeField] private Color _wrongAnswerColor;
        
        private Color _defaultColor;

        private void Awake()
        {
            _defaultColor = _bgImage.color;
            _wrongAnswerColor.a = _rightAnswerColor.a = _defaultColor.a;
            
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void SetInteractable(bool isInteractable)
        {
            _button.interactable = isInteractable;
        }

        public void ToDefaultState()
        {
            _bgImage.color = _defaultColor;
            SetInteractable(true);
        }

        public void ToRightAnswerState()
        {
            _bgImage.color = _rightAnswerColor;
            SetInteractable(false);
        }

        public void ToWrongAnswerState()
        {
            _bgImage.color = _wrongAnswerColor;
            SetInteractable(false);
        }

        private void OnButtonClick()
        {
            Clicked?.Invoke(this);
        }
    }
}