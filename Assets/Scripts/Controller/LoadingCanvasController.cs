using UnityEngine;
using UnityEngine.UI;

namespace Controller
{
    public class LoadingCanvasController : ControllerBase
    {
        [SerializeField] private Image _image;
        
        public override void Initialize()
        {
            DontDestroyOnLoad(gameObject);
        }

        public override void Dispose()
        {
        }
    }
}