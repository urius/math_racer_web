using UnityEngine;

namespace Controller
{
    public abstract class MonoBehaviourControllerBase : MonoBehaviour, IController
    {
        public abstract void Initialize();
        public abstract void Dispose();
    }
}