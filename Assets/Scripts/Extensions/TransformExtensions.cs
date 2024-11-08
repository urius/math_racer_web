using UnityEngine;

namespace Extensions
{
    public static class TransformExtensions
    {
        public static void Move(this Transform transform, Vector3 delta)
        {
            var pos = transform.position;
            pos += delta;
            
            transform.position = pos;
        }
    }
}