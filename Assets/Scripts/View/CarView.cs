using System;
using Data;
using UnityEngine;
using View.Gameplay.Race.VFX;

namespace View
{
    public class CarView : MonoBehaviour
    {
        [SerializeField] private Transform _carBody;
        [SerializeField] private Transform _wheelBack;
        [SerializeField] private Transform _wheelFront;
        [SerializeField] private Transform _boosterContainer;

        [SerializeField] private float _bodyRotationMin = -5;
        [SerializeField] private float _bodyRotationMax = 5;
        
        private GameObject _boosterPrefab;
        private GameObject _exhaustPrefab;
        
        private Vector3 _wheelSize;
        private float _wheelRadius;
        private float _wheelRotationMultiplier;
        private Transform _transform;
        private CarParticleSystemVfxView _turboBoosterVfxView;
        private CarParticleSystemVfxView _exhaustVfxView;

        public float WheelRotationMultiplier => _wheelRotationMultiplier;

        private void Awake()
        {
            _transform = transform;
            _wheelSize = _wheelBack.GetComponent<SpriteRenderer>().bounds.size;

            _wheelRadius = _wheelSize.x * 0.5f;

            _wheelRotationMultiplier = 360 / (_wheelRadius * (float)Math.PI * 2);
        }

        public void RotateWheels(float degrees)
        {
            _wheelBack.Rotate(0, 0, -degrees);
            _wheelFront.Rotate(0, 0, -degrees);
        }

        public void SetBodyRotation(float bodyRotation)
        {
            var bodyTransform = _carBody.transform;
            
            var eulerAngles = bodyTransform.eulerAngles;
            eulerAngles.z = -Math.Clamp(bodyRotation, _bodyRotationMin, _bodyRotationMax);
            bodyTransform.eulerAngles = eulerAngles;
        }

        public void SetXOffset(float xOffset)
        {
            var pos = _transform.localPosition;
            pos.x = xOffset;
            _transform.localPosition = pos;
        }

        public void ShowBoostVFX()
        {
            if (_turboBoosterVfxView == null)
            {
                _boosterPrefab ??= Resources.Load(Constants.PrefabPathTurboBooster) as GameObject;
                var go = Instantiate(_boosterPrefab, _boosterContainer);
                _turboBoosterVfxView = go.GetComponent<CarParticleSystemVfxView>();
            }
            
            _turboBoosterVfxView.gameObject.SetActive(true);
            
            _turboBoosterVfxView.Play();
        }

        public void ShowExhaustVFX()
        {
            if (_exhaustVfxView == null)
            {
                _exhaustPrefab ??= Resources.Load(Constants.PrefabPathExhaust) as GameObject;
                var go = Instantiate(_exhaustPrefab, _boosterContainer);
                _exhaustVfxView = go.GetComponent<CarParticleSystemVfxView>();
            }
            
            _exhaustVfxView.gameObject.SetActive(true);
            
            _exhaustVfxView.Play();
        }

        public void StopBoostVFX()
        {
            if (_turboBoosterVfxView == null) return;
            
            _turboBoosterVfxView.Stop();
        }
    }
}