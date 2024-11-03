using UnityEngine;
using View.UI.RaceScene;

namespace Controller.RaceScene
{
    public class RaceSceneRootController : ControllerBase
    {
        private UIRaceSceneRootCanvasView _rootCanvasView;

        public override void Initialize()
        {
            _rootCanvasView = Object.FindObjectOfType<UIRaceSceneRootCanvasView>();
        }

        public override void Dispose()
        {
            _rootCanvasView = null;
        }
    }
}