using Data;
using Providers;
using UnityEngine;

namespace Extensions
{
    public static class PrefabsHolderExtensions
    {
        public static TView Instantiate<TView>(this IPrefabHolder prefabHolder, PrefabKey prefabKey, Transform parent)
            where TView : MonoBehaviour
        {
            var prefab = prefabHolder.GetPrefabByKey(prefabKey);
            var go = Object.Instantiate(prefab, parent);
            var view = go.GetComponent<TView>();

            return view;
        }
    }
}