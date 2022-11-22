using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

namespace AlbeRt.AREnvironment
{
    [RequireComponent(typeof(ARTrackedImageManager))]
    class ImageTracking : MonoBehaviour
    {
        [SerializeField] private Object3DDatabase _database;
        private Dictionary<string, GameObject> _spawnedPrefabs = new();
        private ARTrackedImageManager _trackedImageManager;

        private void Awake()
        {
            _trackedImageManager = GetComponent<ARTrackedImageManager>();
            for (int i = 0; i < _database.Length; i++)
            {
                InstantiateNewPrefab(_database.GetObject3D(i));
            }

        }
        private void OnEnable()
        {
            _trackedImageManager.trackedImagesChanged += OnImageChanged;
        }

        private void OnDisable()
        {
            _trackedImageManager.trackedImagesChanged -= OnImageChanged;
        }

        private void InstantiateNewPrefab(Object3D object3d)
        {
            GameObject _newPrefab = Instantiate(object3d.Object, transform);
            _newPrefab.name = object3d.Name;
            _newPrefab.SetActive(false);
            _spawnedPrefabs.Add(object3d.Name, _newPrefab);
        }

        private void OnImageChanged(ARTrackedImagesChangedEventArgs args)
        {
            
            foreach (ARTrackedImage _trackedImage in args.updated)
            {
                string _trackedImageName = _trackedImage.referenceImage.name;
                GameObject _trackedObject = _spawnedPrefabs[_trackedImageName];

                if (_trackedImage.trackingState == TrackingState.Tracking)
                {
                    _trackedObject.SetActive(true);
                    _trackedObject.transform.position = _trackedImage.transform.position;
                }
                else
                {
                    _trackedObject.SetActive(false);
                }
                
            }
        }

    }
}
