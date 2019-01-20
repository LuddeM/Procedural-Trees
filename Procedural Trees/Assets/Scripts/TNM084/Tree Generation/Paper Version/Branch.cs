using UnityEngine;

namespace TreeGeneration
{
    /// <summary>
    /// Branches between the Nodes of the tree
    /// </summary>
    public class Branch
    {
        private const float ThicknessReduction = 0.05f;
        private Vector3 _start;
        private Vector3 _direction;
        private float _length;
        private GameObject _branchVisObject;
        public Vector3 StartPos { get { return _start; } }
        public Vector3 EndPos { get { return _start + (_length * _direction); } }
        public readonly Node ParentNode;
        private Vector3 MidPos { get { return _start + (_length/2 * _direction); } }
        public Vector3 Direction { get { return _direction; } }

        public Branch(Node parentNode, Vector3 start, Vector3 end)
        {
            ParentNode = parentNode;
            _length = Vector3.Distance(end, start);
            _start = start;
            _direction = end - start;
            _direction.Normalize();
        }

        public void VisualizeBranch(float thickness, ColorInformation colorInfo, Transform parentTransform = null)
        {
            CreateCylinderVisualization(thickness, colorInfo, parentTransform);
        }

        private void CreateCylinderVisualization(float thickness, ColorInformation colorInfo, Transform parentTransform = null)
        {
            _branchVisObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            if(parentTransform)
            {
                _branchVisObject.transform.SetParent(parentTransform);
            }
            _branchVisObject.name = "Branch";
            SetCylinderTransform(thickness);
            SetCylinderMaterial(colorInfo);
        }

        private void SetCylinderTransform(float thickness)
        {
            _branchVisObject.transform.localScale = new Vector3(thickness, _length / 2, thickness);
            _branchVisObject.transform.position = StartPos + (_length / 2 * _direction);
            _branchVisObject.transform.LookAt(EndPos);
            _branchVisObject.transform.rotation *= Quaternion.Euler(90, 0, 0);

            var collider = _branchVisObject.GetComponent<Collider>();
            if (collider)
            {
                GameObject.Destroy(collider);
            }
        }

        private void SetCylinderMaterial(ColorInformation colorInfo)
        {
            var renderMaterial = _branchVisObject.GetComponent<MeshRenderer>().material;
            renderMaterial.color = colorInfo.Color;
            renderMaterial.mainTexture = colorInfo.Texture;
            renderMaterial.EnableKeyword("_NORMALMAP");
        }
    }
}