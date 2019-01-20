using System.Collections.Generic;
using UnityEngine;

namespace TreeGeneration.MyVersion
{
    public class ProceduralTree : MonoBehaviour
    {

        private const int NumberOfPoints = 3000;
        public Volume VolumeType = Volume.Sphere;
        public float StemLength = 1;
        public float StemThickness = 0.5f;
        public float TreeSize = 20;
        public int ChildrenPerBranch = 3;

        private IVolume _volume;
        private TreeSegment _stem;
        private AttractorPointsManager _attractorPointsManager;
        private List<Node> _nodeList;
        private Queue<Node> _nodeQueue;
        private Node _root;

        // Use this for initialization
        void Start()
        {

            CreateStem();
            _attractorPointsManager = new AttractorPointsManager(VolumeType, _stem.End, TreeSize, NumberOfPoints);

            _nodeQueue = new Queue<Node>();
            _nodeList = new List<Node>(1);
            _root = new Node(_stem.End, ChildrenPerBranch, ref _nodeQueue, ref _nodeList, _attractorPointsManager);
            _root.CreateNewNodes();
        }

        private void CreateStem()
        {
            _stem = new TreeSegment(transform.position, transform.position + new Vector3(0, StemLength, 0), StemThickness);
        }

    }
}