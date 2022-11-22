//using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AlbeRt.AREnvironment
{
    public class ARObjectScale : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button _upScaleButton;
        [SerializeField] private Button _downScaleButton;
        [SerializeField] private Button _resetButton;

        //[Header("Indicators")]
        //[SerializeField] private TMP_Text _indicatorScaling;

        [Header("Up Down Scale constraints")]
        [SerializeField] [Range(0.1f, 0.5f)] private float _minimumScale;
        [SerializeField] [Range(1.1f, 10f)] private float _maximumScale;
        [SerializeField] [Range(0.1f, 1.0f)] private float _steps;

        // Local
        Vector3 _defaultScale;
        float _scaleFactor;

        private void Awake()
        {
            _defaultScale = transform.localScale;
            _scaleFactor = 1f;
        }

        private void OnEnable()
        {
            _upScaleButton.onClick.AddListener(OnUpScale);
            _downScaleButton.onClick.AddListener(OnDownScale);
            _resetButton.onClick.AddListener(OnReset);
        }
        private void OnDisable()
        {
            _upScaleButton.onClick.RemoveListener(OnUpScale);
            _downScaleButton.onClick.RemoveListener(OnDownScale);
            _resetButton.onClick.RemoveListener(OnReset);
        }
        private void OnUpScale()
        {
            if (_scaleFactor >= _maximumScale)
                return;
            if (_scaleFactor + _steps >= _maximumScale)
                _scaleFactor = _maximumScale;
            else
                _scaleFactor += _steps;

            TweenScale();
        }
        private void OnDownScale()
        {
            if (_scaleFactor <= _minimumScale)
                return;
            if (_scaleFactor - _steps <= _minimumScale)
                _scaleFactor = _minimumScale;
            else
                _scaleFactor -= _steps;

            TweenScale();
        }
        private void OnReset()
        {
            _scaleFactor = 1.0f;

            TweenScale();
        }

        private void TweenScale()
        {
            LeanTween.scale(gameObject, _defaultScale * _scaleFactor, .25f);
        }
    }
}

