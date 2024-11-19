using System.Collections.Generic;
using Data;
using Extensions;
using Holders;
using Infra.Instance;
using UnityEngine;

namespace Controller
{
    public abstract class ControllerBase : IController
    {
        public abstract void Initialize();
        public abstract void DisposeInternal();

        private LinkedList<ControllerBase> _children;
        
        private static IPrefabHolder PrefabHolder => Instance.Get<IPrefabHolder>();

        private void Awake()
        {
            Initialize();
        }

        public void Dispose()
        {
            DisposeChildren();
            DisposeInternal();
        }

        protected TView Instantiate<TView>(PrefabKey prefabKey, Transform targetTransform)
            where TView : MonoBehaviour
        {
            return PrefabHolder.Instantiate<TView>(prefabKey, targetTransform);
        }

        protected void Destroy(MonoBehaviour view)
        {
            Object.Destroy(view.gameObject);
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