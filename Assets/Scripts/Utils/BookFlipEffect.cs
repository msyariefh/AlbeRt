using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AlbeRt.Global.Utils
{
    [RequireComponent(typeof(Canvas))]
    public class BookFlipEffect : MonoBehaviour
    {
        public enum FlipDirection
        {
            RightLeft, LeftRight
        }
        public enum DragPosition { LEFT, RIGHT }

        [Header("Book Configuration")]
        [SerializeField] private RectTransform _bookPanel;
        [SerializeField] private Sprite _bookBackground;
        [SerializeField] private GameObject[] _bookPagePrefabs;
        [SerializeField] private bool _interactable;

        [Header("Estetique")]
        [SerializeField] private Image _clippingPlane;
        [SerializeField] private Image _nextPageClip;
        [SerializeField] private Image _leftToRightShadow;
        [SerializeField] private Image _rightToLeftShadow;
        [SerializeField] private GameObject _leftPage;
        [SerializeField] private GameObject _rightPage;
        [SerializeField] private GameObject _nextLeft;
        [SerializeField] private GameObject _nextRight;

        private RectTransform GetRectTransform(GameObject go)
        {
            return go.GetComponent<RectTransform>();
        }

        [SerializeField] private UnityEvent OnBookFlip;
        private Canvas _canvas;

        private Vector2 _radius;
        private Vector3 _spineBottom;
        private Vector3 _spineTop;
        private Vector3 _bottomRightEdge;
        private Vector3 _bottomLeftEdge;
        private Vector3 _followPoint;
        private Vector3 _pageCorner;

        private int _currentPage = 0;
        private bool _isDraggingPage = false;
        private FlipDirection _flipDirection;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
        }

        private void Start()
        {
            _leftPage.SetActive(false);
            _rightPage.SetActive(false);

            UpdateContentPage();
            CalculateCriticalCurlPoints();

            float _height = _bookPanel.rect.height;
            float _width = _bookPanel.rect.width / 2f;

            _nextPageClip.rectTransform.sizeDelta = new Vector2(
                _width, _height * 3);
            _clippingPlane.rectTransform.sizeDelta = new Vector2(
                _width * 2 + _height, _height * 3);

            float _hypotenous = Mathf.Sqrt(Mathf.Pow(_width, 2) + Mathf.Pow(_height, 2));
            float _shadowPageHeight = _width / 2 + _hypotenous;

            _rightToLeftShadow.rectTransform.sizeDelta = new Vector2(
                _width, _shadowPageHeight);
            _rightToLeftShadow.rectTransform.pivot = new Vector2(
                1, _width / 2 / _shadowPageHeight);

            _leftToRightShadow.rectTransform.sizeDelta = new Vector2(
                _width, _shadowPageHeight);
            _leftToRightShadow.rectTransform.pivot = new Vector2(
                0, _width / 2 / _shadowPageHeight);
        }

        private void Update()
        {
            if (_isDraggingPage && _interactable)
                UpdateBook();
        }
        public void OnDragLeft()
        {
            OnMouseDragPage(DragPosition.LEFT);
        }
        public void OnDragRight()
        {
            OnMouseDragPage(DragPosition.RIGHT);
        }

        public void OnMouseDragPage(DragPosition dragPosition)
        {
            print(dragPosition);
            Vector3 _inputPoint = ToWorldPoint(Input.mousePosition);
            switch (dragPosition)
            {
                case DragPosition.LEFT:
                    DragPageToPoint(FlipDirection.LeftRight, _inputPoint);
                    break;
                case DragPosition.RIGHT:
                    DragPageToPoint(FlipDirection.RightLeft, _inputPoint);
                    break;
            }
        }
        public void OnMouseDragRelease()
        {
            if (_interactable)
                ReleasePage();
        }

        private void CalculateCriticalCurlPoints()
        {
            float _pageHeight = _bookPanel.rect.height;
            float _pageWidth = _bookPanel.rect.width;
            _spineBottom = new(0, -_pageHeight / 2);
            _spineTop = new(0, _pageHeight / 2);
            _bottomRightEdge = new Vector3(_pageWidth / 2, -_pageHeight / 2);
            _bottomLeftEdge = new Vector3(-_pageWidth / 2, -_pageHeight / 2);

            _radius = new Vector2(Vector2.Distance(_spineBottom, _bottomRightEdge),
                Mathf.Sqrt(Mathf.Pow(_pageWidth / 2.0f, 2) + Mathf.Pow(_pageHeight, 2)));
        }

        private Vector2 ToWorldPoint(Vector3 position)
        {
            Vector2 _localPosition = position;
            switch (_canvas.renderMode)
            {
                case RenderMode.ScreenSpaceCamera:
                    Vector2 _worldPosition = _canvas.worldCamera.ScreenToWorldPoint(
                        new Vector3(position.x, position.y, _canvas.planeDistance));
                    _localPosition = _bookPanel.InverseTransformPoint(_worldPosition);
                    break;
                case RenderMode.ScreenSpaceOverlay:
                    _localPosition = _bookPanel.InverseTransformPoint(position);
                    break;
                case RenderMode.WorldSpace:
                    Ray _raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
                    Plane _simulationPlane = new(transform.TransformPoint(_bottomRightEdge),
                        transform.TransformPoint(_bottomLeftEdge), transform.TransformPoint(_spineTop));
                    _simulationPlane.Raycast(_raycast, out float _distance);
                    _localPosition = _bookPanel.InverseTransformPoint(_raycast.GetPoint(_distance));
                    break;
            }

            return _localPosition;
        }

        private void UpdateBook()
        {
            _followPoint = Vector3.Lerp(
                _followPoint, ToWorldPoint(Input.mousePosition),
                Time.deltaTime * 10);
            UpdatePageToPoint(_flipDirection, _followPoint);
        }

        private void UpdatePageToPoint(FlipDirection direction, Vector3 followPosition)
        {
            float _clipAngle, _cornerT1Angle;
            Vector3 _t1;
            switch (direction)
            {
                case FlipDirection.LeftRight:
                    _flipDirection = FlipDirection.LeftRight;

                    _leftToRightShadow.transform.SetParent(_clippingPlane.transform, true);
                    _leftToRightShadow.transform.localPosition = Vector3.zero;
                    _leftToRightShadow.transform.localEulerAngles = Vector3.zero;

                    _leftPage.transform.SetParent(_clippingPlane.transform, true);
                    _rightPage.transform.SetParent(_bookPanel.transform, true);
                    _rightPage.transform.localEulerAngles = Vector3.zero;
                    _nextLeft.transform.SetParent(_bookPanel.transform, true);

                    _pageCorner = CalculateCornerPosition(followPosition);
                    _clipAngle = CalculateClippingAngle(_bottomLeftEdge, out _t1);
                    _clipAngle = (_clipAngle + 180) % 180;

                    _clippingPlane.transform.localEulerAngles = new Vector3(0, 0, _clipAngle - 90);
                    _clippingPlane.transform.position = _bookPanel.TransformPoint(_t1);

                    // Position and angle of a to-be-updated page
                    _leftPage.transform.position = _bookPanel.TransformPoint(_pageCorner);
                    _cornerT1Angle = Mathf.Atan2(
                        _t1.y - _pageCorner.y, _t1.x - _pageCorner.x) * Mathf.Rad2Deg;
                    _leftPage.transform.localEulerAngles = new Vector3(0, 0, 
                        _cornerT1Angle - 90 - _clipAngle);

                    _nextPageClip.transform.localEulerAngles = new Vector3(0, 0, _clipAngle - 90);
                    _nextPageClip.transform.position = _bookPanel.TransformPoint(_t1);
                    _nextLeft.transform.SetParent(_nextPageClip.transform, true);
                    _rightPage.transform.SetParent(_clippingPlane.transform, true);
                    _rightPage.transform.SetAsFirstSibling();

                    _leftToRightShadow.rectTransform.SetParent(GetRectTransform(_leftPage), true);

                    break;
                case FlipDirection.RightLeft:
                    _flipDirection = FlipDirection.RightLeft;

                    _rightToLeftShadow.transform.SetParent(_clippingPlane.transform, true);
                    _rightToLeftShadow.transform.localPosition = Vector3.zero;
                    _rightToLeftShadow.transform.localEulerAngles = Vector3.zero;

                    _rightPage.transform.SetParent(_clippingPlane.transform, true);
                    _leftPage.transform.SetParent(_bookPanel.transform, true);
                    _leftPage.transform.localEulerAngles = Vector3.zero;
                    _nextRight.transform.SetParent(_bookPanel.transform, true);

                    _pageCorner = CalculateCornerPosition(followPosition);
                    _clipAngle = CalculateClippingAngle(_bottomRightEdge, out _t1);
                    if (_clipAngle > -90) _clipAngle += 180;

                    _clippingPlane.rectTransform.pivot = new Vector2(1, .35f);
                    _clippingPlane.transform.localEulerAngles = new Vector3(0, 0, _clipAngle + 90);
                    _clippingPlane.transform.position = _bookPanel.TransformPoint(_t1);

                    // Position and angle of a to-be-updated page
                    _rightPage.transform.position = _bookPanel.TransformPoint(_pageCorner);
                    _cornerT1Angle = Mathf.Atan2(
                        _t1.y - _pageCorner.y, _t1.x - _pageCorner.x) * Mathf.Rad2Deg;
                    _rightPage.transform.localEulerAngles = new Vector3(0, 0,
                        _cornerT1Angle - 90 - _clipAngle);

                    _nextPageClip.transform.localEulerAngles = new Vector3(0, 0, _clipAngle + 90);
                    _nextPageClip.transform.position = _bookPanel.TransformPoint(_t1);
                    _nextRight.transform.SetParent(_nextPageClip.transform, true);
                    _leftPage.transform.SetParent(_clippingPlane.transform, true);
                    _leftPage.transform.SetAsFirstSibling();

                    _rightToLeftShadow.rectTransform.SetParent(GetRectTransform(_rightPage), true);
                    break;
            }
        }

        private Vector3 CalculateCornerPosition(Vector3 followPosition)
        {
            Vector3 _corner;
            float _followSpineBottomAngle = Mathf.Atan2(
                followPosition.y - _spineBottom.y, followPosition.x - _spineBottom.x);
            float _followSpineBottomDistance = Vector2.Distance(
                followPosition, _spineBottom);

            _ = _followSpineBottomDistance < _radius.x ? _corner = followPosition :
                _corner = new Vector3(_radius.x * Mathf.Cos(_followSpineBottomAngle),
                _radius.x * Mathf.Sin(_followSpineBottomAngle), 0) + _spineBottom;

            float _cornerSpineTopAngle = Mathf.Atan2(
                _corner.y - _spineTop.y, _corner.x - _spineTop.x);
            float _cornerSpineTopDistance = Vector2.Distance(
                _corner, _spineTop);

            _ = _cornerSpineTopDistance > _radius.y ?
                _corner = new Vector3(_radius.y * Mathf.Cos(_cornerSpineTopAngle),
                _radius.y * Mathf.Sin(_cornerSpineTopAngle), 0) + _spineTop : Vector3.zero;

            return _corner;
        }

        private float CalculateClippingAngle(Vector3 cornerPosition, out Vector3 _t1)
        {
            Vector3 _t0 = (_pageCorner + cornerPosition) / 2;
            float _t0CornerAngle = Mathf.Atan2(cornerPosition.y - _t0.y,
                cornerPosition.x - _t0.x);

            _t1 = new Vector3(
                NormalizeT1X(_t0.x - Mathf.Tan(_t0CornerAngle) * (cornerPosition.y - _t0.y))
                , _spineBottom.y, 0);

            return Mathf.Atan2(_t1.y - _t0.y, _t1.x - _t0.x) * Mathf.Rad2Deg;
        }

        private float NormalizeT1X(float t1)
        {
            if (t1 > _spineBottom.x && _spineBottom.x > _pageCorner.x
                || t1 < _spineBottom.x && _spineBottom.x < _pageCorner.x)
                return _spineBottom.x;
            return t1;
        }

        private void DragPageToPoint(FlipDirection direction, Vector3 point)
        {
            print(direction);
            print(point);
            if (_currentPage >= _bookPagePrefabs.Length || 
                _currentPage <= 0) return;

            _isDraggingPage = true;
            _flipDirection = direction;
            _nextPageClip.rectTransform.pivot = new Vector2(0, .13f);
            _clippingPlane.rectTransform.pivot = new Vector2(0, .38f);

            switch (direction)
            {
                case FlipDirection.RightLeft: 
                    
                    _leftPage.SetActive(true);
                    GetRectTransform(_leftPage).pivot = Vector2.zero;
                    _leftPage.transform.position = _nextRight.transform.position;
                    _leftPage.transform.eulerAngles = Vector3.zero;
                    if (_currentPage < _bookPagePrefabs.Length)
                        ChangePageContent(_bookPagePrefabs[_currentPage], _leftPage.transform);
                    _leftPage.transform.SetAsFirstSibling();

                    _rightPage.SetActive(true);
                    _rightPage.transform.position = _nextRight.transform.position;
                    _rightPage.transform.eulerAngles = Vector3.zero;
                    if (_currentPage < _bookPagePrefabs.Length - 1)
                        ChangePageContent(_bookPagePrefabs[_currentPage + 1], _rightPage.transform);
                    if (_currentPage < _bookPagePrefabs.Length - 2)
                        ChangePageContent(_bookPagePrefabs[_currentPage + 2], _nextRight.transform);

                    UpdatePageToPoint(direction, point);
                    break;

                case FlipDirection.LeftRight:

                    _rightPage.SetActive(true);
                    _rightPage.transform.position = _nextLeft.transform.position;
                    _rightPage.transform.eulerAngles = Vector3.zero;
                    ChangePageContent(_bookPagePrefabs[_currentPage - 1], _rightPage.transform);
                    _rightPage.transform.SetAsFirstSibling();

                    _leftPage.SetActive(true);
                    (GetRectTransform(_leftPage)).pivot = Vector2.right;
                    _leftPage.transform.position = _nextLeft.transform.position;
                    _leftPage.transform.eulerAngles = Vector3.zero;
                    if (_currentPage >= 2)
                        ChangePageContent(_bookPagePrefabs[_currentPage - 2], _leftPage.transform);
                    if (_currentPage >= 3)
                        ChangePageContent(_bookPagePrefabs[_currentPage - 3], _nextLeft.transform);

                    _rightPage.transform.SetAsFirstSibling();
                    UpdatePageToPoint(direction, point);

                    break;
            }
        }

        private void ChangePageContent(GameObject go, Transform pageTarget)
        {
            // Destroy all children
            while (pageTarget.childCount > 0)
            {
                Destroy(pageTarget.GetChild(0).gameObject);
            }

            Instantiate(go, pageTarget);
        }


        private void ReleasePage()
        {
            if (_isDraggingPage)
            {
                _isDraggingPage = false;
                float _cornerToLeft = Vector2.Distance(_pageCorner, _bottomLeftEdge);
                float _cornerToRight = Vector2.Distance(_pageCorner, _bottomRightEdge);

                if (_cornerToLeft > _cornerToRight && _flipDirection == FlipDirection.RightLeft)
                    TweenBack();
                else if (_cornerToLeft < _cornerToRight && _flipDirection == FlipDirection.LeftRight)
                    TweenBack();
                else
                    TweenForward();
            }
        }

        private void UpdateContentPage()
        {
            if (_currentPage > 0 && _currentPage <= _bookPagePrefabs.Length)
                ChangePageContent(_bookPagePrefabs[_currentPage - 1], _nextLeft.transform);
            if (_currentPage >= 0 && _currentPage < _bookPagePrefabs.Length)
                ChangePageContent(_bookPagePrefabs[_currentPage], _nextRight.transform);
        }

        private void Flip()
        {
            _ = _flipDirection == FlipDirection.RightLeft ?
                _currentPage += 2 : _currentPage -= 2;

            _nextLeft.transform.SetParent(_bookPanel.transform, true);
            _leftPage.transform.SetParent(_bookPanel.transform, true);
            _nextRight.transform.SetParent(_bookPanel.transform, true);
            _rightPage.transform.SetParent(_bookPanel.transform, true);

            _leftPage.SetActive(false);
            _rightPage.SetActive(false);

            UpdateContentPage();
            _leftToRightShadow.gameObject.SetActive(false);
            _rightToLeftShadow.gameObject.SetActive(false);
            OnBookFlip?.Invoke();

        }

        private void TweenForward()
        {
            Vector3 _target = Vector3.zero;
            switch (_flipDirection)
            {
                case FlipDirection.RightLeft:
                    _target = _bottomLeftEdge;
                    break;
                case FlipDirection.LeftRight:
                    _target = _bottomRightEdge;
                    break;
            }
            StartCoroutine(TweenTo(_target, .15f, () =>
            {
                Flip();
            }));
        }

        private void TweenBack()
        {
            switch (_flipDirection)
            {
                case FlipDirection.RightLeft:
                    StartCoroutine(TweenTo(_bottomRightEdge, .15f, () =>
                    {
                        UpdateContentPage();
                        _nextRight.transform.SetParent(_bookPanel.transform, true);
                        _rightPage.transform.SetParent(_bookPanel.transform, true);
                    }));
                    break;
                case FlipDirection.LeftRight:
                    StartCoroutine(TweenTo(_bottomLeftEdge, .15f, () =>
                    {
                        UpdateContentPage();
                        _nextLeft.transform.SetParent(_bookPanel.transform, true);
                        _leftPage.transform.SetParent(_bookPanel.transform, true);
                    }));
                    break;
            }

            _leftPage.SetActive(false);
            _rightPage.SetActive(false);
            _isDraggingPage = false;
        }

        private IEnumerator TweenTo(Vector3 to, float duration, System.Action OnFinish)
        {
            int _steps = (int)(duration / .01f);
            Vector3 _displacement = (to - _followPoint) / _steps;
            for( int i = 0; i < _steps - 1; i++)
            {
                UpdatePageToPoint(_flipDirection, _followPoint + _displacement);
                yield return new WaitForSeconds(.01f);
            }

            OnFinish?.Invoke();
        }

    }
}