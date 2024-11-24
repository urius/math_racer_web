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
        
        public static void MoveX(this Transform transform, float x)
        {
            var pos = transform.position;
            pos.x += x;
            
            transform.position = pos;
        }
        
        public static void SetXPosition(this Transform transform, float xPosition)
        {
            var pos = transform.position;
            pos.x = xPosition;
            
            transform.position = pos;
        }
        
        public static void SetAnchoredXPosition(this RectTransform transform, float xPosition)
        {
            var pos = transform.anchoredPosition;
            pos.x = xPosition;
            
            transform.anchoredPosition = pos;
        }
    }
}