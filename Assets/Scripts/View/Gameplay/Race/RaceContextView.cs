using UnityEngine;
using View.UI.RaceScene;

namespace View.Gameplay.Race
{
    public class RaceContextView : MonoBehaviour
    {
        [SerializeField] private Transform _playerCarTargetTransform;
        [SerializeField] private Transform _opponentCarTargetTransform;
        [SerializeField] private Transform _opponent2CarTargetTransform;
        [SerializeField] private Transform _startLineTransform;
        [SerializeField] private Transform _finishLineTransform;
        [SerializeField] private UIRaceSceneRootCanvasView _rootCanvasView;
        [SerializeField] private BgContainerView _bgContainerView;
        [SerializeField] private RoadContainerView _roadContainerView;
        [SerializeField] private TrafficLightView _trafficLightView;

        public Transform PlayerCarTargetTransform => _playerCarTargetTransform;
        public Transform OpponentCarTargetTransform => _opponentCarTargetTransform;
        public Transform Opponent2CarTargetTransform => _opponent2CarTargetTransform;
        public Transform StartLineTransform => _startLineTransform;
        public Transform FinishLineTransform => _finishLineTransform;
        public UIRaceSceneRootCanvasView RootCanvasView => _rootCanvasView;
        public BgContainerView BgContainerView => _bgContainerView;
        public RoadContainerView RoadContainerView => _roadContainerView;
        public TrafficLightView TrafficLightView => _trafficLightView;
    }
}