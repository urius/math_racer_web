using System;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI.MenuScene
{
    public class UIMenuSceneDailyGiftButtonView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Sprite _greenSprite;
        [SerializeField] private Sprite _blueSprite;
        
        public event Action Clicked
        {
            add => _button.onClick.AddListener(value.Invoke);
            remove => _button.onClick.RemoveListener(value.Invoke);
        }

        public void SetVisibility(bool isVisible)
        {
            _button.gameObject.SetActive(isVisible);
        }

        public void SetGreenState()
        {
            _button.image.sprite = _greenSprite;
        }
        
        public void SetBlueState()
        {
            _button.image.sprite = _blueSprite;
        }
    }
}