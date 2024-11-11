using System;
using UnityEngine;

namespace View
{
    public class CarView : MonoBehaviour
    {
        [SerializeField] private Transform _carBody;
        [SerializeField] private Transform _wheelBack;
        [SerializeField] private Transform _wheelFront;

        [SerializeField] private float _bodyRotationMin = -5;
        [SerializeField] private float _bodyRotationMax = 5;
        
        private Vector3 _wheelSize;
        private float _wheelRadius;
        private float _wheelRotationMultiplier;
        private Transform _transform;

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
    }
}