using UnityEngine;
using View.UI.RaceScene;

namespace View.Gameplay.Race
{
    public class RaceContextView : MonoBehaviour
    {
        [SerializeField] private Transform _playerCarTargetTransform;
        [SerializeField] private UIRaceSceneRootCanvasView _rootCanvasView;

        public Transform PlayerCarTargetTransform => _playerCarTargetTransform;
        public UIRaceSceneRootCanvasView RootCanvasView => _rootCanvasView;
    }
}