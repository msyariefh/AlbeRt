using TMPro;
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

        [Header("Indicators")]
        [SerializeField] private TMP_Text _indicatorScaling;

        [Header("Up Down Scale constraints")] 
        [SerializeField] [Range(0.1f, 0.5f)] private float _minimumScale;
        [SerializeField] [Range(1.1f, 10f)] private float _maximumScale;
        [SerializeField] [Range(0.1f, 1.0f)] private float _steps;

        ARObjectScaleData _arObjectData;

        private void Awake()
        {
            UpdateScaleIndicatorValue(1.0f);
            UpdateButtonsInteractable(false);
        }

        public void ChangeReferenceObject(GameObject obj)
        {
            _arObjectData = obj.GetComponent<ARObjectScaleData>();
            if (_arObjectData != null)
            {
                UpdateButtonsInteractable(true);
                UpdateScaleIndicatorValue(1.0f);
            }
            else
            {
                UpdateButtonsInteractable(false);
            }
        }

        private void UpdateButtonsInteractable(bool current)
        {
            _upScaleButton.interactable = current;
            _downScaleButton.interactable = current;
            _resetButton.interactable = current;
        }

        private void UpdateScaleIndicatorValue(float val)
        {
            var _temp = $"Scale: {val:.0}x";
            _indicatorScaling.text = _temp;
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
            var _scales = _arObjectData.ScaleUp(_steps, _maximumScale);
            UpdateScaleIndicatorValue(_scales);
        }
        private void OnDownScale()
        {
            var _scales = _arObjectData.ScaleDown(_steps, _minimumScale);
            UpdateScaleIndicatorValue(_scales);
        }
        private void OnReset()
        {
            var _scales = _arObjectData.ResetScale();
            UpdateScaleIndicatorValue(_scales);
        }

    }
}

