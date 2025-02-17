using System;
using UnityEngine;
using UnityEngine.UI;
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
            add => _settingsButton.onClick.AddListener(value.Invoke);
            remove => _settingsButton.onClick.RemoveListener(value.Invoke);
        }
        public event Action LeaderBoardButtonClicked
        {
            add => _leaderBoardButton.onClick.AddListener(value.Invoke);
            remove => _leaderBoardButton.onClick.RemoveListener(value.Invoke);
        }

        [SerializeField] private UITextButtonView _playButton;
        [SerializeField] private UITextButtonView _multiplayerButton;
        [SerializeField] private UITextButtonView _carsButton;
        [SerializeField] private RectTransform _popupsCanvasTransform;
        [SerializeField] private UIMenuSceneMoneyCanvasView _moneyCanvasView;
        [SerializeField] private UIMenuSceneLevelCanvasView _levelCanvasView;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private UIMenuSceneDailyGiftButtonView _dailyGiftButton;
        [SerializeField] private Button _leaderBoardButton;
        
        public RectTransform PopupsCanvasTransform => _popupsCanvasTransform;
        public UIMenuSceneMoneyCanvasView MoneyCanvasView => _moneyCanvasView;
        public UIMenuSceneLevelCanvasView LevelCanvasView => _levelCanvasView;
        public UIMenuSceneDailyGiftButtonView DailyGiftButton => _dailyGiftButton;

        private void OnDestroy()
        {
            _settingsButton.onClick.RemoveAllListeners();
        }

        public void SetLeaderBoardButtonVisibility(bool isVisible)
        {
            _leaderBoardButton.gameObject.SetActive(isVisible);
        }

        public void SetMultiplayerButtonVisibility(bool isVisible)
        {
            _multiplayerButton.gameObject.SetActive(isVisible);
        }

        public void SetSettingsButtonInteractable(bool isInteractable)
        {
            _settingsButton.interactable = isInteractable;
        }
    }
}