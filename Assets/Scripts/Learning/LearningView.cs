using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AlbeRt.Learning
{
    public class LearningView : MonoBehaviour
    {
        [Header("Scene Control")]
        [SerializeField] private string _homeSceneName;
        [SerializeField] private Button _backHomeButton;

        private void OnEnable()
        {
            RemoveAllButtonListeners();
            _backHomeButton.onClick.AddListener(OnHomeButtonClicked);
        }

        private void OnDisable()
        {
            RemoveAllButtonListeners();
        }

        private void RemoveAllButtonListeners()
        {
            _backHomeButton.onClick.RemoveAllListeners();
        }

        private void OnHomeButtonClicked()
        {
            SceneManager.LoadScene(_homeSceneName);
        }

    }

}
