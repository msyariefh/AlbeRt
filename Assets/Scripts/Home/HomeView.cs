using AlbeRt.Global.Utils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace AlbeRt.Home
{
    public class HomeView : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button _arEnvironmentButton;
        [SerializeField] private Button _learningMaterialsButton;
        [SerializeField] private Button _guideButton;
        [SerializeField] private Button _questionsButton;
        [SerializeField] private Button _aboutButton;
        [SerializeField] private Button _competenciesButton;
        [SerializeField] private Button _exitButton;

        [Header("PopUps")]
        [SerializeField] private GameObject _guidePopUp;
        [SerializeField] private GameObject _aboutPopUp;
        [SerializeField] private GameObject _competenciesPopUp;

        [Header("Scene Names")]
        [SerializeField] private string _arEnvironmentScene;
        [SerializeField] private string _learningMaterialsScene;
        [SerializeField] private string _questionsScene;


        private void OnDisable()
        {
            RemoveAllButtonListeners();
        }

        private void OnEnable()
        {
            RemoveAllButtonListeners();
            AddAllListeners();

        }

        private void AddAllListeners()
        {
            _arEnvironmentButton.onClick.AddListener(ArButtonListener);
            _learningMaterialsButton.onClick.AddListener(LearningButtonListener);
            _guideButton.onClick.AddListener(GuideButtonListener);
            _questionsButton.onClick.AddListener(QestionButtonListener);
            _aboutButton.onClick.AddListener(AboutButtonListener);
            _competenciesButton.onClick.AddListener(CompetencyButtonListener);
            _exitButton.onClick.AddListener(ExitButtonListener);
        }

        private void RemoveAllButtonListeners()
        {
            _arEnvironmentButton.onClick.RemoveAllListeners();
            _learningMaterialsButton.onClick.RemoveAllListeners();
            _guideButton.onClick.RemoveAllListeners();
            _questionsButton.onClick.RemoveAllListeners();
            _aboutButton.onClick.RemoveAllListeners();
            _competenciesButton.onClick.RemoveAllListeners();
            _exitButton.onClick.RemoveAllListeners();
        }

        private void ArButtonListener()
        {
            SceneManager.LoadScene(_arEnvironmentScene);
        }
        private void LearningButtonListener()
        {
            SceneManager.LoadScene(_learningMaterialsScene);
        }
        private void GuideButtonListener()
        {
            IPopUp _popUpInterface = _guidePopUp.GetComponent<IPopUp>();
            _popUpInterface?.TriggerPopUp();
        }
        private void QestionButtonListener()
        {
            SceneManager.LoadScene(_questionsScene);
        }
        private void AboutButtonListener()
        {
            IPopUp _popUpInterface = _aboutPopUp.GetComponent<IPopUp>();
            _popUpInterface?.TriggerPopUp();
        }
        private void CompetencyButtonListener()
        {
            IPopUp _popUpInterface = _competenciesPopUp.GetComponent<IPopUp>();
            _popUpInterface?.TriggerPopUp();
        }
        private void ExitButtonListener()
        {
            Application.Quit();
        }
    }
}
