using System.Collections;
using UnityEngine;

namespace View.Gameplay.Race.CarExtras
{
    public class PoliceCarLampView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            StartCoroutine(ToggleSpriteRenderer());
        }
        
        private IEnumerator ToggleSpriteRenderer()
        {
            while (true)
            {
                _spriteRenderer.enabled = !_spriteRenderer.enabled;
                yield return new WaitForSeconds(0.25f);
            }
        }
    }
}