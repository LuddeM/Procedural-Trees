using System.Collections.Generic;
using UnityEngine;
using TreeGeneration;

/// <summary>
/// Takes snapshots of the tree during its creation
/// to show the process, step by step
/// </summary>
public class TreeAnimator : MonoBehaviour {

    public Camera Camera;
    private SnapShotTaker _snapShotTaker;
    public Material SphereMaterial;

	void Start ()
    {
        TryInitiating();
	}

    private void TryInitiating()
    {
        if(_snapShotTaker)
        {
            return;
        }

        if (Camera)
        {
            _snapShotTaker = Camera.GetComponent<SnapShotTaker>();
        }
        else
        {
            var camera = FindObjectOfType<Camera>();
            if (camera)
            {
                _snapShotTaker = camera.GetComponent<SnapShotTaker>();
            }
        }
    }
	

    public void MakePrintScreenAndRemoveObjects(AttractorPointsManager attractorPointsManager, List<Node> nodeList)
    {
        TryInitiating();

        var parentObject = new GameObject();

        foreach (var node in nodeList)
        {
            node.VisualizeNode(parentObject.transform);
        }

        attractorPointsManager.VisualizeAttractorPoints(parentObject.transform);
        _snapShotTaker.CaptureScreenShotNow();

        parentObject.SetActive(false);
        Destroy(parentObject);
    }

    public void VisualizeVolumeTransparently(AttractorPointsManager attractorPointsManager, IVolume volume)
    {
        TryInitiating();

        var parentObject = new GameObject();
        var attractorPointsObject = new GameObject();
        attractorPointsObject.transform.SetParent(parentObject.transform);
        attractorPointsManager.VisualizeAttractorPoints(attractorPointsObject.transform);

        SphereVolume sphereVolume = (SphereVolume)volume; // TODO: Bad hack, fix later
        var volumeVis = sphereVolume.VisualizeAndGetVolume(parentObject.transform, true);
        var renderer = volumeVis.GetComponent<MeshRenderer>();

        if(SphereMaterial)
        {
            renderer.material = SphereMaterial;
        }
        var numberOfTimes = 20;
        var decreaseAmount = renderer.material.color.a / numberOfTimes;

        attractorPointsObject.SetActive(false);
        _snapShotTaker.CaptureScreenShotNow();
        attractorPointsObject.SetActive(true);
        _snapShotTaker.CaptureScreenShotNow();

        for (int i = 0; i < numberOfTimes; i++)
        {
            Color color = renderer.material.color;
            color.a -= decreaseAmount;
            renderer.material.color = color;
            _snapShotTaker.CaptureScreenShotNow();
        }

        parentObject.SetActive(false);
        Destroy(parentObject);
    }

    public void TakeSnapShot()
    {
        _snapShotTaker.CaptureScreenShotNow();
    }

}
