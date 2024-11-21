using UnityEngine;

namespace View.Gameplay.Race
{
    public class TrafficLightView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] _lights;

        public int LightsCount => _lights.Length;
        
        private void Awake()
        {
            for (var i = 0; i < _lights.Length; i++)
            {
                _lights[i].gameObject.SetActive(false);
            }
        }

        public void SetLightGreen(int lightIndex)
        {
            SetLightColor(lightIndex, Color.green);
        }

        public void SetLightRed(int lightIndex)
        {
            SetLightColor(lightIndex, Color.red);
        }

        private void SetLightColor(int lightIndex, Color color)
        {
            _lights[lightIndex].gameObject.SetActive(true);
            _lights[lightIndex].color = color;
        }
    }
}