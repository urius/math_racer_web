using System;
using System.Collections.Generic;
using Data;
using Extensions;
using Infra.Instance;
using Providers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Controller
{
    public abstract class ControllerBase : IController
    {
        public Action<ControllerBase> DisposeRequested;
        
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

        protected void RequestDispose()
        {
            DisposeRequested?.Invoke(this);
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
            SubscribeOnChild(child);
        }

        private void SubscribeOnChild(ControllerBase child)
        {
            child.DisposeRequested += OnChildDisposeRequested;
        }
        
        private void UnsubscribeFromChild(ControllerBase child)
        {
            child.DisposeRequested -= OnChildDisposeRequested;
        }

        private void OnChildDisposeRequested(ControllerBase child)
        {
            UnsubscribeFromChild(child);

            _children.Remove(child);
            child.Dispose();
        }

        private void DisposeChildren()
        {
            if (_children != null)
            {
                foreach (var child in _children)
                {
                    UnsubscribeFromChild(child);
                    child.Dispose();
                }

                _children.Clear();
            }
        }
    }
}