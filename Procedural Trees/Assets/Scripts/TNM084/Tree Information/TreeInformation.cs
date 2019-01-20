using UnityEngine;

namespace TreeGeneration
{
    /// <summary>
    /// Contains information about the tree
    /// </summary>
    public class TreeInformation : ScriptableObject {

        public VolumeType VolumeType;

        public int NumberOfPoints;

        public float TreeSize;
        public float StemLength;
        public float TipNodeThickness;
        public float ThicknessPower; // Preferably between 2 and 3
        public float NodeDistance;
        public float InfluenceMult;
        public float DeathMult;

    }
}