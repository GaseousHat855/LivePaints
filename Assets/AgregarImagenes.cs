using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

    [RequireComponent(typeof(ARTrackedImageManager))]
public class AgregarImagenes : MonoBehaviour
{
    //Reference to AR tracked image manager component
    private ARTrackedImageManager _trackedImagesManager;

    //List of prefabs to instantiate - these should be named the same as ththeir corresponding 2D images in there reference image library
    public GameObject[] ArPrefabs;

    //Keep dictionary array of created prefabs
    private readonly Dictionary<string, GameObject> _instantiatedPrefabs = new Dictionary<string, GameObject>();

    void Awake() {
        //Change a reference to the Tracked Image Manager component
        _trackedImagesManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable() {
        //Attach event handler when traked image change
        _trackedImagesManager.trackedImagesChanged += OnTrakedImagesChanged;
    }

    void OnDisable() {
        //Remove event handler
        _trackedImagesManager.trackedImagesChanged -= OnTrakedImagesChanged;
    }

    //Event Hnandler
    private void OnTrakedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs){
        //Loop through all new traked images that have been detected
        foreach(var trackedImage in eventArgs.added){
            //Get the name of the reference image
            var imageName = trackedImage.referenceImage.name;
            //Now loop over the array of prefabs
            foreach(var curPrefab in ArPrefabs){
                //Check whether this prefab matches the traked image name, and the prefab hasent been created already
                if(string.Compare(curPrefab.name, imageName, StringComparison.OrdinalIgnoreCase) == 0 && !_instantiatedPrefabs.ContainsKey(imageName)){
                    //Instantiate the prefab, parenting it to the ARTackeImage
                    var newPrefab = Instantiate(curPrefab, trackedImage.transform);
                    //Add the created prefab to the array
                    _instantiatedPrefabs[imageName] = newPrefab;
                }
            }
        }

        //For all prefabs that have been created so far, set them active or not depending 
        //on whether their corresponding image is currently being tracked
        foreach(var trackedImage in eventArgs.removed){
            _instantiatedPrefabs[trackedImage.referenceImage.name].SetActive(trackedImage.trackingState == TrackingState.Tracking);
        }

        //If the AR subsystem has given up looking for a tracked image
        foreach(var trackedImage in eventArgs.removed){
            //Destroy its prefab
            Destroy(_instantiatedPrefabs[trackedImage.referenceImage.name]);
            //Allso remove the instance forme the array
            _instantiatedPrefabs.Remove(trackedImage.referenceImage.name);
            //Or simply set the prefab instance as inactive
            //_instantiatedPrefabs[trackedImage.referenceImage.name].SetActive(false);
        }
    }
}
