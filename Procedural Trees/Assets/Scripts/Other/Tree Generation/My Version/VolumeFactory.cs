using UnityEngine;
using System;

namespace TreeGeneration.MyVersion
{
    public enum Volume
    {
        Sphere
    }

    public static class VolumeFactory
    {


        public static IVolume Create(Volume volume, Vector3 position, float scale)
        {
            if (volume == Volume.Sphere)
            {
                return new SphereVolume(position, scale);
            }

            throw new ArgumentException("Expected provided volume to be one of the enum Volume types. Got: " + volume);
        }

    }
}