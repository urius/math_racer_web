using System.Collections.Generic;
using System.Linq;
using Extensions;
using UnityEngine;

namespace View.Gameplay.Race
{
    public class BgContainerView : MonoBehaviour
    {
        [SerializeField] private Transform[] _bgTransforms;
        [SerializeField] private Camera _camera;
        
        private LinkedList<Transform> _bgTransformsList;
        private float _cameraPositionX;
        private Vector3 _bgBoundsSize;

        private void Awake()
        {
            _cameraPositionX = _camera.transform.position.x;
            _bgBoundsSize = _bgTransforms[0].GetComponent<SpriteRenderer>().bounds.size;
            
            _bgTransformsList = new LinkedList<Transform>(_bgTransforms);
        }

        public void Move(float distance)
        {
            var moveVector = new Vector3(-distance, 0, 0);
            
            foreach (var bgTransform in _bgTransformsList)
            {
                bgTransform.Move(moveVector);
            }

            var firstBg = _bgTransformsList.First();
            if (firstBg.position.x + _bgBoundsSize.x < _cameraPositionX)
            {
                var newPos = _bgTransformsList.Last.Value.position;
                newPos.x += _bgBoundsSize.x;
                firstBg.position = newPos;
                
                _bgTransformsList.RemoveFirst();
                _bgTransformsList.AddLast(firstBg);
            }
        }
    }
}