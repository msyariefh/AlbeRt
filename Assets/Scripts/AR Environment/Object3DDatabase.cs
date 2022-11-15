using UnityEngine;
using System;

namespace AlbeRt.AREnvironment
{
    [CreateAssetMenu(menuName = "3D Object Database", fileName = "Object3DDb")]
    class Object3DDatabase : ScriptableObject
    {
        [SerializeField] private Object3D[] _object3DCollections;

        public Object3D GetObject3D(string name)
        {
            return Array.Find(_object3DCollections, obj => obj.Name == name);
        }
    }
}
