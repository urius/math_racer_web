using System.Collections.Generic;
using Extensions;
using UnityEngine;

namespace View.Gameplay.Race
{
    public class RoadContainerView : MonoBehaviour
    {
        [SerializeField] private Transform[] _roadTransforms;
        
        private LinkedList<Transform> _roadTransformsList;
        private float _veryLeftRoadPartXPosition;
        private Vector3 _roadPartBoundsSize;

        private void Awake()
        {
            _veryLeftRoadPartXPosition = _roadTransforms[0].position.x;
            _roadPartBoundsSize = _roadTransforms[0].GetComponent<SpriteRenderer>().bounds.size;
            
            _roadTransformsList = new LinkedList<Transform>(_roadTransforms);
        }

        public void Move(float distance)
        {
            var moveVector = new Vector3(-distance, 0, 0);
            
            foreach (var bgTransform in _roadTransformsList)
            {
                bgTransform.Move(moveVector);
            }

            var firstRoadPart = _roadTransformsList.First.Value;
            
            if (firstRoadPart.transform.position.x <= _veryLeftRoadPartXPosition)
            {
                var newPos = _roadTransformsList.Last.Value.position;
                newPos.x += _roadPartBoundsSize.x;
                firstRoadPart.position = newPos;
                
                _roadTransformsList.RemoveFirst();
                _roadTransformsList.AddLast(firstRoadPart);
            }
        }
    }
}