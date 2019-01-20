using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TreeGeneration.MyVersion
{
    public class TreeSegment
    {

        private float _thickness;
        private Vector3 _start;
        private Vector3 _end;
        private GameObject _gameObject;


        public Vector3 Start { get { return _start; } }
        public Vector3 End { get { return _end; } }

        public TreeSegment(Vector3 start, Vector3 end, float thickness)
        {
            _start = start;
            _end = end;
            _thickness = thickness;

            CreateBranch();
        }


        private void CreateBranch()
        {
            _gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            var halfLength = (_end - _start) / 2;
            _gameObject.transform.localScale += halfLength;
            _gameObject.transform.position = _start + halfLength;
        }
    }
}