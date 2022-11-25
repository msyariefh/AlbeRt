using UnityEngine;

namespace AlbeRt.AREnvironment
{
    public class ARObjectScaleData : MonoBehaviour
    {
        Vector3 _defaultScale;
        float _scaleFactor;

        private void Awake()
        {
            _defaultScale = transform.localScale;
            _scaleFactor = 1.0f;
        }

        /// <summary>
        /// Scale Up an AR Object that being tracked/ visible in the scene.
        /// </summary>
        /// <param name="step">scale up by</param>
        /// <param name="maxScaleFactor">maximum scale factor allowed</param>
        /// <returns>Object Scale Factor</returns>
        public float ScaleUp(float step, float maxScaleFactor)
        {
            if (step < 0 || _scaleFactor >= maxScaleFactor)
                return _scaleFactor;

            if (_scaleFactor + step >= maxScaleFactor)
                _scaleFactor = maxScaleFactor;
            else
                _scaleFactor += step;


            TweenScale();
            return _scaleFactor;
        }

        /// <summary>
        /// Scale Down an AR Object that being tracked/ visible in the scene.
        /// </summary>
        /// <param name="step">Scale down by</param>
        /// <param name="minScaleFactor">minimum scale factor allowed</param>
        /// <returns>Object Scale Factor</returns>
        public float ScaleDown(float step, float minScaleFactor)
        {
            if (step < 0 || _scaleFactor <= minScaleFactor)
                return _scaleFactor;

            if (_scaleFactor - step <= minScaleFactor)
                _scaleFactor = minScaleFactor;
            else
                _scaleFactor -= step;


            TweenScale();
            return _scaleFactor;
        }

        /// <summary>
        /// Reset the local scale of an AR object.
        /// </summary>
        /// <returns>Object Scale Factor</returns>
        public float ResetScale()
        {
            _scaleFactor = 1.0f;
            TweenScale();
            return _scaleFactor;
        }

        private void TweenScale()
        {
            LeanTween.scale(gameObject, _defaultScale * _scaleFactor, .25f);
        }

    }
}