using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TreeGeneration.MyVersion
{

    public class AttractorPointsManager
    {

        public readonly List<Vector3> OriginalPoints;
        private List<Vector3> _attractorPoints;
        private IVolume _volume;

        public AttractorPointsManager(Volume VolumeType, Vector3 stemEnd, float treeSize, int numberOfPoints)
        {

            _volume = VolumeFactory.Create(VolumeType, stemEnd, treeSize);

            _attractorPoints = new List<Vector3>();
            CreateRandomAttractorPoints(stemEnd, treeSize, numberOfPoints);
            _attractorPoints = _volume.GetPointsWithinVolume(_attractorPoints);
            OriginalPoints = new List<Vector3>(_attractorPoints);

            VisualizeAttractorPoints();
        }

        private void CreateRandomAttractorPoints(Vector3 stemEnd, float treeSize, int numberOfPoints)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                var randX = Random.Range(-treeSize * 2, treeSize * 2);
                var randY = Random.Range(-treeSize * 2, treeSize * 2);
                var randZ = Random.Range(-treeSize * 2, treeSize * 2);

                _attractorPoints.Add(stemEnd + new Vector3(randX, randY, randZ));
            }
        }

        private void VisualizeAttractorPoints()
        {
            foreach (var point in _attractorPoints)
            {
                var pointVis = GameObject.CreatePrimitive(PrimitiveType.Cube);
                pointVis.transform.position = point;
                pointVis.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            }

        }

        public Vector3 GetClosestPoint(Vector3 position, float minRadius = 2)
        {
            var minDistance = float.MaxValue;
            Vector3 closestPoint = new Vector3();

            foreach (var point in _attractorPoints)
            {
                var distance = Vector3.Distance(position, point);
                if (distance < minDistance && distance > minRadius)
                {
                    minDistance = distance;
                    closestPoint = point;
                }
            }

            if (closestPoint == new Vector3())
            {
                return closestPoint;
            }

            RemoveNearbyPoints(closestPoint);

            return closestPoint;
        }

        private void RemoveNearbyPoints(Vector3 point, float threshold = 0.5f)
        {
            var toRemove = new List<Vector3>();
            foreach (var otherPoint in _attractorPoints)
            {
                var distance = Vector3.Distance(point, otherPoint);
                if (distance < 0.5f)
                {
                    toRemove.Add(otherPoint);
                }
            }

            if (toRemove.Count > 0)
            {
                _attractorPoints = _attractorPoints.Except(toRemove).ToList();
            }
        }

    }


}

