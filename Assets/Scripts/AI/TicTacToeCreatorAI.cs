﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AI
{
    public class TicTacToeCreatorAI : MonoBehaviour {
        #region Variables

        public GameObject line;
        public GameObject card;

        private Button _menuButton;
        private Button _playAgainButton;

        public Sprite handshakeSprite;
        public Sprite loserSprite;
        public Sprite oImg;

        public Material oMat;

        public Image cardBorder;
        public Image endImage;
        public Image downArrowPr;
        public Image player1Card;
        public Image player2Card;
        public Image playerCardBorder;
        public Image blueLineDotPr;

        public Text turnTxtPr;
        public Text endText;
        
        private Image _lineDot;
        private Image _lineDotPr;

        private Image _downArrow;
        private Image _border;

        private GameObject _lineGen;
        private LineRenderer _lineRend;
        
        public int grid;

        // Integer
        public int[, ] Board;
        [HideInInspector]
        public int boardLen;

        private int _player;
        private int _ai;
        private int _rowCount;
        private int _colCount;

        // Game Objects
        private RectTransform _rt;
        private TagBoxAI _boxScript;
        private GameObject _box;
        private Canvas _canvas;
        private Text _turnTxt;
        private Text _winnerTxt;
        private Text _loserTxt;
        private Image _particle;

        // Float
        private float _counter;
        private float _dist;
        private float _boxSize;
        private float _cardW;
        private float _cardH;
        private float _randParticleSize;
        private float _canvasW;
        private float _canvasH;
        private float _gapX;
        private float _drawSpeed;
        private float _drawDelay;
        private float _boxGridHeight;
        private float _aiMoveDelay;
        private float _buttonCounter;
        private float _boxOffset;
        private float _cardBorderTopGap;
        private float _linedotSize;
        
        // Vector3 
        private Vector3 _lineDot1Pos;
        private Vector3 _lineDot2Pos;
        private Vector3 _lineDot3Pos;

        private Vector3 _lineSpawnPos;
        private Vector3 _lineDrawPos;
        private Vector3 _destination;
        private Vector3 _origin;

        // Boolean
        [HideInInspector]
        public bool isGameOver;
        [HideInInspector]
        public bool isAIMoving;
        private bool _animateLine;
        private bool _hasInitButton;
        private bool _hasLost;
        
        #endregion

        #region Setup Variable
        // Create the board
        private void Start()
        {
            Board = new int[grid, grid];

            endImage.gameObject.SetActive (false);

            _border = Instantiate (playerCardBorder, new Vector3 (0, 0, 0), Quaternion.identity);
            _border.transform.SetParent (player1Card.transform, false);

            _turnTxt = Instantiate (turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
            _turnTxt.transform.SetParent (player1Card.transform, false);

            _downArrow = Instantiate (downArrowPr, downArrowPr.transform.position, Quaternion.identity);
            _downArrow.transform.SetParent (player1Card.transform, false);
            Debug.Log("Instantiating Border, turn text and down arrow");

            _player = 1;
            _ai = 2;

            _drawSpeed = 12f;
            _drawDelay = 0.1f;
            _aiMoveDelay = 1f;
            _buttonCounter = 1.5f;
            _linedotSize = 15;

            _rowCount = 0;
            _colCount = 0;
            boardLen = 0;
            _boxGridHeight = 50;
            _cardBorderTopGap = _boxGridHeight;

            _origin = new Vector3 (0, 0, 0);

            // Canvas
            _canvas = FindObjectOfType<Canvas> ();
            _canvasW = _canvas.GetComponent<RectTransform> ().rect.width;
            _canvasH = _canvas.GetComponent<RectTransform> ().rect.height;

            // Booleans
            _hasInitButton = false;
            _animateLine = false;
            isGameOver = false;

            _boxSize = 170;
            _boxOffset = 10;

            // Initialize board
            InitBoxGrid();
        }

        private void OnDisable()
        {
            Board = new int[grid, grid];

            endImage.gameObject.SetActive (false);
            if (!_hasLost)
            {
                Destroy(_border.gameObject);
                Destroy(_turnTxt.gameObject);
                Destroy(_downArrow.gameObject);
            }

            _border = Instantiate (playerCardBorder, new Vector3 (0, 0, 0), Quaternion.identity);
            _border.transform.SetParent (player1Card.transform, false);
            
            _turnTxt = Instantiate (turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
            _turnTxt.transform.SetParent (player1Card.transform, false);
            
            _downArrow = Instantiate (downArrowPr, downArrowPr.transform.position, Quaternion.identity);
            _downArrow.transform.SetParent (player1Card.transform, false);
            
            _drawSpeed = 12f;
            _drawDelay = 0.1f;
            _aiMoveDelay = 1f;
            _buttonCounter = 1.5f;
            _linedotSize = 15;

            _counter = 0;
            _rowCount = 0;
            _colCount = 0;
            _boxGridHeight = 50;
            _cardBorderTopGap = _boxGridHeight;

            _origin = new Vector3 (0, 0, 0);

            // Booleans
            isGameOver = false;
            _hasInitButton = false;
            isAIMoving = false;
            _animateLine = false;
            _hasLost = false;
            _hasLost = false;

            if (boardLen > 0)
            {
                // Destroy previous board if it's been used  
                foreach (Transform child in cardBorder.transform) {
                    GameObject.Destroy(child.gameObject);
                }
                InitBoxGrid();
            }
            boardLen = 0;

            // Destroy arrow after being disabled
            cardBorder.gameObject.SetActive(true);
            if (_winnerTxt != null)
            {
                Destroy(_winnerTxt);
                Destroy(_loserTxt);
            }
        }

        #endregion
        
        #region Init grid

        private void InitBoxGrid()
        {
            // Give size of boxes
            card.GetComponent<RectTransform>().sizeDelta = new Vector2(_boxSize, _boxSize);

            for (var i = 0; i < grid; i++)
            {
                // Reset the gap on the x axis every time
                _gapX = _boxOffset * 2;

                for (var j = 0; j < grid; j++)
                {
                    _box = Instantiate(card, new Vector3(_gapX + 10, _boxGridHeight, 0),
                        Quaternion.identity) as GameObject;
                    _box.transform.SetParent(cardBorder.transform, false);
                    _box.name = _colCount + "" + _rowCount;
                    TagBoxAI _boxScript = _box.GetComponent<TagBoxAI>();
                    
                    _boxScript.boxColNum = _colCount;
                    _boxScript.boxRowNum = _rowCount;

                    // Update gap on X axis after evey box
                    _gapX += _boxSize + _boxOffset * 2;
                    _rowCount++;
                }

                _boxGridHeight += _boxSize + _boxOffset;
                _colCount++;
                _rowCount = 0;
            }
        }

        #endregion

        // Animate line
        private void Update () {
            if (_animateLine) {
                if (_counter < _dist) {
                    _counter += _drawDelay / _drawSpeed;
                    var x = Mathf.Lerp (0, _dist, _counter);

                    var a = _origin;
                    var b = _destination;

                    var aLine = x * Vector3.Normalize (b - a) + a;

                    _lineRend.SetPosition (1, aLine);
                }
            }

        
            if (!_hasInitButton && isGameOver) {
                if (_buttonCounter > 0) {
                    _buttonCounter -= Time.deltaTime;
                } else {
                    Destroy(_border.gameObject);
                    Destroy(_turnTxt.gameObject);
                    Destroy(_downArrow.gameObject);
                    
                    if (!isAIMoving) {
                        _winnerTxt = Instantiate (turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                        _winnerTxt.transform.SetParent (player2Card.transform, false);
                        _winnerTxt.text = "Winner";

                        _loserTxt = Instantiate (turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                        _loserTxt.transform.SetParent (player1Card.transform, false);
                        _loserTxt.text = "Loser";

                        endImage.GetComponent<Image> ().sprite = loserSprite;
                        endText.text = "You Lose";
                        _lineGen.gameObject.SetActive (false);
                    } else {
                        _turnTxt = Instantiate (turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                        _turnTxt.transform.SetParent (player2Card.transform, false);
                        _turnTxt.text = "Draw";

                        _turnTxt = Instantiate (turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                        _turnTxt.transform.SetParent (player1Card.transform, false);
                        _turnTxt.text = "Draw";

                        endImage.GetComponent<Image> ().sprite = handshakeSprite;
                        endText.text = "Draw";
                    }

                    cardBorder.gameObject.SetActive (false);
                    endImage.gameObject.SetActive (true);
                    _hasInitButton = true;
                }
            }
        }
        
          // Winner Checker
        private int HasMatched()
        {
            int n = grid;
            int rowWinner = 0;
            int colWinner = 0;
            int diagWinner = 0;
            int antiDiagWinner = 0;
            int hasTied = 3;

            for (int i = 0; i < n; i++)
            {
                if (i < n - 1)
                {
                    // Detect Diagnal and Anti Diagnal
                    if (diagWinner > -1)
                    {
                        diagWinner = Board[i, (n - 1) - i];
                        if (Board[i, (n - 1) - i] == 0 || Board[i, (n - 1) - i] != Board[i + 1, (n - 1) - i - 1])
                        {
                            diagWinner = -1;
                        }
                    }

                    if (antiDiagWinner > -1)
                    {
                        antiDiagWinner = Board[i, i];
                        if (Board[i, i] == 0 || Board[i, i] != Board[i + 1, i + 1])
                        {
                            antiDiagWinner = -1;
                        }
                    }
                }

                for (int j = 0; j < n; j++)
                {
                    if (j < n - 1)
                    {
                        if (rowWinner > -1)
                        {
                            rowWinner = Board[i, j];
                            // If the row has a gap or doesnt match value then this row cant match
                            if (Board[i, j] == 0 || Board[i, j] != Board[i, j + 1])
                            {
                                rowWinner = -1;
                            }
                        }

                        if (colWinner > -1)
                        {
                            colWinner = Board[j, i];
                            if (Board[j, i] == 0 || Board[j, i] != Board[j + 1, i])
                            {
                                colWinner = -1;
                            }
                        }
                    }

                    if (Board[i, j] == 0)
                    {
                        hasTied = -1;
                    }
                }

                if (rowWinner > -1)
                {
                    _lineSpawnPos = new Vector3((_boxOffset * 2) + 5 + (_boxSize - (_boxSize / 2)),
                        _cardBorderTopGap + (_boxSize * (1 + i) + (_boxOffset * i)) - (_boxSize / 2), -1);
                    _lineDrawPos = new Vector3((_boxSize * (grid - 1)) + (_boxOffset * 2) * (grid - 1) + 5, 0, 0);

                    _destination = _lineDrawPos;
                    _dist = Vector3.Distance(_origin, _destination);

                    // 1st line dot
                    _lineDot1Pos = new Vector3(_lineSpawnPos.x + _linedotSize / 2, _lineSpawnPos.y, _lineSpawnPos.z);

                    // 2nd line dot
                    _lineDot2Pos = new Vector3(_lineSpawnPos.x + _boxSize + (_boxOffset * 2) + _linedotSize / 2,
                        _lineSpawnPos.y, _lineSpawnPos.z);

                    // 3rd line dot
                    _lineDot3Pos = new Vector3(_lineSpawnPos.x + _lineDrawPos.x, _lineSpawnPos.y, _lineSpawnPos.z);

                    return rowWinner;
                }

                if (colWinner > -1)
                {
                    // Draw lines
                    _lineSpawnPos = new Vector3(10 + (_boxSize * (1 + i)) - (_boxSize / 2) + (_boxOffset * 2) * (1 + i),
                        _cardBorderTopGap + _boxSize - (_boxSize / 2) - 5, -1);
                    _lineDrawPos = new Vector3(0, (_boxSize * (grid - 1)) + _boxOffset * (grid - 1) + 10, 0);

                    _destination = _lineDrawPos;
                    _dist = Vector3.Distance(_origin, _destination);

                    // 1st line dot
                    _lineDot1Pos = new Vector3(_lineSpawnPos.x, _lineSpawnPos.y + _linedotSize / 2, _lineSpawnPos.z);

                    // 2nd line dot
                    _lineDot2Pos = new Vector3(_lineSpawnPos.x,
                        _lineSpawnPos.y + _boxSize + (_boxOffset * 2) - _linedotSize / 2, _lineSpawnPos.z);

                    // 3rd line dot
                    _lineDot3Pos = new Vector3(_lineSpawnPos.x, _lineSpawnPos.y + _lineDrawPos.y - _linedotSize / 2,
                        _lineSpawnPos.z);

                    return colWinner;
                }

                rowWinner = 0;
                colWinner = 0;
            }

            if (diagWinner > -1)
            {
                float lineSize = (_boxSize * (grid - 1)) + (_boxOffset * 2) * (grid - 1);
                _lineSpawnPos = new Vector3((_boxOffset * 2) + 10 + _boxSize / 2,
                    _cardBorderTopGap + (_boxSize * grid + _boxOffset * (grid - 1)) - _boxSize / 2, -1);
                _lineDrawPos = new Vector3(lineSize, -lineSize + 15, 0);

                _destination = _lineDrawPos;
                _dist = Vector3.Distance(_origin, _destination);

                // 1st line dot
                _lineDot1Pos = _lineSpawnPos;

                // 2nd line dot
                _lineDot2Pos = new Vector3(_lineSpawnPos.x + (_boxSize + _boxOffset * 2),
                    _lineSpawnPos.y - (_boxSize + _boxOffset * 2) + 6, _lineSpawnPos.z);

                // 3rd line dot
                _lineDot3Pos = new Vector3(_lineSpawnPos.x + _lineDrawPos.x, _lineSpawnPos.y + _lineDrawPos.y,
                    _lineSpawnPos.z);

                return diagWinner;
            }

            if (antiDiagWinner > -1)
            {
                float lineSize = (_boxSize * (grid - 1)) + (_boxOffset * 2) * (grid - 1);
                _lineSpawnPos =
                    new Vector3((_boxOffset * 2) + 10 + (_boxSize * grid) + (_boxOffset * grid) - _boxSize / 2 + 10,
                        _cardBorderTopGap + (_boxSize * grid + _boxOffset * (grid - 1)) - _boxSize / 2, -1);
                _lineDrawPos = new Vector3(-lineSize, -lineSize + 15, 0);

                _destination = _lineDrawPos;
                _dist = Vector3.Distance(_origin, _destination);

                // 1st line dot
                _lineDot1Pos = _lineSpawnPos;

                // 2nd line dot
                _lineDot2Pos = new Vector3(_lineSpawnPos.x - (_boxSize + _boxOffset * 2),
                    _lineSpawnPos.y - (_boxSize + _boxOffset * 2) + 8, _lineSpawnPos.z);

                // 3rd line dot
                _lineDot3Pos = new Vector3(_lineSpawnPos.x + _lineDrawPos.x, _lineSpawnPos.y + _lineDrawPos.y,
                    _lineSpawnPos.z);

                return antiDiagWinner;
            }

            if (hasTied > -1)
            {
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
                            board[i, j] = _player;
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
                            board[i, j] = _ai;
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
            _border.transform.SetParent (player2Card.transform, false);
            _turnTxt.transform.SetParent (player2Card.transform, false);
            _turnTxt.text = "Bot's Turn";
            _downArrow.transform.SetParent (player2Card.transform, false);

            double bestScore = Double.PositiveInfinity;
            int[] bestMove = new int[2];

            for (int i = 0; i < grid; i++) {
                for (int j = 0; j < grid; j++) {
                    if (Board[i, j] == 0) {
                        Board[i, j] = _ai;
                        double score = Minimax (Board, 0, true);
                        Board[i, j] = 0;

                        if (bestScore > score) {
                            bestScore = score;
                            bestMove[0] = i;
                            bestMove[1] = j;
                        }
                    }
                }
            }

            // If the spot is the first value and it is not empty
            if (bestMove[0] == 0 && bestMove[1] == 0 && Board[0, 0] != 0) {

            } else {
                // Get game object and script
                GameObject box = GameObject.Find (bestMove[0] + "" + bestMove[1]);
                TagBoxAI boxScript = box.GetComponent<TagBoxAI> ();

                yield return new WaitForSeconds (_aiMoveDelay);

                boxScript.image.sprite = oImg;
                boxScript.clicked = true;
                Board[bestMove[0], bestMove[1]] = _ai;

                boardLen++;
                isAIMoving = false;
                HasEnded ();

                _border.transform.SetParent (player1Card.transform, false);
                _turnTxt.transform.SetParent (player1Card.transform, false);
                _turnTxt.text = "Your turn";
                _downArrow.transform.SetParent (player1Card.transform, false);
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
        private void GameOver (bool hasTied) {
            // If is already over
            if (isGameOver) {
                return;
            }
            
            // If AI won
            if (!hasTied)
            {
                // Draw lines
                _lineGen = Instantiate(line, _lineSpawnPos, Quaternion.identity) as GameObject;
                _lineGen.transform.SetParent(cardBorder.transform, false);
                _lineRend = _lineGen.GetComponent<LineRenderer>();

                _lineRend.material = oMat;
                _lineDotPr = blueLineDotPr;

                // 1st line dot
                _lineDot = Instantiate(_lineDotPr, _lineDot1Pos, Quaternion.identity);
                _lineDot.transform.SetParent(cardBorder.transform, false);

                // 2nd line dot
                _lineDot = Instantiate(_lineDotPr, _lineDot2Pos, Quaternion.identity);
                _lineDot.transform.SetParent(cardBorder.transform, false);

                // 3rd line dot
                _lineDot = Instantiate(_lineDotPr, _lineDot3Pos, Quaternion.identity);
                _lineDot.transform.SetParent(cardBorder.transform, false);

                _animateLine = true;
                _hasLost = true;
            }
            
            isGameOver = true;
        }
    }
}