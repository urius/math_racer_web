using UnityEngine;
using View.UI.RaceScene;

namespace View.Gameplay.Race
{
    public class RaceContextView : MonoBehaviour
    {
        [SerializeField] private Transform _playerCarTargetTransform;
        [SerializeField] private UIRaceSceneRootCanvasView _rootCanvasView;
        [SerializeField] private BgContainerView _bgContainerView;
        [SerializeField] private RoadContainerView _roadContainerView;

        public Transform PlayerCarTargetTransform => _playerCarTargetTransform;
        public UIRaceSceneRootCanvasView RootCanvasView => _rootCanvasView;
        public BgContainerView BgContainerView => _bgContainerView;
        public RoadContainerView RoadContainerView => _roadContainerView;
    }
}