using UnityEngine;

namespace View.UI.MenuScene
{
    public class UIMenuSceneRootView : MonoBehaviour
    {
        [SerializeField] private Transform _playerCarContainerTransform;

        public Transform PlayerCarContainerTransform => _playerCarContainerTransform;

        public void ClearPlayerCarContainerChildren()
        {
            foreach (Transform child in _playerCarContainerTransform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}