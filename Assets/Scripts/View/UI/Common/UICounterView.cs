using TMPro;
using UnityEngine;

namespace View.UI.Common
{
    public class UICounterView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        public void SetVisibility(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void SetCounterText(string text)
        {
            _text.text = text;
        }
    }
}