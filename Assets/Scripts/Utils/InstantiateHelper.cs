using Data;
using Infra.Instance;
using Providers;
using UnityEngine;

namespace Utils
{
    public static class InstantiateHelper
    {
        private static PrefabsHolderSo _prefabsHolder;

        public static T InstantiatePrefab<T>(PrefabKey prefabKey, Transform transform)
            where T : MonoBehaviour
        {
            var go = InstantiatePrefab(prefabKey, transform);
            var result = go.GetComponent<T>();
            return result;
        }

        public static GameObject InstantiatePrefab(PrefabKey prefabKey, Transform transform)
        {
            return Instantiate(GetPrefabByKey(prefabKey), transform);
        }

        public static GameObject Instantiate(GameObject prefab, Transform transform)
        {
            return Object.Instantiate(prefab, transform);
        }

        public static GameObject GetPrefabByKey(PrefabKey prefabKey)
        {
            var prefabsHolder = Instance.Get<PrefabsHolderSo>();

            return prefabsHolder.GetPrefabByKey(prefabKey);
        }

        public static GameObject InstantiateColdPrefab(string path, Transform targetTransform)
        {
            _prefabsHolder ??= Instance.Get<PrefabsHolderSo>();

            var prefab = _prefabsHolder.GetColdPrefab(path);
            if (prefab != null)
            {
                return Instantiate(prefab, targetTransform);
            }

            return null;
        }

        public static TView InstantiateColdPrefab<TView>(string path, Transform targetTransform)
            where TView : MonoBehaviour
        {
            var go = InstantiateColdPrefab(path, targetTransform);

            if (go != null)
            {
                return go.GetComponent<TView>();
            }

            return null;
        }

        public static void Destroy(MonoBehaviour monoBehaviour)
        {
            Destroy(monoBehaviour.gameObject);
        }

        public static void Destroy(GameObject gameObject)
        {
            Object.Destroy(gameObject);
        }
    }
}