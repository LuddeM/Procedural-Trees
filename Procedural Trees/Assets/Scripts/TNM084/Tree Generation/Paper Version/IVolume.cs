using System.Collections.Generic;
using UnityEngine;

namespace TreeGeneration
{
    public interface IVolume
    {
        Vector3 CenterPos
        {
            get;
        }

        List<Vector3> GetPointsWithinVolume(List<Vector3> points);
        bool IsWithinVolume(Vector3 point);
        void VisualizeVolume(Transform parentTransform = null);
    }
}