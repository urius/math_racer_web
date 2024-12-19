using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI.RaceScene
{
    public class UITopPanelCanvasView : MonoBehaviour
    {
        public event Action SettingsButtonClicked;
        
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private UIRaceSceneRaceSchemaView _raceSchemaView;

        public UIRaceSceneRaceSchemaView RaceSchemaView => _raceSchemaView;

        private void Awake()
        {
            _settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        }

        private void OnSettingsButtonClicked()
        {
            SettingsButtonClicked?.Invoke();
        }

        public void SetText(string text)
        {
            _text.text = text;
        }
    }
}