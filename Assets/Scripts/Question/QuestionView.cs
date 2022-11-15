using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Gpm.WebView;

namespace AlbeRt.Question
{
    public class QuestionView : MonoBehaviour
    {
        [SerializeField] private string _homeSceneName;
        [SerializeField] private string _preTestUrl;
        [SerializeField] private string _postTestUrl;
        [SerializeField] private Button _backHomeButton;
        [SerializeField] private Button _preTestButton;
        [SerializeField] private Button _postTestButton;

        private void OnEnable()
        {
            RemoveAllButtonListeners();
            _backHomeButton.onClick.AddListener(OnBackHomeButtonClicked);
            _preTestButton.onClick.AddListener(OnPreTestButtonClicked);
            _postTestButton.onClick.AddListener(OnPostTestButtonClicked);
        }
        private void OnDisable()
        {
            RemoveAllButtonListeners();
        }

        private void RemoveAllButtonListeners()
        {
            _backHomeButton.onClick.RemoveAllListeners();
            _preTestButton.onClick.RemoveAllListeners();
            _postTestButton.onClick.RemoveAllListeners();
        }

        private void OnBackHomeButtonClicked()
        {
            SceneManager.LoadScene(_homeSceneName);
        }
        private void OnPreTestButtonClicked()
        {
            ShowUrlFullScreen(_preTestUrl, "(AlbeRt) PRE-TEST");
        }

        private void OnPostTestButtonClicked()
        {
            ShowUrlFullScreen(_postTestUrl, "(AlbeRt) POST-TEST");
        }
        private void ShowUrlFullScreen(string url, string testTitle)
        {
            GpmWebView.ShowUrl(
                url,
                new GpmWebViewRequest.Configuration()
                {
                    style = GpmWebViewStyle.FULLSCREEN,
                    orientation = GpmOrientation.PORTRAIT,
                    isClearCookie = true,
                    isClearCache = true,
                    isNavigationBarVisible = true,
                    navigationBarColor = "#4B96E6",
                    title = testTitle,
                    isBackButtonVisible = true,
                    isForwardButtonVisible = true,
                    supportMultipleWindows = true,
#if UNITY_IOS
            contentMode = GpmWebViewContentMode.MOBILE
#endif
        },
                OnCallback,
                new List<string>()
                {
            "USER_ CUSTOM_SCHEME"
                });
        }

        private void OnCallback(
            GpmWebViewCallback.CallbackType callbackType,
            string data,
            GpmWebViewError error)
        {
            Debug.Log("OnCallback: " + callbackType);
            switch (callbackType)
            {
                case GpmWebViewCallback.CallbackType.Open:
                    if (error != null)
                    {
                        Debug.LogFormat("Fail to open WebView. Error:{0}", error);
                    }
                    break;
                case GpmWebViewCallback.CallbackType.Close:
                    Screen.orientation = ScreenOrientation.LandscapeLeft;
                    if (error != null)
                    {
                        Debug.LogFormat("Fail to close WebView. Error:{0}", error);
                    }
                    break;
                case GpmWebViewCallback.CallbackType.PageLoad:
                    if (string.IsNullOrEmpty(data) == false)
                    {
                        Debug.LogFormat("Loaded Page:{0}", data);
                    }
                    break;
                case GpmWebViewCallback.CallbackType.MultiWindowOpen:
                    Debug.Log("MultiWindowOpen");
                    break;
                case GpmWebViewCallback.CallbackType.MultiWindowClose:
                    Debug.Log("MultiWindowClose");
                    break;
                case GpmWebViewCallback.CallbackType.Scheme:
                    if (error == null)
                    {
                        if (data.Equals("USER_ CUSTOM_SCHEME") == true || data.Contains("CUSTOM_SCHEME") == true)
                        {
                            Debug.Log(string.Format("scheme:{0}", data));
                        }
                    }
                    else
                    {
                        Debug.Log(string.Format("Fail to custom scheme. Error:{0}", error));
                    }
                    break;
                case GpmWebViewCallback.CallbackType.GoBack:
                    Debug.Log("GoBack");
                    break;
                case GpmWebViewCallback.CallbackType.GoForward:
                    Debug.Log("GoForward");
                    break;
            }
        }
    }
}
