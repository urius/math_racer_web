using System;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI
{
    public class UIMenuSceneRootCanvasView : MonoBehaviour
    {
        public event Action PlayButtonClicked;
        
        [SerializeField] private Button _playButton;

        private void Awake()
        {
            _playButton.onClick.AddListener(OnPlayButtonClicked);
        }

        private void OnPlayButtonClicked()
        {
            PlayButtonClicked?.Invoke();
        }
    }
}