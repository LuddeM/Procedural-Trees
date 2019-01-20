using UnityEngine;

namespace TreeGeneration
{
    /// <summary>
    /// Shows tree information visualization on mouse hover
    /// </summary>
    public class TreeMouseInteraction : MonoBehaviour
    {
        public TreeInformation TreeInformation;
        public GameObject InformationVisualizer;
        private SphereCollider _collider;

        void Start()
        {
            if (!(TreeInformation && InformationVisualizer))
            {
                return;
            }
            _collider = gameObject.AddComponent<SphereCollider>();
            var position = transform.localPosition;
            _collider.center = -position + SphereVolume.CalculateCenterPosition(new Vector3(0, TreeInformation.StemLength, 0), TreeInformation.TreeSize);
            _collider.radius = TreeInformation.TreeSize;
        }


        private void OnMouseEnter()
        {
            if(!(TreeInformation && InformationVisualizer))
            {
                return;
            }

            InformationVisualizer.SetActive(true);
        }

        private void OnMouseExit()
        {
            if (!(TreeInformation && InformationVisualizer))
            {
                return;
            }

            InformationVisualizer.SetActive(false);
        }
    }
}