using System;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI.MenuScene
{
    public class UIMenuSceneRootCanvasView : MonoBehaviour
    {
        public event Action PlayButtonClicked;
        public event Action CarsButtonClicked;
        
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _carsButton;

        private void Awake()
        {
            _playButton.onClick.AddListener(OnPlayButtonClicked);
            _carsButton.onClick.AddListener(OnCarsButtonClicked);
        }

        private void OnPlayButtonClicked()
        {
            PlayButtonClicked?.Invoke();
        }

        private void OnCarsButtonClicked()
        {
            CarsButtonClicked?.Invoke();
        }
    }
}