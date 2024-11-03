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

        private void Awake()
        {
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

        private void OnButtonClick()
        {
            Clicked?.Invoke(this);
        }
    }
}