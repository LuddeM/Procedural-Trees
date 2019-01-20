using UnityEngine;

/// <summary>
/// Removes the saved preview mesh from the tree prefab
/// </summary>
public class RemovePreviewVisualization : MonoBehaviour {

	void Start () {
        var tree = transform.Find("Generated Tree");
        if(tree)
        {
            Destroy(tree.gameObject);
        }
	}
}
