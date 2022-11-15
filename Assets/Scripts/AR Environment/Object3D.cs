using System;
using System.Collections.Generic;
using UnityEngine;

namespace AlbeRt.AREnvironment
{
    [Serializable]
    class Object3D
    {
        [SerializeField] private string _name;
        [SerializeField] private GameObject _3dObject;

        public string Name => _name;
        public GameObject Object => _3dObject;
    }
}
