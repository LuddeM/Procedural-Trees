using System.Collections.Generic;
using UnityEngine;

namespace TreeGeneration.MyVersion
{
    public interface IVolume
    {

        List<Vector3> GetPointsWithinVolume(List<Vector3> points);
    }
}

