using UnityEngine;

namespace Controller.MenuScene
{
    public class MenuSceneCarsPopupController : ControllerBase
    {
        private readonly RectTransform _targetTransform;

        public MenuSceneCarsPopupController(RectTransform targetTransform)
        {
            _targetTransform = targetTransform;
        }

        public override void Initialize()
        {
            
        }

        public override void DisposeInternal()
        {
        }
    }
}