using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {
    public Image bgImg;
    public Image starPr;

    private Image star;
    private float starCount;
    private float starW;
    private float starH;
    private float randStarSize;

    private Canvas canvas;
    private float canvasW;
    private float canvasH;

    // Start is called before the first frame update
    void Start () {
        canvas = FindObjectOfType<Canvas> ();
        canvasW = canvas.GetComponent<RectTransform> ().rect.width;
        canvasH = canvas.GetComponent<RectTransform> ().rect.height;

        bgImg.GetComponent<RectTransform> ().sizeDelta = new Vector2 (canvasW, canvasH);

        starW = starPr.GetComponent<RectTransform> ().rect.width;
        starH = starPr.GetComponent<RectTransform> ().rect.height;

        starCount = (Screen.width + Screen.height) / (starH + starW);

        starCount = 300;
        InitStars ();
    }

    #region Init Stars
    public void InitStars () {
        for (var i = 0; i <= starCount; i++) {
            star = Instantiate (starPr, new Vector3 (UnityEngine.Random.Range (0, canvasW), -(UnityEngine.Random.Range (0, canvasH)), 0), Quaternion.identity);
            star.transform.SetParent (canvas.transform, false);

            // Randomize Opacity
            Image image = star.GetComponent<Image> ();
            image.color = new Color (image.color.r, image.color.g, image.color.b, UnityEngine.Random.Range (0.3f, 0.9f));

            // Randomize star sizes
            // Make the last few "weird" shapes
            if (starCount - i < (starCount / 3) / 2) {
                star.GetComponent<RectTransform> ().sizeDelta = new Vector2 (UnityEngine.Random.Range (starW, starW + 10), UnityEngine.Random.Range (starH, starH + 10));
            } else {
                randStarSize = UnityEngine.Random.Range (starW, starW + 10);
                star.GetComponent<RectTransform> ().sizeDelta = new Vector2 (randStarSize, randStarSize);
            }
        }
    }
    #endregion
}