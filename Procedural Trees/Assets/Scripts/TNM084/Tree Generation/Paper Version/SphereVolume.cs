using System.Collections.Generic;
using UnityEngine;

namespace TreeGeneration
{
    /// <summary>
    /// Contains all of the tree's attractor points
    /// </summary>
    public class SphereVolume : IVolume
    {
        private readonly float _radius;

        public Vector3 CenterPos { get; private set; }

        public SphereVolume(Vector3 stemEnd, float radius)
        {
            CenterPos = CalculateCenterPosition(stemEnd, radius);
            _radius = radius;
        }

        public static Vector3 CalculateCenterPosition(Vector3 stemEnd, float radius)
        {
            return stemEnd + new Vector3(0, radius - (radius / 5), 0);
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

        public bool IsWithinVolume(Vector3 point)
        {
            return Vector3.Distance(CenterPos, point) < _radius;
        }

        public void VisualizeVolume(Transform parentTransform = null)
        {
            var treeSizeRadiusVisualized = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            if (parentTransform)
            {
                treeSizeRadiusVisualized.transform.SetParent(parentTransform);
            }

            treeSizeRadiusVisualized.name = "Tree Size Sphere";
            var renderer = treeSizeRadiusVisualized.GetComponent<MeshRenderer>();
            renderer.material.color = Color.green;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            var diameter = _radius * 2;
            treeSizeRadiusVisualized.transform.localScale = new Vector3(diameter, diameter, diameter);
            treeSizeRadiusVisualized.transform.position = CenterPos;
            var collider = treeSizeRadiusVisualized.GetComponent<Collider>();
            if (collider)
            {
                GameObject.Destroy(collider);
            }

            treeSizeRadiusVisualized.SetActive(false);
        }

        public GameObject VisualizeAndGetVolume(Transform parentTransform = null, bool shouldBeEnabled = false)
        {
            var treeSizeRadiusVisualized = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            if (parentTransform)
            {
                treeSizeRadiusVisualized.transform.SetParent(parentTransform);
            }
            treeSizeRadiusVisualized.name = "Tree Size Sphere";
            var renderer = treeSizeRadiusVisualized.GetComponent<MeshRenderer>();
            renderer.material.color = Color.green;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            var diameter = _radius * 2;
            treeSizeRadiusVisualized.transform.localScale = new Vector3(diameter, diameter, diameter);
            treeSizeRadiusVisualized.transform.position = CenterPos;
            var collider = treeSizeRadiusVisualized.GetComponent<Collider>();
            if (collider)
            {
                GameObject.Destroy(collider);
            }

            treeSizeRadiusVisualized.SetActive(shouldBeEnabled);

            return treeSizeRadiusVisualized;
        }
    }
}