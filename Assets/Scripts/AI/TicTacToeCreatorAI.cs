using System;
using System.Collections;
using System.Timers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TicTacToeCreatorAI : MonoBehaviour {
    #region Variables
    public GameObject confettiWindow;
    public GameObject line;
    public GameObject card;

    private Image star;
    private Button menuButton;
    private Button playAgainButton;

    public Sprite OImg;
    public Material oMat;
    public Transform oConfetti;

    public Button playAgainBtn;
    public Button menuBtn;
    public Image bgImg;
    public Text winnerTxt;
    public Image starPr;

    private GameObject lineGen;
    private LineRenderer lineRend;

    public float topOffset;
    public float boxOffset;

    public int grid;

    // Integer
    public int[, ] board;
    [HideInInspector]
    public int boardLen;

    private int player;
    private int ai;
    private int tie;
    private int rowCount;
    private int colCount;

    private int n;

    // Game Objects
    private RectTransform rt;
    private TagBoxAI boxScript;
    private GameObject box;
    private Canvas canvas;

    // Float
    private float counter;
    private float dist;
    private float size;
    private float cardW;
    private float cardH;
    private float starCount;
    private float starW;
    private float starH;
    private float randStarSize;
    private float canvasW;
    private float canvasH;
    private float gapX;
    private float drawSpeed;
    private float drawDelay;
    private float boxGridHeight;
    private float aiMoveDelay;

    // Vector3 
    private Vector3 lineSpawnPos;
    private Vector3 lineDrawPos1;
    private Vector3 lineDrawPos;
    private Vector3 destination;
    private Vector3 origin;

    // Boolean
    [HideInInspector]
    public bool isGameOver;
    [HideInInspector]
    public bool P1;
    private bool animateLine = false;

    #endregion

    #region Initially set variables board
    void Start () {
        board = new int[grid, grid];
        topOffset = -(Screen.height / 2) + -(-(Screen.height / 2) / 3);

        player = 1;
        ai = 2;
        tie = 3;

        drawSpeed = 12f;
        drawDelay = 0.1f;
        aiMoveDelay = 0.4f;

        rowCount = 0;
        colCount = 0;
        boardLen = 0;
        n = grid;

        lineDrawPos1 = new Vector3 (0, 0, 0);
        origin = new Vector3 (0, 0, 0);

        winnerTxt.text = "";

        // Canvas
        canvas = FindObjectOfType<Canvas> ();
        canvasW = canvas.GetComponent<RectTransform> ().rect.width;
        canvasH = canvas.GetComponent<RectTransform> ().rect.height;
        canvas.GetComponent<CanvasScaler> ().referenceResolution = new Vector2 (Screen.width, Screen.height);

        bgImg.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width, Screen.height);

        // Booleans
        isGameOver = false;
        P1 = true;

        // Calculate sizes
        size = Screen.width / grid - boxOffset;
        starW = starPr.GetComponent<RectTransform> ().rect.width;
        starH = starPr.GetComponent<RectTransform> ().rect.height;

        starCount = (Screen.width + Screen.height) / (starH + starW);

        // Initialize stars
        InitStars ();
        // Initialize boxes
        InitBoxGrid ();
    }
    #endregion

    #region Init grid
    private void InitBoxGrid () {
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

                boxScript = box.GetComponent<TagBoxAI> ();
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

    #region Init Stars
    public void InitStars () {
        for (var i = 0; i <= starCount; i++) {
            star = Instantiate (starPr, new Vector3 (UnityEngine.Random.Range (starW, Screen.width), -(UnityEngine.Random.Range (starH, Screen.height)), 0), Quaternion.identity);
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
        int tie = 3;

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

            for (int j = 0; j < n; j++) {
                if (j < n - 1) {
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
                }

                if (board[i, j] == 0) {
                    tie = -1;
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

        if (tie > -1) {
            return 3;
        }

        return -1;
    }

    private double Minimax (int[, ] board, int depth, bool isMax) {
        int result = HasMatched ();

        // -1 = No one won nor did it tie
        if (result != -1) {
            // X = O = 0, tie = 3
            if (result == 3) {
                return 0;
            } else if (result == 1) {
                return 10 - depth;
            } else if (result == 2) {
                return -10 + depth;
            }
        }

        if (isMax) {
            var bestScore = Double.NegativeInfinity;

            for (int i = 0; i < grid; i++) {
                for (int j = 0; j < grid; j++) {
                    if (board[i, j] == 0) {
                        board[i, j] = player;
                        var score = Minimax (board, depth + 1, false);
                        board[i, j] = 0;

                        if (score > bestScore) {
                            bestScore = score;
                        }
                    }
                }
            }

            return bestScore;
        } else {
            var bestScore = Double.PositiveInfinity;

            for (int i = 0; i < grid; i++) {
                for (int j = 0; j < grid; j++) {
                    if (board[i, j] == 0) {
                        board[i, j] = ai;
                        var score = Minimax (board, depth + 1, true);
                        board[i, j] = 0;

                        if (score < bestScore) {
                            bestScore = score;
                        }
                    }
                }
            }

            return bestScore;
        }
    }

    // Make ai move
    public IEnumerator MoveAi () {
        double bestScore = Double.PositiveInfinity;
        int[] bestMove = new int[2];

        for (int i = 0; i < grid; i++) {
            for (int j = 0; j < grid; j++) {
                if (board[i, j] == 0) {
                    board[i, j] = ai;
                    double score = Minimax (board, 0, true);
                    board[i, j] = 0;

                    if (bestScore > score) {
                        bestScore = score;
                        bestMove[0] = i;
                        bestMove[1] = j;
                    }
                }
            }
        }

        // If the spot is the first value and it is not empty
        if (bestMove[0] == 0 && bestMove[1] == 0 && board[0, 0] != 0) {

        } else {
            // Get game object and script
            GameObject box = GameObject.Find (bestMove[0] + "" + bestMove[1]);
            TagBoxAI boxScript = box.GetComponent<TagBoxAI> ();

            yield return new WaitForSeconds (aiMoveDelay);

            boxScript.image.sprite = OImg;
            boxScript.clicked = true;
            board[bestMove[0], bestMove[1]] = ai;

            boardLen++;
            ChangePlayer ();
        }
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
                    if (result == 2) { }

                    GameOver (false);
                }
            }
        }
    }

    public void GoToMenu () {
        SceneManager.LoadScene ("Menu");
    }

    // .. Game Over
    public void GameOver (bool hasTied) {
        // If is already over
        if (isGameOver) {
            return;
        }

        if (!hasTied) {
            // Measure line properties
            destination = lineDrawPos;
            dist = Vector3.Distance (origin, destination);

            // Draw lines
            lineGen = Instantiate (line, lineSpawnPos, Quaternion.identity) as GameObject;
            lineGen.transform.SetParent (canvas.transform, false);
            lineRend = lineGen.GetComponent<LineRenderer> ();

            // Confetti
            GameObject confettiWindowObj = Instantiate (confettiWindow, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;

            confettiWindowObj.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width, Screen.height);
            confettiWindowObj.transform.SetParent (canvas.transform, false);

            WindowConfetti windowScript = confettiWindowObj.GetComponent<WindowConfetti> ();
            windowScript.pfConfetti = oConfetti;

            winnerTxt.text = "Winner: O";
            lineRend.material = oMat;
        } else {
            winnerTxt.text = "Tie";
        }

        menuButton = Instantiate (menuBtn, new Vector3 (0, 0, -2), Quaternion.identity);
        menuButton.transform.SetParent (canvas.transform, false);

        playAgainButton = Instantiate (playAgainBtn, new Vector3 (0, menuButton.GetComponent<RectTransform> ().rect.height + 10, -2), Quaternion.identity);
        playAgainButton.transform.SetParent (canvas.transform, false);

        isGameOver = true;
        animateLine = true;
    }
}