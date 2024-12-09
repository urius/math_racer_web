using System.Collections.Generic;
using Providers;
using UnityEngine;
using UnityEngine.Assertions;
using Utils.GameObjectsCache;

namespace Infra.Controller
{
    public abstract class ControllerBase
    {
        private IGameObjectsCache _gameObjectsCache;
        private LinkedList<ControllerBase> _children;
        private PrefabsHolderSo _prefabsHolder;

        protected Transform TargetTransform { get; private set; }

        public virtual void Start(Transform transform)
        {
            TargetTransform = transform;

            StartInternal();
        }
        
        public virtual void Stop()
        {
            if (_children != null)
            {
                foreach (var childController in _children)
                {
                    childController.Stop();
                }
                _children.Clear();
            }

            StopInternal();
        }
        
        protected abstract void StartInternal();
        protected abstract void StopInternal();

        protected T StartChildController<T>(Transform transform) where T : ControllerBase, new()
        {
            Assert.IsNotNull(transform);
            
            var controller = new T();
            controller.Start(transform);
            
            AddChildController(controller);

            return controller;
        }

        protected T StartChildController<T, TModel>(Transform transform, TModel model)
            where TModel : class
            where T : ControllerWithModelBase<TModel>, new()
        {
            Assert.IsNotNull(transform);
            Assert.IsNotNull(model);

            var controller = new T();
            controller.Mediate(transform, model);

            AddChildController(controller);

            return controller;
        }

        protected ControllerBase StartChildController(ControllerBase controller, Transform transform)
        {
            Assert.IsNotNull(transform);
            
            controller.Start(transform);
            
            AddChildController(controller);

            return controller;
        }
        
        protected ControllerBase StartChildController(ControllerBase controller)
        {
            return StartChildController(controller, TargetTransform);
        }

        protected bool RemoveChildController(ControllerBase childController)
        {
            if (_children.Contains(childController))
            {
                childController.Stop();
                _children.Remove(childController);

                return true;
            }

            return false;
        }

        protected void AddChildController(ControllerBase controller)
        {
            _children ??= new LinkedList<ControllerBase>();
            _children.AddLast(controller);
        }
    }
}