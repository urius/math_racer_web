using UnityEngine;

namespace View.UI.RaceScene
{
    public class UIRaceSceneRootCanvasView : MonoBehaviour
    {
        [SerializeField] private UIRightPanelView _rightPanelView;

        public UIRightPanelView RightPanelView => _rightPanelView;
    }
}