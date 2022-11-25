using System;
using UnityEngine;

namespace AlbeRt.AREnvironment
{
    [Serializable]
    class Object3D
    {
        [SerializeField] private string _name;
        [SerializeField] private string _nameDescription;
        [SerializeField] private GameObject _3dObject;

        public string Name => _name;
        public string Description => _nameDescription;
        public GameObject Object => _3dObject;
    }
}
