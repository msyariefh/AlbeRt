using UnityEngine;

namespace AlbeRt.AREnvironment
{
    public class SpinAnimation3D : MonoBehaviour
    {
        private float _inputX;
        private bool _isDragging = false;
        private bool _isScalling = false;
        [SerializeField] private float _rotationSpeed;
        private float _rotateSpeed = 0;
        private float _defaultDistance = 0;
        private Vector3 _defaultScale = Vector3.zero;
        private float _lastXtouched;

        private void OnEnable()
        {
            _rotateSpeed = _rotationSpeed;
        }

        private void Awake()
        {
            _defaultScale = transform.localScale;
        }
        private void Update()
        {
            if (!_isScalling)
                transform.Rotate(Vector3.up, Time.deltaTime * _rotateSpeed * 10f);

            int _totalTouches = Input.touchCount;

            if (_totalTouches == 1)
            {
                DragProcess();
                return;
            }
            if (_totalTouches < 2)
            {
                _isScalling = false;
                _defaultDistance = 0;
                return;
            }

            ScalingProcess();

        }
        private void ScalingProcess()
        {
            if (_defaultDistance == 0)
            {
                _defaultDistance = Mathf.Sqrt(Input.touches[0].position.sqrMagnitude + Input.touches[1].position.sqrMagnitude);
                _isScalling = true;
                return;
            }

            float _currentDistance = Mathf.Sqrt(Input.touches[0].position.sqrMagnitude + Input.touches[1].position.sqrMagnitude);
            float _scaleFactor = (_currentDistance - _defaultDistance) / _defaultDistance * .75f;

            Vector3 _newScale = _scaleFactor * Vector3.one;
            Vector3 _tempScale = transform.localScale + _newScale;
            if (_tempScale.x < _defaultScale.x || _tempScale.x > _defaultScale.x * 5f)
                return;
            transform.localScale += _newScale;
        }

        private void DragProcess()
        {

            Touch _touchInput = Input.GetTouch(0);
            Vector3 _position = _touchInput.position;

            if (_touchInput.phase == TouchPhase.Began)
            {
                Ray _ray = Camera.main.ScreenPointToRay(_position);

                if (Physics.Raycast(_ray, out RaycastHit _hit))
                {
                    if (_hit.collider.gameObject == gameObject)
                    {
                        _rotateSpeed = 0;
                        _isDragging = true;
                        _lastXtouched = _position.x;
                    }
                }
            }
            if (_isDragging && _touchInput.phase == TouchPhase.Moved)
            {
                _inputX += (_position.x - _lastXtouched) * .01f ;
                print(_inputX);
            }

            if (_isDragging && (_touchInput.phase == TouchPhase.Ended || _touchInput.phase == TouchPhase.Canceled))
            {
                _isDragging = false;
                _rotateSpeed = _rotationSpeed;
            }
        }
        private void LateUpdate()
        {
            if(_isDragging && !_isScalling){
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, -_inputX, 0), 0.4f);
            }
        }
    }
}

