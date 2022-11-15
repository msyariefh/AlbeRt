using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AlbeRt.AREnvironment
{
    public class AREnvironmentView : MonoBehaviour
    {
        [SerializeField] Button _backHomeButton;
        [SerializeField] string _homeSceneName;

        private void Awake()
        {
            _backHomeButton.onClick.RemoveAllListeners();
            _backHomeButton.onClick.AddListener(OnBackHomeClicked);
        }
        private void OnBackHomeClicked()
        {
            SceneManager.LoadScene(_homeSceneName);
        }
    }
}

