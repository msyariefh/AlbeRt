using UnityEngine;
using UnityEngine.XR.ARFoundation;
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

        }
        private void OnEnable()
        {
            _trackedImageManager.trackedImagesChanged += OnImageChanged;
        }

        private void OnDisable()
        {
            _trackedImageManager.trackedImagesChanged -= OnImageChanged;
        }

        private void InstantiateNewPrefab(string name, GameObject go)
        {
            GameObject _newPrefab = Instantiate(go, Vector3.zero, Quaternion.identity);
            _spawnedPrefabs.Add(name, _newPrefab);
        }

        private void OnImageChanged(ARTrackedImagesChangedEventArgs args)
        {
            foreach(ARTrackedImage _trackedImage in args.added)
            {
                UpdateImage(_trackedImage);
            }

            foreach (ARTrackedImage _trackedImage in args.updated)
            {
                UpdateImage(_trackedImage);
            }

            foreach (ARTrackedImage _trackedImage in args.removed)
            {
                _spawnedPrefabs[_trackedImage.name].SetActive(false);
            }
        }

        private void UpdateImage(ARTrackedImage trackedImage)
        {
            string _name = trackedImage.referenceImage.name;
            Vector3 _position = trackedImage.transform.position;

            if (!_spawnedPrefabs.ContainsKey(_name))
            {
                var _temp = _database.GetObject3D(_name);
                InstantiateNewPrefab(_temp.Name, _temp.Object);
            }

            GameObject _prefab = _spawnedPrefabs[name];
            _prefab.transform.position = _position;
            _prefab.SetActive(true);

            foreach(GameObject _go in _spawnedPrefabs.Values)
            {
                if (_go.name != name)
                    _go.SetActive(false);
            }
        }

        
    }
}
