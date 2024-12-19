using UnityEngine;

namespace View.UI.RaceScene
{
    public class UIRaceSceneRootCanvasView : MonoBehaviour
    {
        [SerializeField] private UIRightPanelView _rightPanelView;
        [SerializeField] private UITopPanelCanvasView _topPanelCanvasView;
        [SerializeField] private RectTransform _popupsCanvasTransform;

        public UIRightPanelView RightPanelView => _rightPanelView;
        public UITopPanelCanvasView TopPanelCanvasView => _topPanelCanvasView;
        public RectTransform PopupsCanvasTransform => _popupsCanvasTransform;
    }
}