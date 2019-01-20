using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TreeGeneration
{
    public class ProceduralTree : MonoBehaviour
    {
        private const int MaxIterations = 250; // When it takes too much time

        public int RandomSeed = 5000;
        public float MaximumNoiseDisplacement = 5;
        public VolumeType VolumeType = VolumeType.Sphere;
        public int NumberOfPoints = 1500;
        public float TreeSize = 10;

        public float StemLength = 1;
        public float TipNodeThickness = 0.1f;
        public float ThicknessPower = 2.0f; // Preferably between 2 and 3

        public float NodeDistance = 0.5f;
        public float InfluenceDistanceMultiplier = 20;
        public float DeathDistanceMultiplier = 5;

        private float _influenceDistance;
        private float _deathDistance;

        private AttractorPointsManager _attractorPointsManager;
        private List<Node> _nodeList;
        private List<Branch> _branchList;
        private TextureColorManager _textureColorManager;
        private IVolume _volume;

        private GameObject _treeObject;
        private GameObject _branches;
        private GameObject _treeNodes;
        private GameObject _tipNodes;
        private GameObject _originalAttractorPoints;
        private GameObject _currentAttractorPoints;
        private GameObject _infoVisualizer;

        private TreeInformation _treeInformation;

        private TreeAnimator _treeAnimator;
        private bool _shouldCreateAnimation;


        void Start()
        {
            _treeAnimator = GetComponent<TreeAnimator>();
            if(_treeAnimator)
            {
                _shouldCreateAnimation = true;
            }

            AddTreeInformation();
            SetUpVisualizationObjects();
            SetUpInteraction();

            _textureColorManager = GetComponent<TextureColorManager>();
            if(_textureColorManager == null)
            {
                gameObject.AddComponent<TextureColorManager>();
                _textureColorManager = GetComponent<TextureColorManager>();
            }

            _influenceDistance = InfluenceDistanceMultiplier * NodeDistance;
            _deathDistance = DeathDistanceMultiplier * NodeDistance;

            var stemEnd = transform.position + new Vector3(0, StemLength, 0);
            _volume = VolumeFactory.Create(VolumeType, stemEnd, TreeSize);
            _attractorPointsManager = new AttractorPointsManager(
                _volume,
                TreeSize,
                NumberOfPoints,
                RandomSeed,
                MaximumNoiseDisplacement);

            var startNode = new Node(transform.position, null, ThicknessPower);
            _nodeList = new List<Node>();
            _nodeList.Add(startNode);
            _branchList = new List<Branch>();

            // For creating a step by step animation of the tree creation, saved as Images
            if(_shouldCreateAnimation)
            {
                _treeAnimator.VisualizeVolumeTransparently(_attractorPointsManager, _volume);
            }

            CreateTreeStructure(startNode);

            SetTreeThickness();
            VisualizeTree();
            if (_shouldCreateAnimation)
            {
                _treeAnimator.TakeSnapShot();
            }
        }


        private void CreateTreeStructure(Node rootNode)
        {
            int iterations = 0;
            bool isCreatingStem = true;

            List<Node> totalNodeList = new List<Node>();
            totalNodeList.Add(rootNode);
            Dictionary<Node, List<Vector3>> nodeAndPointsSets;
            var nodeSearchList = new List<Node>{ rootNode };

            do
            {
                nodeAndPointsSets = _attractorPointsManager.GetNodeAndAttractorPointsSets(nodeSearchList, _influenceDistance);

                // Making sure that the Stem can be created, regardless if the influence distance is too small.
                if(isCreatingStem)
                {
                    if(nodeAndPointsSets.Count == 0)
                    {
                        // Use maximum float value to search for nodes.
                        nodeAndPointsSets = _attractorPointsManager.GetNodeAndAttractorPointsSets(nodeSearchList, float.MaxValue);
                    }
                    else
                    {
                        isCreatingStem = false;
                    }
                }

                // Maybe later for optimizing....
                // Remove nodes from nodeList which are not part of the attractorSets
                // totalNodeList = totalNodeList.Intersect(attractorSets);

                // For the ones part of the attractorSets calculate their children/branches
                var newNodes = GetNewNodes(nodeAndPointsSets);
                totalNodeList.AddRange(newNodes);
                nodeSearchList = newNodes;

                // Check if any of the new nodes are too close to the attractorPoints (deathRadius)
                _attractorPointsManager.RemoveTooCloseAttractorPoints(newNodes, _deathDistance);
                iterations++;

                if(_shouldCreateAnimation)
                {
                    _treeAnimator.MakePrintScreenAndRemoveObjects(_attractorPointsManager, totalNodeList);
                }
            }
            while (nodeAndPointsSets.Count > 0 && iterations < MaxIterations);

            if(iterations == MaxIterations)
            {
                Debug.Log("Maximum Iterations Reached.");
            }

            _nodeList.AddRange(totalNodeList);
        }

        private void SetTreeThickness()
        {
            var tipNodes = _nodeList
                .Where(node => node.IsBranchTipNode).ToList();

            foreach(var node in tipNodes)
            {
                var currentNode = node;
                currentNode.AddThickness(null, TipNodeThickness);

                while (currentNode != _nodeList.First())
                {
                    currentNode.ParentBranch.ParentNode.AddThickness(currentNode.ParentBranch, currentNode.Thickness);
                    currentNode = currentNode.ParentBranch.ParentNode;
                }
            }

            tipNodes.ForEach(node => node.VisualizeNode(_tipNodes.transform));
        }

        /// <summary>
        /// Uses sets of existing nodes and attractor points to create new tree Nodes
        /// </summary>
        /// <param name="nodeAndPointsSets"></param>
        /// <returns></returns>
        private List<Node> GetNewNodes(Dictionary<Node, List<Vector3>> nodeAndPointsSets)
        {
            var newNodes = new List<Node>();

            foreach(var set in nodeAndPointsSets)
            {
                var newDirection = CalculateMeanDirection(set.Key, set.Value);
                var newPosition = set.Key.Position + NodeDistance * newDirection;
                var newBranch = new Branch(set.Key, set.Key.Position, newPosition);

                if (!set.Key.ShouldAddBranch(newBranch))
                {
                    continue;
                }
                _branchList.Add(newBranch);
                var newNode = new Node(newPosition, newBranch, ThicknessPower);
                newNodes.Add(newNode);
                set.Key.AddBranch(newBranch);
            }

            return newNodes;
        }

        private Vector3 CalculateMeanDirection(Node node, List<Vector3> points)
        {
            Vector3 direction = new Vector3();

            foreach(var point in points)
            {
                direction += point - node.Position;
            }

            direction.Normalize();

            return direction;
        }

        private void VisualizeTreeNodes()
        {
            foreach (var node in _nodeList)
            {
                node.VisualizeNode(_treeNodes.transform);
            }
        }

        /// <summary>
        /// Visualizes the distances used for the current tree generation using cylinders
        /// </summary>
        private void VisualizeSettings()
        {
            var influenceDistanceVisualized = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            influenceDistanceVisualized.name = "Influence Distance Visualized";
            influenceDistanceVisualized.GetComponent<MeshRenderer>().material.color = Color.green;
            influenceDistanceVisualized.transform.localScale = new Vector3(1, _influenceDistance / 2, 1);
            influenceDistanceVisualized.transform.position = new Vector3(3, _influenceDistance / 2, 0);
            var collider = influenceDistanceVisualized.GetComponent<Collider>();
            if (collider)
            {
                GameObject.Destroy(collider);
            }

            var deathDistanceVisualized = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            deathDistanceVisualized.name = "Death Distance Visualized";
            deathDistanceVisualized.GetComponent<MeshRenderer>().material.color = Color.red;
            deathDistanceVisualized.transform.localScale = new Vector3(1, _deathDistance / 2, 1);
            deathDistanceVisualized.transform.position = new Vector3(5, _deathDistance / 2, 0);
            collider = deathDistanceVisualized.GetComponent<Collider>();
            if (collider)
            {
                GameObject.Destroy(collider);
            }

            var treeSizeRadiusVisualized = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            treeSizeRadiusVisualized.name = "Tree Size (Radius) Visualized";
            treeSizeRadiusVisualized.GetComponent<MeshRenderer>().material.color = Color.grey;
            treeSizeRadiusVisualized.transform.localScale = new Vector3(1, TreeSize / 2, 1);
            treeSizeRadiusVisualized.transform.position = new Vector3(7, TreeSize / 2, 0);
            collider = treeSizeRadiusVisualized.GetComponent<Collider>();
            if (collider)
            {
                GameObject.Destroy(collider);
            }
        }

        private void VisualizeTree()
        {
            VisualizeTreeNodes();
            //VisualizeSettings();
            _attractorPointsManager.VisualizeOriginalAttractorPoints(_originalAttractorPoints.transform);
            _attractorPointsManager.VisualizeAttractorPoints(_currentAttractorPoints.transform);

            _nodeList
                .Where(node => node.ParentBranch != null).ToList()
                .ForEach(node => node.ParentBranch.VisualizeBranch(node.Thickness, _textureColorManager.GetBranchColorWithTexture(), _branches.transform));

            var infoVisualizer = _infoVisualizer.AddComponent<TreeInformationVisualizer>();
            _infoVisualizer.transform.position = -_infoVisualizer.transform.parent.localPosition;
            infoVisualizer.Information = _treeInformation;
            infoVisualizer.CreateInformationVisualization();

            _volume.VisualizeVolume(_treeObject.transform);
        }

        /// <summary>
        /// Used to make it possible to hide and show certain
        /// parts of the tree's visualization
        /// </summary>
        private void SetUpVisualizationObjects()
        {
            _treeObject = new GameObject();
            _treeObject.transform.SetParent(transform);
            _treeObject.name = "Generated Tree";

            _branches = new GameObject();
            _branches.transform.SetParent(_treeObject.transform);
            _branches.name = "Branches";

            _treeNodes = new GameObject();
            _treeNodes.transform.SetParent(_treeObject.transform);
            _treeNodes.name = "Nodes";
            _treeNodes.SetActive(false);

            _tipNodes = new GameObject();
            _tipNodes.transform.SetParent(_treeObject.transform);
            _tipNodes.name = "Tip Nodes";
            _tipNodes.SetActive(false);

            _originalAttractorPoints = new GameObject();
            _originalAttractorPoints.transform.SetParent(_treeObject.transform);
            _originalAttractorPoints.name = "Original Attractor Points";
            _originalAttractorPoints.SetActive(false);

            _currentAttractorPoints = new GameObject();
            _currentAttractorPoints.transform.SetParent(_treeObject.transform);
            _currentAttractorPoints.name = "Current Attractor Points";
            _currentAttractorPoints.SetActive(false);

            _infoVisualizer = new GameObject();
            _infoVisualizer.transform.SetParent(_treeObject.transform);
            _infoVisualizer.name = "Information";
            _infoVisualizer.SetActive(false);
  
        }

        private void SetUpInteraction()
        {
            var interaction = _treeObject.AddComponent<TreeMouseInteraction>();
            interaction.TreeInformation = _treeInformation;
            interaction.InformationVisualizer = _infoVisualizer;
        }

        /// <summary>
        /// For visualizing the tree's information
        /// </summary>
        private void AddTreeInformation()
        {
            _treeInformation = ScriptableObject.CreateInstance<TreeInformation>();
            _treeInformation.VolumeType = VolumeType;
            _treeInformation.NumberOfPoints = NumberOfPoints;
            _treeInformation.TipNodeThickness = TipNodeThickness;
            _treeInformation.TreeSize = TreeSize;
            _treeInformation.StemLength = StemLength;
            _treeInformation.ThicknessPower = ThicknessPower;
            _treeInformation.NodeDistance = NodeDistance;
            _treeInformation.InfluenceMult = InfluenceDistanceMultiplier;
            _treeInformation.DeathMult = DeathDistanceMultiplier;
        }
    }


}