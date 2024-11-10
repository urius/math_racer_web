using UnityEngine;

namespace View.UI.RaceScene
{
    public class UIRaceSceneRootCanvasView : MonoBehaviour
    {
        [SerializeField] private UIRightPanelView _rightPanelView;
        [SerializeField] private UITopPanelCanvasView _topPanelCanvasView;

        public UIRightPanelView RightPanelView => _rightPanelView;
        public UITopPanelCanvasView TopPanelCanvasView => _topPanelCanvasView;
    }
}