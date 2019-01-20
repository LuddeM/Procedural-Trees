using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TreeGeneration
{
    /// <summary>
    /// A Class for each of the Nodes in the Procedural Tree,
    /// and their connections to the Branches of the tree.
    /// </summary>
    public class Node
    {
        private const float NodeVisualizationSize = 0.5f;

        public Vector3 Position;
        private List<Branch> _treeSegments;
        public List<Branch> TreeSegments { get { return _treeSegments; } }

        private List<float> _treeSegmentPoweredThickness;

        private float _poweredThickness;
        public readonly Branch ParentBranch;
        private readonly float _thicknessPower;

        // Maybe it needs to be sent by reference?
        public Node(Vector3 position, Branch parentBranch, float thicknessPower)
        {
            _poweredThickness = 0.0f;
            _thicknessPower = thicknessPower;
            ParentBranch = parentBranch;
            Position = position;
            _treeSegments = new List<Branch>();
            _treeSegmentPoweredThickness = new List<float>();
        }

        public bool ShouldAddBranch(Branch branch)
        {
            if(ParentBranch != null)
            {
                // Make sure that the branch does not make a hard turn
                if (Vector3.Dot(branch.Direction, ParentBranch.Direction) < -0.8){
                    return false;
                }
            }
            return IsNewBranchDirection(branch.Direction);
        }

        private bool IsNewBranchDirection(Vector3 direction)
        {
            foreach(var segment in _treeSegments)
            {
                if(Vector3.Angle(segment.Direction, direction) < 10)
                {
                    return false;
                }
            }
            return true;
        }

        public void AddBranch(Branch branch)
        {
            _treeSegments.Add(branch);
            _treeSegmentPoweredThickness.Add(0.0f);
        }
        /// <summary>
        /// True if there are no branches spawned from this
        /// node.
        /// </summary>
        public bool IsBranchTipNode
        {
            get { return _treeSegments.Count == 0; }
        }

        /// <summary>
        /// Gets the thickness of the parent branch, depending on the 
        /// 'to the power of'-variable _thicknessPower
        /// </summary>
        public float Thickness
        {
            get {
                return Mathf.Min(3, Mathf.Pow(_poweredThickness, 1 / _thicknessPower)); }
        }

        /// <summary>
        /// Takes a branch parameter which can be null and a thickness parameter.
        /// Adds thickness.
        /// </summary>
        /// <param name="branch"></param>
        /// <param name="thickness"></param>
        public void AddThickness(Branch branch, float thickness)
        {
            if (_treeSegments.Count > 1 && branch != null)
            {
                if (_treeSegments.Contains(branch))
                {
                    var index = _treeSegments.IndexOf(branch);
                    _treeSegmentPoweredThickness[index] = Mathf.Pow(thickness, _thicknessPower);
                    UpdateThickness();
                }
            }
            else
            {
                _poweredThickness = Mathf.Pow(thickness, _thicknessPower);
            }
        }

        private void UpdateThickness()
        {
            _poweredThickness = _treeSegmentPoweredThickness.Sum(thickness => thickness);
        }

        public void VisualizeNode(Transform parentTransform = null)
        {
            var startNode = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            if(parentTransform)
            {
                startNode.transform.SetParent(parentTransform);
            }
            var renderer = startNode.GetComponent<MeshRenderer>();
            renderer.material.color = Color.black;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            startNode.transform.position = Position;
            startNode.transform.localScale = new Vector3(NodeVisualizationSize, NodeVisualizationSize, NodeVisualizationSize);

            var collider = startNode.GetComponent<Collider>();
            if (collider)
            {
                GameObject.Destroy(collider);
            }
        }
    }
}