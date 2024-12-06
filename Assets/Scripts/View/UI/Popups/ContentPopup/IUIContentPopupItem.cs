using UnityEngine;

namespace View.UI.Popups.ContentPopup
{
    public interface IUIContentPopupItem
    {
        public RectTransform RectTransform { get; }
        public Vector2 Size { get; }
    }
}