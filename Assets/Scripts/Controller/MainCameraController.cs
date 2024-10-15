using UnityEngine;

namespace Controller
{
    public class MainCameraController : ControllerBase
    {
        [SerializeField] private Camera _camera;
        
        public override void Initialize()
        {
        }

        public override void Dispose()
        {
        }
    }
}