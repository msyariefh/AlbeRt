using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using TMPro;

namespace AlbeRt.AREnvironment
{
    [RequireComponent(typeof(ARTrackedImageManager))]
    class ImageTracking : MonoBehaviour
    {
        [SerializeField] private Object3DDatabase _database;
        [SerializeField] private ARObjectScale _scaleManager;
        [SerializeField] private TMP_Text _objectNameInformation;
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
                string _trackedImageDescription = _database.GetObject3D(_trackedImageName)?.Description;
                GameObject _trackedObject = _spawnedPrefabs[_trackedImageName];

                if (_trackedImage.trackingState == TrackingState.Tracking)
                {
                    _scaleManager.ChangeReferenceObject(_trackedObject);
                    _trackedObject.SetActive(true);
                    _objectNameInformation.text = $"Tracked: {_trackedImageDescription}";
                    _trackedObject.transform.position = _trackedImage.transform.position;
                }
                else
                {
                    _objectNameInformation.text = "Tracked: None";
                    _trackedObject.SetActive(false);
                }
                
            }
        }

    }
}
