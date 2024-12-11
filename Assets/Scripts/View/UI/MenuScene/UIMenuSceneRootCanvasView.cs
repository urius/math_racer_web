using System;
using UnityEngine;
using View.UI.Common;

namespace View.UI.MenuScene
{
    public class UIMenuSceneRootCanvasView : MonoBehaviour
    {
        public event Action PlayButtonClicked
        {
            add => _playButton.ButtonClicked += value;
            remove => _playButton.ButtonClicked -= value;
        }
        public event Action CarsButtonClicked
        {
            add => _carsButton.ButtonClicked += value;
            remove => _carsButton.ButtonClicked -= value;
        }
        
        [SerializeField] private UITextButtonView _playButton;
        [SerializeField] private UITextButtonView _carsButton;
        [SerializeField] private RectTransform _popupsCanvasTransform;
        [SerializeField] private UIMenuSceneMoneyCanvasView _moneyCanvasView;
        
        public RectTransform PopupsCanvasTransform => _popupsCanvasTransform;
        public UIMenuSceneMoneyCanvasView MoneyCanvasView => _moneyCanvasView;

        public void SetPlayButtonText(string text)
        {
            _playButton.SetText(text);
        }

        public void SetCarsButtonText(string text)
        {
            _carsButton.SetText(text);
        }
    }
}