﻿using System;
using UnityEngine;

public class TicTacToeCreatorPvP : MonoBehaviour {
    #region Variables
    public GameObject line;
    public GameObject card;
    private Canvas canvas;

    public Material xMat;
    public Material oMat;

    private GameObject lineGen;
    private LineRenderer lineRend;

    public float topOffset;
    public float boxOffset;

    public int[, ] board;
    public int grid;
    public int boardLen;

    private int hasTied = 3;

    public bool isGameOver;
    public bool P1;

    private RectTransform rt;
    private TagBoxPvP boxScript;
    private GameObject box;

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

    private float boxGridHeight;
    private int rowCount;
    private int colCount;

    private int n;
    private bool animateLine = false;
    #endregion

    #region Initially set variables board
    void Start () {
        board = new int[grid, grid];
        rowCount = 0;
        colCount = 0;
        boardLen = 0;
        n = grid;
        lineDrawPos1 = new Vector3 (0, 0, 0);
        origin = new Vector3 (0, 0, 0);

        // Canvas
        canvas = FindObjectOfType<Canvas> ();
        canvasW = canvas.GetComponent<RectTransform> ().rect.width;
        canvasH = canvas.GetComponent<RectTransform> ().rect.height;

        // Booleans
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
                box.name = colCount + "" + rowCount;

                boxScript = box.GetComponent<TagBoxPvP> ();
                boxScript.boxColNum = colCount;
                boxScript.boxRowNum = rowCount;

                // Update gap on X axis after evey box
                gapX += size + boxOffset;
                rowCount++;
            }
            boxGridHeight += size + boxOffset;
            colCount++;
            rowCount = 0;
        }
    }

    #endregion

    // Animate line
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

    // Winner Checker
    public int HasMatched () {
        int n = grid;
        int rowWinner = 0;
        int colWinner = 0;
        int diagWinner = 0;
        int antiDiagWinner = 0;
        int hasTied = 3;

        for (int i = 0; i < n; i++) {
            if (i < n - 1) {
                // Detect Diagnal and Anti Diagnal
                if (antiDiagWinner > -1) {
                    antiDiagWinner = board[i, (n - 1) - i];
                    if (board[i, (n - 1) - i] == 0 || board[i, (n - 1) - i] != board[i + 1, (n - 1) - i - 1]) {
                        antiDiagWinner = -1;
                    }
                }

                if (diagWinner > -1) {
                    diagWinner = board[i, i];
                    if (board[i, i] == 0 || board[i, i] != board[i + 1, i + 1]) {
                        diagWinner = -1;
                    }
                }
            }

            for (int j = 0; j < n - 1; j++) {
                if (rowWinner > -1) {
                    rowWinner = board[i, j];
                    // If the row has a gap or doesnt match value then this row cant match
                    if (board[i, j] == 0 || board[i, j] != board[i, j + 1]) {
                        rowWinner = -1;
                    }
                }

                if (colWinner > -1) {
                    colWinner = board[j, i];
                    if (board[j, i] == 0 || board[j, i] != board[j + 1, i]) {
                        colWinner = -1;
                    }
                }

                if (board[i, j] == 0) {
                    hasTied = -1;
                }
            }

            if (rowWinner > -1) {
                lineSpawnPos = new Vector3 (0, topOffset - (size * i) - (boxOffset * i), -1);
                lineDrawPos = new Vector3 (canvasW, 0, 0);

                return rowWinner;
            }

            if (colWinner > -1) {
                lineSpawnPos = new Vector3 (((size + boxOffset) * (i + 1)) - size / 2, topOffset - boxGridHeight + size / 2 + boxOffset, -1);
                lineDrawPos = new Vector3 (0, boxGridHeight - boxOffset, 0);

                return colWinner;
            }

            rowWinner = 0;
            colWinner = 0;
        }

        if (diagWinner > -1) {
            lineSpawnPos = new Vector3 (11, topOffset + size / 2, -1);
            lineDrawPos = new Vector3 (boxGridHeight - boxOffset, -(boxGridHeight - boxOffset), 0);

            return diagWinner;
        }

        if (antiDiagWinner > -1) {
            lineSpawnPos = new Vector3 (11, topOffset - boxGridHeight + size / 2 + boxOffset, -1);
            lineDrawPos = new Vector3 (boxGridHeight - boxOffset, boxGridHeight - boxOffset, 0);

            return antiDiagWinner;
        }

        if (hasTied > -1) {
            return 3;
        }

        return -1;
    }

    // Change player and check winner
    public void ChangePlayer () {
        if (P1) {
            P1 = false;
        } else {
            P1 = true;
        }

        if (boardLen >= grid * 2 - 1) {
            int result = HasMatched ();

            if (result != -1) {
                if (result == 3) {
                    GameOver (true);
                } else {
                    GameOver (false);
                }
            }
        }
    }

    // .. Game Over
    public void GameOver (bool hasTied) {
        // If is already over
        if (isGameOver) {
            return;
        }

        if (!hasTied) {
            // Draw lines
            destination = lineDrawPos;
            dist = Vector3.Distance (origin, destination);

            lineGen = Instantiate (line, lineSpawnPos, Quaternion.identity) as GameObject;
            lineGen.transform.SetParent (canvas.transform, false);
            lineRend = lineGen.GetComponent<LineRenderer> ();

            if (!P1) {
                lineRend.material = xMat;
                Debug.Log ("Player 1 won");
            } else {
                lineRend.material = oMat;
                Debug.Log ("Player 2 won");
            }
        } else {
            Debug.Log ("Tied");
        }
        isGameOver = true;
        animateLine = true;
    }
}