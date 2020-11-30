using System;
using System.Collections;
using System.Timers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TicTacToeCreatorAI : MonoBehaviour {
    #region Variables
    public GameObject line;
    public GameObject card;

    private Image _particle;
    private Button _menuButton;
    private Button _playAgainButton;

    public Sprite handshakeSprite;
    public Sprite loserSprite;
    public Sprite oImg;

    public Material oMat;

    public Image endImage;
    public Image downArrowPr;
    public Image player1Card;
    public Image player2Card;
    public Image cardBorder;
    public Image blueLineDotPr;

    public Button playAgainBtn;
    public Button menuBtn;
    public Image bgImg;
    public Text turnTxtPr;
    public Text endText;
    public Image particlePr;
    private Image _lineDot;
    private Image lineDotPr;

    private Image downArrow;
    private Image border;

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
    private Text turnTxt;

    // Float
    private float counter;
    private float dist;
    private float boxSize;
    private float cardW;
    private float cardH;
    private float particleCount;
    private float particleW;
    private float particleH;
    private float randParticleSize;
    private float canvasW;
    private float canvasH;
    private float gapX;
    private float drawSpeed;
    private float drawDelay;
    private float boxGridHeight;
    private float aiMoveDelay;
    private float buttonCounter;

    // Vector3 
    private Vector3 lineDot1Pos;
    private Vector3 lineDot2Pos;
    private Vector3 lineDot3Pos;

    private Vector3 lineSpawnPos;
    private Vector3 lineDrawPos1;
    private Vector3 lineDrawPos;
    private Vector3 destination;
    private Vector3 origin;

    // Boolean
    [HideInInspector]
    public bool isGameOver;
    [HideInInspector]
    public bool isAIMoving;
    private bool animateLine = false;
    private bool hasInitButton;
    #endregion

    #region Initially set variables board
    void Start () {
        board = new int[grid, grid];
        topOffset = -(Screen.height / 2) + -(-(Screen.height / 2) / 3);

        endImage.gameObject.SetActive (false);

        border = Instantiate (cardBorder, new Vector3 (0, 0, 0), Quaternion.identity);
        border.transform.SetParent (player1Card.transform, false);

        turnTxt = Instantiate (turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
        turnTxt.transform.SetParent (player1Card.transform, false);

        downArrow = Instantiate (downArrowPr, downArrowPr.transform.position, Quaternion.identity);
        downArrow.transform.SetParent (player1Card.transform, false);

        hasInitButton = false;
        player = 1;
        ai = 2;
        tie = 3;

        drawSpeed = 12f;
        drawDelay = 0.1f;
        aiMoveDelay = 1f;
        buttonCounter = 1.5f;

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
        // canvas.GetComponent<CanvasScaler> ().referenceResolution = new Vector2 (Screen.width, Screen.height);

        bgImg.GetComponent<RectTransform> ().sizeDelta = new Vector2 (canvasW, canvasH);

        // Booleans
        isGameOver = false;

        particleCount = 150;
        boxSize = 170;
        // boxSize = Screen.width / grid - boxOffset;
        particleW = particlePr.GetComponent<RectTransform> ().rect.width;
        particleH = particlePr.GetComponent<RectTransform> ().rect.height;

        // Initialize particles
        InitParticles ();
        // Initialize boxes
        IntiBoxGrid ();
    }
    #endregion

    #region Init grid
    private void IntiBoxGrid () {
        // Give size of boxes
        card.GetComponent<RectTransform> ().sizeDelta = new Vector2 (boxSize, boxSize);
        boxGridHeight = 50;

        for (int i = 0; i < grid; i++) {
            // Reset the gap on the x axis every time
            gapX = boxOffset * 2;
            // gapX = boxSize / 2 + boxOffset;

            for (int j = 0; j < grid; j++) {
                box = Instantiate (card, new Vector3 (gapX + 10, boxGridHeight, 0), Quaternion.identity) as GameObject;
                box.transform.SetParent (GameObject.Find ("Card Border").transform, false);
                box.name = colCount + "" + rowCount;

                boxScript = box.GetComponent<TagBoxAI> ();
                boxScript.boxColNum = colCount;
                boxScript.boxRowNum = rowCount;

                // Update gap on X axis after evey box
                gapX += boxSize + boxOffset * 2;
                rowCount++;
            }

            boxGridHeight += boxSize + boxOffset;
            colCount++;
            rowCount = 0;
        }
    }

    #endregion

    #region Init particles
    public void InitParticles () {
        for (var i = 0; i <= particleCount; i++) {
            _particle = Instantiate (particlePr, new Vector3 (UnityEngine.Random.Range (particleW, canvasW), -(UnityEngine.Random.Range (particleH, canvasH)), 0), Quaternion.identity);
            _particle.transform.SetParent (GameObject.Find ("Particle Spawner").transform, false);

            // Randomize Opacity
            Image image = _particle.GetComponent<Image> ();
            image.color = new Color (image.color.r, image.color.g, image.color.b, UnityEngine.Random.Range (0.2f, 0.6f));

            // Randomize particle sizes
            randParticleSize = UnityEngine.Random.Range (particleW, particleW + 8);
            _particle.GetComponent<RectTransform> ().sizeDelta = new Vector2 (randParticleSize, randParticleSize);
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

        
        if (!hasInitButton && isGameOver) {
            if (buttonCounter > 0) {
                buttonCounter -= Time.deltaTime;
            } else {
                border.gameObject.SetActive (false);
                turnTxt.gameObject.SetActive (false);
                downArrow.gameObject.SetActive (false);

                if (!isAIMoving) {
                    turnTxt = Instantiate (turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                    turnTxt.transform.SetParent (player2Card.transform, false);
                    turnTxt.text = "Winner";

                    turnTxt = Instantiate (turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                    turnTxt.transform.SetParent (player1Card.transform, false);
                    turnTxt.text = "Loser";

                    endImage.GetComponent<Image> ().sprite = loserSprite;
                    endText.text = "You Lose";
                    lineGen.gameObject.SetActive (false);
                } else {
                    turnTxt = Instantiate (turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                    turnTxt.transform.SetParent (player2Card.transform, false);
                    turnTxt.text = "Draw";

                    turnTxt = Instantiate (turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                    turnTxt.transform.SetParent (player1Card.transform, false);
                    turnTxt.text = "Draw";

                    endImage.GetComponent<Image> ().sprite = handshakeSprite;
                    endText.text = "Draw";
                }

                GameObject.Find ("Card Border").SetActive (false);
                GameObject.Find ("Line Dots").SetActive (false);
                endImage.gameObject.SetActive (true);
                hasInitButton = true;
            }
        }
    }

    // Winner Checker
    private int HasMatched () {
        int n = grid;
        int rowWinner = 0;
        int colWinner = 0;
        int diagWinner = 0;
        int antiDiagWinner = 0;
        int hasTied = 3;

        for (int i = 0; i < n; i++) {
            if (i < n - 1) {
                // Detect Diagnal and Anti Diagnal
                if (diagWinner > -1) {
                    diagWinner = board[i, (n - 1) - i];
                    if (board[i, (n - 1) - i] == 0 || board[i, (n - 1) - i] != board[i + 1, (n - 1) - i - 1]) {
                        diagWinner = -1;
                    }
                }

                if (antiDiagWinner > -1) {
                    antiDiagWinner = board[i, i];
                    if (board[i, i] == 0 || board[i, i] != board[i + 1, i + 1]) {
                        antiDiagWinner = -1;
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
                    hasTied = -1;
                }
            }

            if (rowWinner > -1) {
                lineSpawnPos = new Vector3 (170, 192 + boxSize * (1 + i) + boxOffset * (1 + i) - boxSize / 2 - 5, -1);
                // lineSpawnPos = new Vector3 ((boxSize / 2 + boxOffset + 3) + boxSize / 2, (cardBorderRect.anchoredPosition.y - cardBorderRect.rect.width / 2 + 42) + (boxSize * (1 + 2)) + boxOffset * (2) - boxSize / 2, -1);
                // lineSpawnPos = new Vector3 ((boxSize / 2 + boxOffset + 3) + boxSize / 2, boxSize * (0 + 1) + boxOffset * (0 + 1) + 3, -1);
                lineDrawPos = new Vector3 (canvasW - ((boxSize * 2) + 3 + boxOffset - 10), 0, 0);

                destination = lineDrawPos;
                dist = Vector3.Distance (origin, destination);

                // 1st line dot
                lineDot1Pos = lineSpawnPos;

                // 2nd line dot
                lineDot2Pos = new Vector3 ((boxSize / 2 + boxOffset + 3) + boxSize * 2 + boxOffset - boxSize / 2, lineSpawnPos.y, lineSpawnPos.z);

                // 3rd line dot
                lineDot3Pos = new Vector3 (552, lineSpawnPos.y, lineSpawnPos.z);

                return rowWinner;
            }

            if (colWinner > -1) {
                lineSpawnPos = new Vector3 ((80 + boxSize * (1 + i) + boxOffset * 2 * (1 + i)) - boxSize / 2 - boxOffset - 5, 194 + boxSize / 2, -1);
                lineDrawPos = new Vector3 (0, boxSize * (grid - 1) + (boxOffset * grid), 0);

                destination = lineDrawPos;
                dist = Vector3.Distance (origin, destination);

                // 1st line dot
                lineDot1Pos = new Vector3 (lineSpawnPos.x, lineSpawnPos.y + 5, lineSpawnPos.z);

                // 2nd line dot
                lineDot2Pos = new Vector3 (lineSpawnPos.x, 461, lineSpawnPos.z);

                // 3rd line dot
                lineDot3Pos = new Vector3 (lineSpawnPos.x, lineSpawnPos.y + dist - 5, lineSpawnPos.z);

                return colWinner;
            }

            rowWinner = 0;
            colWinner = 0;
        }

        if (diagWinner > -1) {
            lineSpawnPos = new Vector3 (173, 640, -1);
            lineDrawPos = new Vector3 (375, -360, 0);
            // lineDrawPos = new Vector3 (boxSize * (grid - 1) + boxOffset * grid - boxOffset, -(boxSize * (grid - 1) + boxOffset * grid - boxOffset), 0);

            destination = lineDrawPos;
            dist = Vector3.Distance (origin, destination);

            // 1st line dot
            lineDot1Pos = lineSpawnPos;

            // 2nd line dot
            lineDot2Pos = new Vector3 (360, 460, lineSpawnPos.z);
            // lineDot2Pos = new Vector3 (boxSize + boxSize / 2 + boxOffset, -lineDrawPos.y, lineSpawnPos.z);

            // 3rd line dot
            lineDot3Pos = new Vector3 (549, 281, lineSpawnPos.z);

            return diagWinner;
        }

        if (antiDiagWinner > -1) {
            lineSpawnPos = new Vector3 (553, 640, -1);
            lineDrawPos = new Vector3 (-380, -360, 0);

            destination = lineDrawPos;
            dist = Vector3.Distance (origin, destination);

            // 1st line dot
            lineDot1Pos = lineSpawnPos;

            // 2nd line dot
            lineDot2Pos = new Vector3 (360, 459, lineSpawnPos.z);

            // 3rd line dot
            lineDot3Pos = new Vector3 (173, 281, lineSpawnPos.z);

            return antiDiagWinner;
        }

        if (hasTied > -1) {
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
        border.transform.SetParent (player2Card.transform, false);
        turnTxt.transform.SetParent (player2Card.transform, false);
        turnTxt.text = "Bot's Turn";
        downArrow.transform.SetParent (player2Card.transform, false);

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

            boxScript.image.sprite = oImg;
            boxScript.clicked = true;
            board[bestMove[0], bestMove[1]] = ai;

            boardLen++;
            isAIMoving = false;
            HasEnded ();

            border.transform.SetParent (player1Card.transform, false);
            turnTxt.transform.SetParent (player1Card.transform, false);
            turnTxt.text = "Your turn";
            downArrow.transform.SetParent (player1Card.transform, false);
        }
    }

    // Change player and check winner
    public void HasEnded () {
        int result = HasMatched ();

        if (result != -1) {
            if (result == 3) {
                GameOver (true);
            } else {
                if (result == 2) {
                    GameOver (false);
                }
            }
        }
    }

    public void GoToMenu () {
        SceneManager.LoadScene ("Menu");
    }

    // Game Over
    public void GameOver (bool hasTied) {
        // If is already over
        if (isGameOver) {
            return;
        }

        if (!hasTied) {
            // Draw lines
            lineGen = Instantiate (line, lineSpawnPos, Quaternion.identity) as GameObject;
            lineGen.transform.SetParent (canvas.transform, false);
            lineRend = lineGen.GetComponent<LineRenderer> ();

            lineRend.material = oMat;
            lineDotPr = blueLineDotPr;

            // 1st line dot
            _lineDot = Instantiate (lineDotPr, lineDot1Pos, Quaternion.identity);
            _lineDot.transform.SetParent (GameObject.Find ("Line Dots").transform, false);

            // 2nd line dot
            _lineDot = Instantiate (lineDotPr, lineDot2Pos, Quaternion.identity);
            _lineDot.transform.SetParent (GameObject.Find ("Line Dots").transform, false);

            // 3rd line dot
            _lineDot = Instantiate (lineDotPr, lineDot3Pos, Quaternion.identity);
            _lineDot.transform.SetParent (GameObject.Find ("Line Dots").transform, false);

            animateLine = true;
        } else { }

        isGameOver = true;
    }
}