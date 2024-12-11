using UnityEngine;

namespace View.Gameplay.Race.VFX
{
    public class CarParticleSystemVfxView : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystem;

        public void Play()
        {
            _particleSystem.Play();
        }

        public void Pause()
        {
            _particleSystem.Pause();
        }

        public void Stop()
        {
            _particleSystem.Stop();
        }
    }
}