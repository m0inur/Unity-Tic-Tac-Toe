using UnityEngine;

namespace Particle
{
    public class StarMovement : MonoBehaviour {
        public Rigidbody2D rb;

        private Canvas _canvas;
        private RectTransform _rectTransform;

        private float _xForce;
        private float _yForce;

        private float _width;
        private float _height;
    
        private float _canvasW;
        private float _canvasH;

        // Start is called before the first frame update
        void Start () {
            _rectTransform = GetComponent<RectTransform> ();
            _xForce = 0;
            _yForce = 0;

            _xForce = Random.Range (0.01f, 0.04f);
            _width = _rectTransform.rect.width;
            _height = _rectTransform.rect.height;

            // Canvas
            _canvas = FindObjectOfType<Canvas> ();
            _canvasW = _canvas.GetComponent<RectTransform> ().rect.width;
            _canvasH = _canvas.GetComponent<RectTransform> ().rect.height;

            if (Random.Range (1, 4) != 1) {
                _yForce = Random.Range (0.01f, 0.04f);
            }

            if (Random.Range (1, 3) == 1) {
                _xForce = -_xForce;
            }

            if (Random.Range (1, 3) == 1) {
                _yForce = -_yForce;
            }
        }

        // Update is called once per frame
        private void FixedUpdate () {
            transform.position += _xForce * Time.deltaTime * transform.right;
            transform.position +=  _yForce * Time.deltaTime * transform.up;

            // Check if it left on x axis
            if (_rectTransform.anchoredPosition.x - _height > _canvasW) {
                _rectTransform.anchoredPosition = new Vector3 (-_width, _rectTransform.anchoredPosition.y);
            } else if (_rectTransform.anchoredPosition.x + _width < 0) {
                _rectTransform.anchoredPosition = new Vector3 (_canvasW + _width, _rectTransform.anchoredPosition.y);
                // Check if it left on y axis
            } else if (_rectTransform.anchoredPosition.y > _height) {
                _rectTransform.anchoredPosition = new Vector3 (_rectTransform.anchoredPosition.x, -(_canvasH + _height));
            } else if (_rectTransform.anchoredPosition.y < -(_canvasH + _height)) {
                _rectTransform.anchoredPosition = new Vector3 (_rectTransform.anchoredPosition.x, _height);
            }
        }
    }
}