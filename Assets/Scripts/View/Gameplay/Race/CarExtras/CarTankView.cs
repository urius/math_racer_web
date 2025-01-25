using UnityEngine;

namespace View.Gameplay.Race.CarExtras
{
    public class CarTankView : CarView
    {
        [SerializeField] private Transform[] _extraWheels;

        public override void RotateWheels(float degrees)
        {
            base.RotateWheels(degrees);

            foreach (var extraWheel in _extraWheels)
            {
                extraWheel.Rotate(0, 0, -degrees);
            }
        }
    }
}