using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI.Common
{
    public class UISliderContainerView : MonoBehaviour
    {
        public event Action<float> SliderValueChanged;
        
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Slider _slider;

        public float SliderValue => _slider.value;
        
        private void Awake()
        {
            _slider.onValueChanged.AddListener(OnValueChangedHandler);
        }

        private void OnDestroy()
        {
            _slider.onValueChanged.RemoveAllListeners();
        }

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void SetSliderValue(float value)
        {
            _slider.value = value;
        }

        private void OnValueChangedHandler(float value)
        {
            SliderValueChanged?.Invoke(value);
        }
    }
}