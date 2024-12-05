using UnityEngine;

namespace Data.MonoBehaviourData
{
    public class CarMonoBehaviourData : MonoBehaviour
    {
        [SerializeField] private int _acceleration;
        [SerializeField] private int _maxSpeed;
        [SerializeField] private Sprite _iconSprite;
        [SerializeField] private int _price;
        [SerializeField] private int _unlockLevel;

        public int Acceleration => _acceleration;
        public int MaxSpeed => _maxSpeed;
        public Sprite IconSprite => _iconSprite;
        public int Price => _price;
        public int UnlockLevel => _unlockLevel;
    }
}