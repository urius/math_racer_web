using TMPro;
using UnityEngine;

namespace View.UI.RaceScene
{
    public class UITopPanelCanvasView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        public void SetText(string text)
        {
            _text.text = text;
        }
    }
}