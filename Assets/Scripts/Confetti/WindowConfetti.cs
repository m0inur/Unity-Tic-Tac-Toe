using System.Collections.Generic;
using UnityEngine;

namespace Confetti
{
    public class WindowConfetti : MonoBehaviour {
        public Transform pfConfetti;
        public bool stopConfetti;

        private List<Confetti> _confList;
        private float _spawnTimer;
        private float _stopTimer;
        private const float SpawnTimerMax = 0.11f;

        private void Start () {
            _confList = new List<Confetti> ();
            _stopTimer = 25f;
            stopConfetti = false;
        }

        private void Update () {
            // If the game is over and ai won
            foreach (Confetti confetti in new List<Confetti> (_confList)) {
                if (confetti.Update ()) {
                    _confList.Remove (confetti);
                }
            }

            _spawnTimer -= Time.deltaTime;

            if (_spawnTimer <= 0f) {
                _spawnTimer += SpawnTimerMax;
                int spawnAmount = Random.Range (1, 4);

                for (var i = 0; i < spawnAmount; i++) {
                    if (!stopConfetti)
                    {
                        SpawnConfetti ();
                    }
                }
            }

            if (_stopTimer > 0f && !stopConfetti)
            {
                _stopTimer -= Time.deltaTime;
            }
            else
            {
                stopConfetti = true;
            }
        }

        private void SpawnConfetti () {
            float width = FindObjectOfType<Canvas> ().GetComponent<RectTransform> ().rect.width;
            float height = FindObjectOfType<Canvas> ().GetComponent<RectTransform> ().rect.height;

            Vector2 anchoredPosition = new Vector2 (Random.Range (-width / 2f, width / 2f), height / 2f);
            Confetti confetti = new Confetti (pfConfetti, transform, anchoredPosition, -height / 2f);

            _confList.Add (confetti);
        }

        private class Confetti {
            private Transform _transform;
            private RectTransform _rectTransform;
            private Vector2 _anchoredPosition;

            private Vector3 _eular;
            private Vector2 _moveAmount;
            private float _eularSpeed;
            private float _minimumY;

            public Confetti (Transform prefab, Transform container, Vector2 anchoredPosition, float minimumY) {
                this._anchoredPosition = anchoredPosition;
                this._minimumY = minimumY;

                _transform = Instantiate (prefab, container);
                _rectTransform = _transform.GetComponent<RectTransform> ();
                _rectTransform.anchoredPosition = anchoredPosition;

                // Size
                _rectTransform.sizeDelta *= Random.Range (.8f, 1.2f);

                _eular = new Vector3 (0, 0, Random.Range (0, 360f));
                _eularSpeed *= Random.Range (0, 2) == 0 ? 1f : -1f;
                _transform.localEulerAngles = _eular;

                _moveAmount = new Vector2 (0, Random.Range (-90f, -190f));
            }

            public bool Update () {
                _anchoredPosition += _moveAmount * Time.deltaTime;
                _rectTransform.anchoredPosition = _anchoredPosition;

                _eular.z += _eularSpeed + Time.deltaTime;
                _transform.localEulerAngles = _eular;

                if (_anchoredPosition.y < _minimumY) {
                    Destroy (_transform.gameObject);
                    return true;
                } else {
                    return false;
                }
            }
        }
    }
}