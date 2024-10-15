using UnityEngine;

namespace Controller
{
    public abstract class ControllerBase : MonoBehaviour
    {
        public abstract void Initialize();
        public abstract void Dispose();

        private void Awake()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}