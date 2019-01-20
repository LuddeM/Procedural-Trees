using UnityEngine;

namespace TreeGeneration
{
    /// <summary>
    /// Used to show tree information next to the tree
    /// </summary>
    public class TreeInformationVisualizer : MonoBehaviour
    {

        public TreeInformation Information;
        private TextMesh _textMesh;
        private const int FontSize = 30;
        private float PositionOffset = 5;
        private const float CharacterSize = 0.5f;
        private GameObject _camera;
        private Vector3 _startPosition;

        public void CreateInformationVisualization()
        {
            _textMesh = gameObject.AddComponent<TextMesh>();
            _textMesh.fontSize = FontSize;
            _textMesh.characterSize = CharacterSize;
            _textMesh.alignment = TextAlignment.Left;
            _textMesh.anchor = TextAnchor.MiddleCenter;
            _startPosition = new Vector3(transform.position.x, 0, transform.position.z);
            SetText();
        }

        private void SetText()
        {
            _textMesh.text = "Volume type = " + Information.VolumeType
                + "\nTree size = " + Information.TreeSize
                + "\nStem length = " + Information.StemLength
                + "\nNumber of attractor points = " + Information.NumberOfPoints
                + "\nNode distance = " + Information.NodeDistance
                + "\nInfluence distance = " + Information.NodeDistance * Information.InfluenceMult
                + "\nDeath distance = " + Information.NodeDistance * Information.DeathMult;
        }

        private void OnEnable()
        {
            if (!_camera)
                _camera = GameObject.FindGameObjectWithTag("MainCamera");
            
            transform.LookAt(new Vector3(_camera.transform.position.x, 0, _camera.transform.position.z));
            transform.Rotate(new Vector3(0, 1, 0), 180);
            
            transform.position = _startPosition;

            transform.position += -transform.up * PositionOffset;
        }
    }
}
