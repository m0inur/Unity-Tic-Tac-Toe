using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarMovement : MonoBehaviour {
    public Rigidbody2D rb;

    private Canvas canvas;
    private RectTransform rectTransform;

    private float xForce;
    private float yForce;
    private float width;
    private float height;
    private float canvasW;
    private float canvasH;

    // Start is called before the first frame update
    void Start () {
        rectTransform = GetComponent<RectTransform> ();
        xForce = 0;
        yForce = 0;

        xForce = Random.Range (0.01f, 0.04f);
        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        // Canvas
        canvas = FindObjectOfType<Canvas> ();
        canvasW = canvas.GetComponent<RectTransform> ().rect.width;
        canvasH = canvas.GetComponent<RectTransform> ().rect.height;

        if (Random.Range (1, 4) != 1) {
            yForce = Random.Range (0.01f, 0.04f);
        }

        if (Random.Range (1, 3) == 1) {
            xForce = -xForce;
        }

        if (Random.Range (1, 3) == 1) {
            yForce = -yForce;
        }
    }

    // Update is called once per frame
    void FixedUpdate () {
        transform.position += transform.right * xForce * Time.deltaTime;
        transform.position += transform.up * yForce * Time.deltaTime;

        // Check if it left on x axis
        if (rectTransform.anchoredPosition.x - height > canvasW) {
            rectTransform.anchoredPosition = new Vector3 (-width, rectTransform.anchoredPosition.y);
        } else if (rectTransform.anchoredPosition.x + width < 0) {
            rectTransform.anchoredPosition = new Vector3 (canvasW + width, rectTransform.anchoredPosition.y);
            // Check if it left on y axis
        } else if (rectTransform.anchoredPosition.y > height) {
            rectTransform.anchoredPosition = new Vector3 (rectTransform.anchoredPosition.x, -(canvasH + height));
        } else if (rectTransform.anchoredPosition.y < -(canvasH + height)) {
            rectTransform.anchoredPosition = new Vector3 (rectTransform.anchoredPosition.x, height);
        }
    }
}