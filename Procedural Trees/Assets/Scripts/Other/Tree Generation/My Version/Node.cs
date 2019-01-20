using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TreeGeneration.MyVersion
{
    public class Node
    {

        public Vector3 Position;
        public int[] ChildIndices;
        private TreeSegment[] _treeSegments;
        private List<Node> _nodeList;
        private Queue<Node> _nodeQueue;
        private AttractorPointsManager _pointManager;

        // Maybe it needs to be sent by reference?
        public Node(Vector3 position, int numberOfChildNodes, ref Queue<Node> queue, ref List<Node> nodeList, AttractorPointsManager pointManager)
        {
            Position = position;
            _nodeQueue = queue;
            _nodeList = nodeList;
            int currentIndex = nodeList.Count;
            _pointManager = pointManager;

            ChildIndices = new int[numberOfChildNodes];
            for (int i = 0; i < numberOfChildNodes; i++)
            {
                ChildIndices[i] = currentIndex;
                currentIndex++;
            }
        }

        public void CreateNewNodes()
        {
            if (_nodeList.Count > 200)
            {
                return;
            }
            CreateChildNodesAndEnqueue();
            DequeueAndCreateNewNodes();
        }

        private void CreateChildNodesAndEnqueue()
        {
            // Creates new child Nodes for this Node
            for (int i = 0; i < ChildIndices.Length; i++)
            {
                var point = _pointManager.GetClosestPoint(Position);
                var node = new Node(point, ChildIndices.Length, ref _nodeQueue, ref _nodeList, _pointManager);
                _nodeList.Add(node);
                _nodeQueue.Enqueue(node);
            }
        }

        private void DequeueAndCreateNewNodes()
        {
            // Traverse the queue to the next which should Create nodes
            for (int i = 0; i < ChildIndices.Length; i++)
            {
                if (_nodeQueue.Count <= 0)
                {
                    continue;
                }
                var node = _nodeQueue.Dequeue();
                node.CreateNewNodes();
            }
        }
    }

}
