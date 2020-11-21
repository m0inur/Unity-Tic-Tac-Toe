using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TicTacToeCreatorPvP : MonoBehaviour {
    #region Variables
    public GameObject confettiWindow;
    public GameObject line;
    public GameObject card;

    public Material xMat;
    public Material oMat;

    public Transform xConfetti;
    public Transform oConfetti;

    public Sprite winnerSprite;
    public Sprite handshakeSprite;

    public Image endImage;
    public Button backButton;
    public Image downArrowPr;
    public Image player1Card;
    public Image player2Card;
    public Image darkOverlay;
    public Image cardBorder;
    public Image particlePr;
    public Image blueLineDotPr;
    public Image redLineDotPr;

    public Button menuBtn;
    public Button playAgainBtn;

    public Text turnTxtPr;
    public Text endTxt;

    private GameObject lineGen;
    private LineRenderer lineRend;
    private Canvas canvas;
    private Image downArrow;
    private Image particle;
    private Image border;
    private Text turnTxt;
    private Image lineDot;
    private Image lineDotPr;

    private Button menuButton;
    private Button playAgainButton;

    public int[, ] board;
    public int grid;
    public int boardLen;

    private int hasTied = 3;

    public bool isGameOver;
    public bool P1;

    private RectTransform rt;
    private TagBoxPvP boxScript;
    private GameObject box;

    private Vector3 lineDot1Pos;
    private Vector3 lineDot2Pos;
    private Vector3 lineDot3Pos;

    private Vector3 lineSpawnPos;
    private Vector3 lineDrawPos1;
    private Vector3 lineDrawPos;
    private Vector3 destination;
    private Vector3 origin;

    private float showButtonsTimer;
    private float counter;
    private float dist;
    private float boxSize;
    private float cardW;
    private float cardH;
    private float particleCount;
    private float particleW;
    private float particleH;
    private float randparticleSize;
    private float canvasW;
    private float canvasH;
    private float gapX;
    private float drawSpeed;
    private float drawDelay;
    private float boxGridHeight;
    private float boxOffset;

    private int rowCount;
    private int colCount;

    private int n;
    private bool hasDrawn = false;
    private bool spawnButtons = false;
    private bool animateLine = false;
    #endregion

    #region Initially set variables board
    void Start () {
        board = new int[grid, grid];

        endImage.gameObject.SetActive (false);

        border = Instantiate (cardBorder, new Vector3 (0, 0, 0), Quaternion.identity);
        border.transform.SetParent (player1Card.transform, false);

        turnTxt = Instantiate (turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
        turnTxt.transform.SetParent (player1Card.transform, false);

        downArrow = Instantiate (downArrowPr, downArrowPr.transform.position, Quaternion.identity);
        downArrow.transform.SetParent (player1Card.transform, false);

        // Integers
        rowCount = 0;
        colCount = 0;
        boardLen = 0;
        boxOffset = 10;

        drawSpeed = 12;
        drawDelay = 0.1f;
        boxGridHeight = 50;
        showButtonsTimer = 1.5f;

        n = grid;

        P1 = true;

        lineDrawPos1 = new Vector3 (0, 0, 0);
        lineDrawPos1 = new Vector3 (0, 0, 0);
        lineDrawPos1 = new Vector3 (0, 0, 0);

        lineDrawPos1 = new Vector3 (0, 0, 0);
        origin = new Vector3 (0, 0, 0);

        // Canvas & UI
        canvas = FindObjectOfType<Canvas> ();
        canvasW = canvas.GetComponent<RectTransform> ().rect.width;
        canvasH = canvas.GetComponent<RectTransform> ().rect.height;
        // canvas.GetComponent<CanvasScaler> ().referenceResolution = new Vector2 (Screen.width, Screen.height);

        particleW = particlePr.GetComponent<RectTransform> ().rect.width;
        particleH = particlePr.GetComponent<RectTransform> ().rect.height;

        // Booleans
        isGameOver = false;

        // Calculate sizes
        // boxSize = (Screen.width / grid) - boxOffset - (boxOffset / 3);
        // boxSize = (canvasW / grid) - boxOffset * 4;
        boxSize = 205;
        boxSize = 170;

        particleCount = 150;
        // Debug.Log ((canvasW / grid) - (boxOffset * 3) - (boxOffset * 2) * 2);

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
        // boxGridHeight = 50;

        for (int i = 0; i < grid; i++) {
            // Reset the gap on the x axis every time
            gapX = boxOffset * 2;
            // gapX = boxSize / 2 + boxOffset;

            for (int j = 0; j < grid; j++) {
                box = Instantiate (card, new Vector3 (gapX + 10, boxGridHeight, 0), Quaternion.identity) as GameObject;
                box.transform.SetParent (GameObject.Find ("Card Border").transform, false);

                boxScript = box.GetComponent<TagBoxPvP> ();
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
            particle = Instantiate (particlePr, new Vector3 (UnityEngine.Random.Range (particleW, canvasW), -(UnityEngine.Random.Range (particleH, canvasH)), 0), Quaternion.identity);
            particle.transform.SetParent (GameObject.Find ("Particle Spawner").transform, false);

            // Randomize Opacity
            Image image = particle.GetComponent<Image> ();
            image.color = new Color (image.color.r, image.color.g, image.color.b, UnityEngine.Random.Range (0.2f, 0.6f));

            // Randomize particle sizes
            randparticleSize = UnityEngine.Random.Range (particleW, particleW + 8);
            particle.GetComponent<RectTransform> ().sizeDelta = new Vector2 (randparticleSize, randparticleSize);
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

                lineRend.SetPosition (0, ALine);
            } else {
                animateLine = false;
            }
        }

        if (isGameOver && !spawnButtons) {
            if (showButtonsTimer > 0) {
                showButtonsTimer -= Time.deltaTime;
            } else {
                // Disable
                GameObject.Find ("Card Border").SetActive (false);
                GameObject.Find ("Line Dots").SetActive (false);

                border.gameObject.SetActive (false);
                turnTxt.gameObject.SetActive (false);
                downArrow.gameObject.SetActive (false);

                // If it didnt draw check who won
                if (!hasDrawn) {
                    lineGen.gameObject.SetActive (false);
                    if (!P1) {
                        turnTxt = Instantiate (turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                        turnTxt.transform.SetParent (player1Card.transform, false);
                        turnTxt.text = "Winner";

                        turnTxt = Instantiate (turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                        turnTxt.transform.SetParent (player2Card.transform, false);
                        turnTxt.text = "Loser";
                    } else {
                        turnTxt = Instantiate (turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                        turnTxt.transform.SetParent (player2Card.transform, false);
                        turnTxt.text = "Winner";

                        turnTxt = Instantiate (turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                        turnTxt.transform.SetParent (player1Card.transform, false);
                        turnTxt.text = "Loser";
                    }

                    endImage.GetComponent<Image> ().sprite = winnerSprite;
                    endTxt.text = "You Won";
                } else {
                    turnTxt = Instantiate (turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                    turnTxt.transform.SetParent (player2Card.transform, false);
                    turnTxt.text = "Draw";

                    turnTxt = Instantiate (turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                    turnTxt.transform.SetParent (player1Card.transform, false);

                    endImage.GetComponent<Image> ().sprite = handshakeSprite;
                    endTxt.text = "Draw";
                }

                endImage.gameObject.SetActive (true);
                spawnButtons = true;
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

    // Change player and check winner
    public void ChangePlayer () {
        if (P1) {
            P1 = false;
            border.transform.SetParent (player2Card.transform, false);
            turnTxt.transform.SetParent (player2Card.transform, false);
            turnTxt.text = "Player 2's Turn";
            downArrow.transform.SetParent (player2Card.transform, false);
        } else {
            P1 = true;
            border.transform.SetParent (player1Card.transform, false);
            turnTxt.transform.SetParent (player1Card.transform, false);
            turnTxt.text = "Your Turn";
            downArrow.transform.SetParent (player1Card.transform, false);
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

    public void GoToMenu () {
        SceneManager.LoadScene ("Menu");
    }

    public void PlayAgain () {
        SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
    }

    public void GameOver (bool hasTied) {
        // If is already over
        if (isGameOver) {
            return;
        }
        // Instantiate Dark Overlay
        // Image darkOverlayObj = Instantiate (darkOverlay, new Vector3 (0, 0, 0), Quaternion.identity);
        // darkOverlayObj.transform.SetParent (canvas.transform, false);

        if (!hasTied) {
            // Draw lines
            lineGen = Instantiate (line, lineSpawnPos, Quaternion.identity) as GameObject;
            lineGen.transform.SetParent (GameObject.Find ("Line Wrapper").transform, false);
            lineRend = lineGen.GetComponent<LineRenderer> ();

            // Confetti
            GameObject confettiWindowObj = Instantiate (confettiWindow, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
            confettiWindowObj.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width, Screen.height);
            confettiWindowObj.transform.SetParent (canvas.transform, false);
            WindowConfetti windowScript = confettiWindowObj.GetComponent<WindowConfetti> ();

            if (!P1) {
                endTxt.text = "Winner";

                lineRend.material = xMat;
                windowScript.pfConfetti = xConfetti;
                lineDotPr = redLineDotPr;
            } else {
                endTxt.text = "Winner";

                lineRend.material = oMat;
                windowScript.pfConfetti = oConfetti;
                lineDotPr = blueLineDotPr;
            }

            // 1st line dot
            lineDot = Instantiate (lineDotPr, lineDot1Pos, Quaternion.identity);
            lineDot.transform.SetParent (GameObject.Find ("Line Dots").transform, false);

            // 2nd line dot
            lineDot = Instantiate (lineDotPr, lineDot2Pos, Quaternion.identity);
            lineDot.transform.SetParent (GameObject.Find ("Line Dots").transform, false);

            // 3rd line dot
            lineDot = Instantiate (lineDotPr, lineDot3Pos, Quaternion.identity);
            lineDot.transform.SetParent (GameObject.Find ("Line Dots").transform, false);
        } else {
            hasDrawn = true;
            endTxt.text = "It's a Draw!";
        }

        // Disable back button
        isGameOver = true;
        animateLine = true;
    }
}