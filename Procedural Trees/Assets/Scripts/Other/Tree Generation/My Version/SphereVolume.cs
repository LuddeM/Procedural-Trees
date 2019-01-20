using System.Collections.Generic;
using UnityEngine;

namespace TreeGeneration.MyVersion
{
    public class SphereVolume : IVolume
    {
        private readonly Vector3 _center;
        private readonly float _radius;

        public SphereVolume(Vector3 stemEnd, float radius)
        {
            _center = stemEnd + new Vector3(0, radius, 0);
            _radius = radius;
        }


        public List<Vector3> GetPointsWithinVolume(List<Vector3> points)
        {
            var pointsWithin = new List<Vector3>();

            foreach (var point in points)
            {
                if (!IsWithinVolume(point))
                    continue;

                pointsWithin.Add(point);
            }

            return pointsWithin;
        }

        private bool IsWithinVolume(Vector3 point)
        {
            return Vector3.Distance(_center, point) < _radius;
        }
    }
}