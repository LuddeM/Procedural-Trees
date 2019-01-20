using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TreeGeneration
{
    /// <summary>
    /// Handles the attractor points' generation, visualization,
    /// as well as their interaction with the tree Nodes
    /// </summary>
    public class AttractorPointsManager
    {
        private const float AttractorPointSize = 0.5f;
        private Color AttractorPointColor = Color.green;

        public readonly List<Vector3> OriginalPoints;
        private List<Vector3> _attractorPoints;
        private List<Node> _closestPoints;
        private IVolume _volume;

        public AttractorPointsManager(
            IVolume volume, 
            float treeSize, 
            int numberOfPoints, 
            int randomSeed,
            float maximumNoiseDisplacement)
        {
            _volume = volume;

            _attractorPoints = new List<Vector3>();
            _closestPoints = new List<Node>();

            CreateSimplexNoiseAttractorPointsWithinVolume(
                _volume.CenterPos, 
                treeSize, 
                numberOfPoints, 
                randomSeed, 
                maximumNoiseDisplacement);

            OriginalPoints = new List<Vector3>(_attractorPoints);
        }

        private void CreateSimplexNoiseAttractorPointsWithinVolume(
            Vector3 centerPos, 
            float treeSize, 
            int numberOfPoints, 
            int randomSeed, 
            float maximumNoiseDisplacement)
        {
            var count = 0;
            UnityEngine.Random.InitState(randomSeed);

            while (count < numberOfPoints)
            {
                var randXPos = UnityEngine.Random.Range(-treeSize * 2, treeSize * 2);
                var randYPos = UnityEngine.Random.Range(-treeSize * 2, treeSize * 2);
                var randZPos = UnityEngine.Random.Range(-treeSize * 2, treeSize * 2);

                var point = centerPos + new Vector3(randXPos, randYPos, randZPos);
                if (!_volume.IsWithinVolume(point))
                {
                    continue;
                }

                // Adding Simplex noise to the point within the volume boundaries
                var simplexNoiseValue = (float)Common.SimplexNoise.noise(point.x, point.y, point.z);
                point.x += simplexNoiseValue * maximumNoiseDisplacement;
                point.x += simplexNoiseValue * maximumNoiseDisplacement;
                point.z += simplexNoiseValue * maximumNoiseDisplacement;

                _attractorPoints.Add(point);
                _closestPoints.Add(null);
                count++;
            }
        }

        public void VisualizeOriginalAttractorPoints(Transform parentTransform = null)
        {

            foreach (var point in OriginalPoints)
            {
                var pointVis = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                if(parentTransform)
                {
                    pointVis.transform.SetParent(parentTransform);
                }

                pointVis.transform.position = point;
                pointVis.transform.localScale = new Vector3(AttractorPointSize, AttractorPointSize, AttractorPointSize);

                var renderer = pointVis.GetComponent<MeshRenderer>();
                renderer.material.color = AttractorPointColor;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

                pointVis.name = "Attractor Point";
                var collider = pointVis.GetComponent<Collider>();
                if (collider)
                {
                    GameObject.Destroy(collider);
                }
            }
        }

        public void VisualizeAttractorPoints(Transform parentTransform = null)
        {

            foreach (var point in _attractorPoints)
            {
                var pointVis = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                if (parentTransform)
                {
                    pointVis.transform.SetParent(parentTransform);
                }

                pointVis.transform.position = point;
                pointVis.transform.localScale = new Vector3(AttractorPointSize, AttractorPointSize, AttractorPointSize);

                var renderer = pointVis.GetComponent<MeshRenderer>();
                renderer.material.color = AttractorPointColor;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

                pointVis.name = "Attractor Point";
                var collider = pointVis.GetComponent<Collider>();
                if (collider)
                {
                    GameObject.Destroy(collider);
                }
            }
        }



        private void VisualizeAttractorPoints(Color color, float size = 0.1f)
        {
            foreach (var point in _attractorPoints)
            {
                var pointVis = GameObject.CreatePrimitive(PrimitiveType.Cube);
                pointVis.transform.position = point;
                pointVis.transform.localScale = new Vector3(AttractorPointSize, AttractorPointSize, AttractorPointSize);

                var renderer = pointVis.GetComponent<MeshRenderer>();
                renderer.material.color = color;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

                pointVis.name = "Attractor Point";
                var collider = pointVis.GetComponent<Collider>();
                if (collider)
                {
                    GameObject.Destroy(collider);
                }
            }
        }

        /// <summary>
        /// For each of the attractor points the closest Node is found, which is also within the influence distance.
        /// There may be several attractor points for each of the Nodes. Returns the set of Nodes and their respective
        /// attractor points.
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public Dictionary<Node, List<Vector3>> GetNodeAndAttractorPointsSets(List<Node> nodes, float influenceDistance)
        {
            var nodeAndPointsSets = new Dictionary<Node, List<Vector3>>();

            for(var i=0; i<_attractorPoints.Count; i++)
            {
                var point = _attractorPoints[i];
                var nodeCandidates = new List<Node>(nodes);
                if (_closestPoints.Count != _attractorPoints.Count)
                {
                    throw new Exception(
                        "The number of closest points and attractor points were different."
                        + "Closest points were: " 
                        + _closestPoints.Count 
                        + " and attractor points were: "
                        + _attractorPoints.Count);
                }
                if(_closestPoints[i] != null)
                {
                    nodeCandidates.Add(_closestPoints[i]);
                }
                
                var closestNode = GetClosestNode(nodeCandidates, point);

                if (closestNode == null)
                {
                    continue;
                }

                var distance = Vector3.Distance(closestNode.Position, point);
                if (distance > influenceDistance)
                {
                    continue;
                }

                _closestPoints[i] = closestNode;

                if (!nodeAndPointsSets.ContainsKey(closestNode))
                {
                    nodeAndPointsSets.Add(closestNode, new List<Vector3>());
                }

                nodeAndPointsSets[closestNode].Add(point);
            }

            return nodeAndPointsSets;
        }

        /// <summary>
        /// Removes attractor points which are too close to the provided
        /// nodes.
        /// </summary>
        /// <param name="newNodes"></param>
        public void RemoveTooCloseAttractorPoints(List<Node> newNodes, float DeathDistance)
        {
            var pointsToRemove = new List<Vector3>();
            var closestPointsIndicesToRemove = new List<int>();

            for (int i = 0; i < _attractorPoints.Count; i++)
            {
                var closestNode = GetClosestNode(newNodes, _attractorPoints[i]);

                if(closestNode == null)
                {
                    continue;
                }

                if(Vector3.Distance(closestNode.Position, _attractorPoints[i]) < DeathDistance)
                {
                    pointsToRemove.Add(_attractorPoints[i]);
                    closestPointsIndicesToRemove.Add(i); // Remove the closest distance point
                }
            }
            closestPointsIndicesToRemove = closestPointsIndicesToRemove.OrderByDescending(i => i).ToList();
            closestPointsIndicesToRemove.ForEach(i => _closestPoints.RemoveAt(i));
            _attractorPoints = _attractorPoints.Except(pointsToRemove).ToList();
        }

        private Node GetClosestNode(List<Node> nodes, Vector3 point)
        {
            float shortestDistance = float.MaxValue;
            Node closestNode = null;

            foreach(var node in nodes)
            {
                var distance = Vector3.Distance(point, node.Position);
                if(distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestNode = node;
                }
            }

            return closestNode;
        }



    }
}