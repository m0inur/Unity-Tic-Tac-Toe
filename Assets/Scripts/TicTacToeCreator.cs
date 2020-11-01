using System;
using UnityEngine;

public class TicTacToeCreator : MonoBehaviour {
    #region Variables
    public GameObject line;
    public GameObject card;
    public Canvas canvas;

    private GameObject lineGen;
    private LineRenderer lineRend;

    public float topOffset;
    public float boxOffset;

    public int[, ] values;
    public int grid;
    public int valuesLen;

    public bool isGameOver;
    public bool P1;

    private GameObject box;
    private RectTransform rt;

    private Vector3 lineSpawnPos;
    private Vector3 lineDrawPos1;
    private Vector3 lineDrawPos;
    private Vector3 destination;
    private Vector3 origin;

    private float counter;
    private float dist;
    private float size;
    private float cardW;
    private float cardH;
    private float canvasW;
    private float canvasH;
    private float gapX;
    public float drawSpeed;
    public float drawDelay;
    // private float drawSpeed = 20f;
    // private float drawDelay = 0.02f;

    private float boxGridHeight;
    private int boxCount;
    private int colCount;
    private TagBox boxScript;

    private int n;
    private bool matchedAntiDiag = true;
    private bool matchedDiag = true;
    private bool matchedHori = true;
    private bool matchedRow = true;
    private bool animateLine = false;
    #endregion

    #region Initially set variables values
    void Start () {
        values = new int[grid, grid];
        boxCount = 0;
        colCount = 0;
        valuesLen = 0;
        n = grid;
        lineDrawPos1 = new Vector3 (0, 0, 0);
        origin = new Vector3 (0, 0, 0);

        // Canvas
        canvas = FindObjectOfType<Canvas> ();
        canvasW = canvas.GetComponent<RectTransform> ().rect.width;
        canvasH = canvas.GetComponent<RectTransform> ().rect.height;

        // Booleans
        matchedAntiDiag = true;
        matchedDiag = true;
        matchedHori = true;
        matchedRow = true;
        isGameOver = false;
        P1 = true;

        // Calculate the size of the boxes
        size = Screen.width / grid - boxOffset;
        CreateBoxGrid ();
    }
    #endregion

    #region Create grid
    private void CreateBoxGrid () {
        // Give size of boxes
        card.GetComponent<RectTransform> ().sizeDelta = new Vector2 (size, size);

        lineGen = Instantiate (line, lineSpawnPos, Quaternion.identity) as GameObject;
        lineGen.transform.SetParent (canvas.transform, false);
        lineRend = lineGen.GetComponent<LineRenderer> ();

        for (int i = 0; i < grid; i++) {
            // Reset the gap on the x axis every time
            gapX = size / 2 + boxOffset;

            for (int j = 0; j < grid; j++) {
                box = Instantiate (card, new Vector3 (gapX, topOffset - boxGridHeight, 0), Quaternion.identity) as GameObject;
                box.transform.SetParent (canvas.transform.transform, false);
                boxScript = box.GetComponent<TagBox> ();

                boxScript.boxColNum = colCount;
                boxScript.boxRowNum = boxCount;

                // Update gap on X axis after evey box
                gapX += size + boxOffset;
                boxCount++;
            }
            boxGridHeight += size + boxOffset;
            colCount++;
            boxCount = 0;
        }
    }

    #endregion

    private void Update () {
        if (animateLine) {
            if (counter < dist) {
                counter += drawDelay / drawSpeed;
                float x = Mathf.Lerp (0, dist, counter);

                Vector3 A = origin;
                Vector3 B = destination;

                Vector3 ALine = x * Vector3.Normalize (B - A) + A;

                lineRend.SetPosition (1, ALine);
            }
        }
    }

    public void ChangePlayer () {
        if (P1) {
            P1 = false;
        } else {
            P1 = true;
        }
    }

    public void HasMatched () {
        matchedAntiDiag = true;
        matchedDiag = true;
        matchedHori = true;
        matchedRow = true;

        for (int i = 0; i < n; i++) {
            if (i < n - 1) {
                // Detect Diagnal and Anti Diagnal
                if (matchedAntiDiag) {
                    if (values[i, (n - 1) - i] == 0 || values[i, (n - 1) - i] != values[i + 1, (n - 1) - i - 1]) {
                        matchedAntiDiag = false;
                    }
                }

                if (matchedDiag) {
                    if (values[i, i] == 0 || values[i, i] != values[i + 1, i + 1]) {
                        matchedDiag = false;
                    }
                }
            }

            for (int j = 0; j < n - 1; j++) {
                if (matchedRow) {
                    // If the row has a gap or doesnt match value then this row cant match
                    if (values[i, j] == 0 || values[i, j] != values[i, j + 1]) {
                        matchedRow = false;
                    }
                }

                if (matchedHori) {
                    if (values[j, i] == 0 || values[j, i] != values[j + 1, i]) {
                        matchedHori = false;
                    }
                }
            }

            if (matchedRow) {
                lineSpawnPos = new Vector3 (0, topOffset - (size * i) - (boxOffset * i), -1);
                lineDrawPos = new Vector3 (canvasW, 0, 0);

                GameOver ();
                return;
            }

            if (matchedHori) {
                lineSpawnPos = new Vector3 (((size + boxOffset) * (i + 1)) - size / 2, topOffset - boxGridHeight + size / 2 + boxOffset, -1);
                lineDrawPos = new Vector3 (0, boxGridHeight - boxOffset, 0);

                GameOver ();
                return;
            }

            matchedHori = true;
            matchedRow = true;
        }

        if (matchedDiag) {
            lineSpawnPos = new Vector3 (11, topOffset + size / 2, -1);
            lineDrawPos = new Vector3 (boxGridHeight - boxOffset, -(boxGridHeight - boxOffset), 0);

            GameOver ();
            return;
        }

        if (matchedAntiDiag) {
            lineSpawnPos = new Vector3 (11, topOffset - boxGridHeight + size / 2 + boxOffset, -1);
            lineDrawPos = new Vector3 (boxGridHeight - boxOffset, boxGridHeight - boxOffset, 0);

            GameOver ();
            return;
        }
    }

    public void GameOver () {
        if (!isGameOver) {
            isGameOver = true;
            animateLine = true;
            destination = lineDrawPos;
            dist = Vector3.Distance (origin, destination);

            lineGen = Instantiate (line, lineSpawnPos, Quaternion.identity) as GameObject;
            lineGen.transform.SetParent (canvas.transform, false);
            lineRend = lineGen.GetComponent<LineRenderer> ();

            if (!P1) {
                Debug.Log ("Player 1 won");
            } else {
                Debug.Log ("Player 2 won");
            }
        }
    }
}