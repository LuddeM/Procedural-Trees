using UnityEngine;
using System;

namespace TreeGeneration
{
    public enum VolumeType
    {
        Sphere
    }

    public static class VolumeFactory
    {
        public static IVolume Create(VolumeType volume, Vector3 position, float scale)
        {
            if (volume == VolumeType.Sphere)
            {
                return new SphereVolume(position, scale);
            }

            throw new ArgumentException("Expected provided volume to be one of the enum Volume types. Got: " + volume);
        }
    }
}