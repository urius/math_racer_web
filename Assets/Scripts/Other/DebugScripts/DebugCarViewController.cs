using UnityEngine;
using View;

namespace Other.DebugScripts
{
    public class DebugCarViewController : MonoBehaviour
    {
        private const float SpeedMult = 0.01f;

        [SerializeField] private CarView _carView;
        
        [Space(25)]
        [SerializeField] private float _bodyRotationMult;
        [SerializeField] [Range(0, 200)] private float _targetSpeed;

        private float _targetSpeedAdjusted;
        private float _speed;
        
        private void FixedUpdate()
        {
            AdjustSpeed();
            
            MoveCar();
        }

        private void MoveCar()
        {
            var deltaRotation = _speed * _carView.WheelRotationMultiplier;
            
            var deltaTargetSpeed = _targetSpeedAdjusted - _speed;
            _carView.SetBodyRotation(-deltaTargetSpeed * _bodyRotationMult);
            
            _carView.RotateWheels(deltaRotation);
        }

        private void AdjustSpeed()
        {
            _targetSpeedAdjusted = SpeedMult * _targetSpeed;
            
            _speed = Mathf.Lerp(_speed, _targetSpeedAdjusted, 0.05f);
        }
    }
}