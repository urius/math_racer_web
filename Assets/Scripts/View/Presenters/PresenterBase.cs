using System;
using Data;
using Extensions;
using Infra.Instance;
using Providers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace View.Presenters
{
    public abstract class PresenterBase : IDisposable
    {
        private static IPrefabHolder PrefabHolder => Instance.Get<IPrefabHolder>();

        public abstract void Present();
        public abstract void Dispose();
        
        protected TView Instantiate<TView>(PrefabKey prefabKey, Transform targetTransform)
            where TView : MonoBehaviour
        {
            return PrefabHolder.Instantiate<TView>(prefabKey, targetTransform);
        }
        
        protected void Destroy(MonoBehaviour view)
        {
            Object.Destroy(view.gameObject);
        }
    }
}