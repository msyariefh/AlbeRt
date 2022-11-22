using UnityEngine;

namespace AlbeRt.AREnvironment
{
    public class SpinAnimation3D : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeed;
        private float _rotateSpeed = 0;

        private void OnEnable()
        {
            _rotateSpeed = _rotationSpeed;
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, Time.deltaTime * _rotateSpeed * 10f);

        }
       
    }
}

