namespace Controller
{
    public abstract class ControllerBase : IController
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