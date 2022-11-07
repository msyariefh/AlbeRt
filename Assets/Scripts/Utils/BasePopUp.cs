using UnityEngine;
using UnityEngine.UI;

namespace AlbeRt.Global.Utils
{
    public abstract class BasePopUp : MonoBehaviour, IPopUp
    {
        [SerializeField] protected GameObject _parentToBeHidden;
        [SerializeField] protected Button _popUpCloseButton;

        public virtual void TriggerPopUp()
        {
            gameObject.SetActive(true);
            _parentToBeHidden?.SetActive(false);
            InitializeButton();
        }

        protected virtual void InitializeButton()
        {
            _popUpCloseButton.onClick.RemoveAllListeners();
            _popUpCloseButton.onClick.AddListener(OnCloseButton);
            _popUpCloseButton.gameObject.SetActive(true);

        }
        protected virtual void OnCloseButton()
        {
            _popUpCloseButton.onClick.RemoveAllListeners();
            _popUpCloseButton.gameObject.SetActive(false);
            _parentToBeHidden?.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}