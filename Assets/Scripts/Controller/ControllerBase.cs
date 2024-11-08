using System.Collections.Generic;

namespace Controller
{
    public abstract class ControllerBase : IController
    {
        public abstract void Initialize();
        public abstract void DisposeInternal();

        private LinkedList<ControllerBase> _children;

        private void Awake()
        {
            Initialize();
        }

        public void Dispose()
        {
            DisposeChildren();
            DisposeInternal();
        }
        
        protected void InitChildController(ControllerBase child)
        {
            child.Initialize();
            AddChildController(child);
        }

        protected void AddChildController(ControllerBase child)
        {
            _children ??= new LinkedList<ControllerBase>();

            _children.AddLast(child);
        }

        private void DisposeChildren()
        {
            if (_children != null)
            {
                foreach (var child in _children)
                {
                    child.Dispose();
                }

                _children.Clear();
            }
        }
    }
}