using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowConfetti : MonoBehaviour {
    public Transform pfConfetti;
    public Color[] colorArr;

    private List<Confetti> confList;
    private float spawnTimer;
    private const float spawnTimerMax = 0.11f;
    private TicTacToeCreatorAI gameCreatorScript;

    private void Start () {
        confList = new List<Confetti> ();
        GameObject gameControllerObj = GameObject.FindWithTag ("GameController");

        if (gameControllerObj != null) {
            gameCreatorScript = gameControllerObj.GetComponent<TicTacToeCreatorAI> ();
        } else {
            Debug.Log ("Cannot find 'TicTacToeCreatorAI' object");
        }
    }

    private void Update () {
        // If the game is over and ai won
        foreach (Confetti confetti in new List<Confetti> (confList)) {
            if (confetti.Update ()) {
                confList.Remove (confetti);
            }
        }

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f) {
            spawnTimer += spawnTimerMax;
            int spawnAmount = Random.Range (1, 4);

            for (var i = 0; i < spawnAmount; i++) {
                SpawnConfetti ();
            }
        }
    }

    private void SpawnConfetti () {
        float width = FindObjectOfType<Canvas> ().GetComponent<RectTransform> ().rect.width;
        float height = FindObjectOfType<Canvas> ().GetComponent<RectTransform> ().rect.height;

        Vector2 anchoredPosition = new Vector2 (Random.Range (-width / 2f, width / 2f), height / 2f);
        Confetti confetti = new Confetti (pfConfetti, transform, anchoredPosition, -height / 2f);

        confList.Add (confetti);
    }

    private class Confetti {
        private Transform transform;
        private RectTransform rectTransform;
        private Vector2 anchoredPosition;

        private Vector3 eular;
        private Vector2 moveAmount;
        private float eularSpeed;
        private float minimumY;

        public Confetti (Transform prefab, Transform container, Vector2 anchoredPosition, float minimumY) {
            this.anchoredPosition = anchoredPosition;
            this.minimumY = minimumY;

            transform = Instantiate (prefab, container);
            rectTransform = transform.GetComponent<RectTransform> ();
            rectTransform.anchoredPosition = anchoredPosition;

            // Size
            rectTransform.sizeDelta *= Random.Range (.8f, 1.2f);

            eular = new Vector3 (0, 0, Random.Range (0, 360f));
            eularSpeed *= Random.Range (0, 2) == 0 ? 1f : -1f;
            transform.localEulerAngles = eular;

            moveAmount = new Vector2 (0, Random.Range (-90f, -190f));
        }

        public bool Update () {
            anchoredPosition += moveAmount * Time.deltaTime;
            rectTransform.anchoredPosition = anchoredPosition;

            eular.z += eularSpeed + Time.deltaTime;
            transform.localEulerAngles = eular;

            if (anchoredPosition.y < minimumY) {
                Destroy (transform.gameObject);
                return true;
            } else {
                return false;
            }
        }
    }
}