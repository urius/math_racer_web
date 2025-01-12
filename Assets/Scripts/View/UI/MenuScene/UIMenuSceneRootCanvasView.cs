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
        public event Action MultiplayerButtonClicked
        {
            add => _multiplayerButton.ButtonClicked += value;
            remove => _multiplayerButton.ButtonClicked -= value;
        }
        public event Action CarsButtonClicked
        {
            add => _carsButton.ButtonClicked += value;
            remove => _carsButton.ButtonClicked -= value;
        }
        public event Action SettingsButtonClicked
        {
            add => _settingsButton.ButtonClicked += value;
            remove => _settingsButton.ButtonClicked -= value;
        }
        
        [SerializeField] private UITextButtonView _playButton;
        [SerializeField] private UITextButtonView _multiplayerButton;
        [SerializeField] private UITextButtonView _carsButton;
        [SerializeField] private UITextButtonView _settingsButton;
        [SerializeField] private RectTransform _popupsCanvasTransform;
        [SerializeField] private UIMenuSceneMoneyCanvasView _moneyCanvasView;
        [SerializeField] private UIMenuSceneLevelCanvasView _levelCanvasView;
        
        public RectTransform PopupsCanvasTransform => _popupsCanvasTransform;
        public UIMenuSceneMoneyCanvasView MoneyCanvasView => _moneyCanvasView;
        public UIMenuSceneLevelCanvasView LevelCanvasView => _levelCanvasView;

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