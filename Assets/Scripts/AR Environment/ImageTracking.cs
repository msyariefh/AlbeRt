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
            //foreach(ARTrackedImage _trackedImage in args.added)
            //{
            //    UpdateImage(_trackedImage);
            //}

            //foreach (ARTrackedImage _trackedImage in args.updated)
            //{
            //    UpdateImage(_trackedImage);
            //}

            //foreach (ARTrackedImage _trackedImage in args.removed)
            //{
            //    _spawnedPrefabs[_trackedImage.name].SetActive(false);
            //}
            foreach (ARTrackedImage _trackedImage in args.updated)
            {
                string _trackedImageName = _trackedImage.referenceImage.name;
                GameObject _trackedObject = _spawnedPrefabs[_trackedImageName];

                switch (_trackedImage.trackingState)
                {
                    case TrackingState.Tracking:
                        _trackedObject.SetActive(true);
                        _trackedObject.transform.position = _trackedImage.transform.position;
                        break;
                    case TrackingState.Limited:
                        continue;
                    case TrackingState.None:
                        _trackedObject.SetActive(false);
                        break;

                }
            }
        }

        // https://forum.unity.com/threads/arfoundation-2-image-tracking-with-many-ref-images-and-many-objects.680518/#post-4915259
        //private void UpdateImage(ARTrackedImage trackedImage)
        //{
        //    string _name = trackedImage.referenceImage.name;
        //    Vector3 _position = trackedImage.transform.position;

        //    GameObject _prefab = _spawnedPrefabs[_name];
        //    _prefab.transform.position = _position;
        //    _prefab.SetActive(true);

        //    foreach(GameObject _go in _spawnedPrefabs.Values)
        //    {
        //        if (_go.name != _name)
        //            _go.SetActive(false);
        //    }
        //}

        
    }
}
