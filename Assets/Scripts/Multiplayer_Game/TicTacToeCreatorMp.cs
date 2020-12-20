﻿using Confetti;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer_Game
{
    public class TicTacToeCreatorMp : MonoBehaviourPunCallbacks
    {
        #region Variables

        public static TicTacToeCreatorMp Instance;

        public GameObject menu;
        public GameObject confettiWindow;
        public GameObject cardBorderProps;
        public GameObject cardBorder;
        public GameObject line;
        public GameObject card;

        public Material xMat;
        public Material oMat;

        public Transform xConfetti;
        public Transform oConfetti;

        public Sprite loserSprite;
        public Sprite winnerSprite;
        public Sprite handshakeSprite;

        public Image endImage;
        public Image downArrowPr;
        public Image player1Card;
        public Image player2Card;
        public Image playerCardBorder;
        public Image blueLineDotPr;
        public Image redLineDotPr;

        public Text turnTxtPr;
        public Text endTxt;
        public Text player1CardText;
        public Text player2CardText;

        private GameObject _windowConfetti;
        private GameObject _lineGen;
        private LineRenderer _lineRend;
        private Canvas _canvas;
        private Image _downArrow;
        private Image _border;
        private Text _turnTxt;
        private Text _winnerTxt;
        private Text _loserTxt;
        private Text _cardPlayerWinnerTxt;
        private Image _lineDot;
        private Image _lineDotPr;

        private Button _menuButton;
        private Button _playAgainButton;

        public int[,] Board;
        public int grid;
        public int boardLen;

        private int _hasTied = 3;

        public bool isGameOver;
        public bool p1;

        private RectTransform _rt;
        private RectTransform _cardBoarderRt;
        private TagBoxMp _boxScript;
        private GameObject _box;

        private Vector3 _lineDot1Pos;
        private Vector3 _lineDot2Pos;
        private Vector3 _lineDot3Pos;
        private Vector3 _lineDot4Pos;
        private Vector3 _lineDot5Pos;

        private Vector3 _lineSpawnPos;
        private Vector3 _lineDrawPos;
        private Vector3 _destination;
        private Vector3 _origin;

        private float _showButtonsTimer;
        private float _counter;
        private float _dist;
        private float _boxSize;
        private float _cardW;
        private float _cardH;
        private float _randparticleSize;
        private float _canvasW;
        private float _canvasH;
        private float _gapX;
        private float _drawSpeed;
        private float _drawDelay;
        private float _boxGridHeight;
        private float _boxOffset;
        private float _linedotSize;
        private float _cardBorderTopGap;
        private float _cardBorderXGaps;
        private float _cardBorderWidth;

        private int _rowCount;
        private int _colCount;
        private int _playerIndex = 0;
        private int _lastGrid;

        private bool _hasDrawn = false;
        private bool _showedEndImg = false;
        private bool _animateLine = false;
        public bool isMyTurn = false;
        private bool _hasWon;
        private bool _hasDisabled;
        private bool _hasPlayerLeft;

        #endregion

        #region Initially set variables board

        private void Start()
        {
            Instance = this;

            // Canvas & UI
            _canvas = FindObjectOfType<Canvas>();
            _canvasW = _canvas.GetComponent<RectTransform>().rect.width;
            _canvasH = _canvas.GetComponent<RectTransform>().rect.height;
            _cardBoarderRt = cardBorder.GetComponent<RectTransform>();
        }

        private new void OnDisable()
        {
            _hasDisabled = true;
            // If someone has won
            if (_hasWon)
            {
                GameObject.Destroy(_cardPlayerWinnerTxt);
            }

            if (_windowConfetti)
            {
                Debug.Log("Destroying confetti");
                GameObject.Destroy(_windowConfetti.gameObject);
            }
        }

        private new void OnEnable()
        {
            Setup();
        }

        private void Setup()
        {
            Debug.Log("Setup() was called");
            _playerIndex = PhotonNetwork.IsMasterClient ? 1 : 2;
            // If it has not been set yet
            if (_cardBorderXGaps == 0)
            {
                _cardBorderXGaps = cardBorderProps.GetComponent<RectTransform>().anchoredPosition.x - 250;
                _cardBorderWidth = cardBorderProps.GetComponent<RectTransform>().rect.width;
            }

            Board = new int[grid, grid];

            _boxGridHeight = 50;
            _cardBorderTopGap = _boxGridHeight;

            _counter = 0;
            _colCount = 0;
            _rowCount = 0;

            if (grid == 5)
            {
                _boxOffset = 5;
            }
            else
            {
                _boxOffset = 10;
            }

            if (grid != 3)
            {
                // Calculate Size
                _boxSize = _cardBorderWidth / grid - _boxOffset * (grid - 1);
            }
            else
            {
                _boxSize = 170;
            }

            Debug.Log("Current grid = " + grid + ", Previous grid = " + _lastGrid);

            // If Board has been used then destroy all children and create new board
            if (boardLen > 0 || grid != _lastGrid)
            {
                foreach (Transform child in cardBorder.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }

                // Initialize boxes
                InitBoxGrid();
            }

            if (_downArrow)
            {
                // Disable arrow after being disabled
                GameObject.Destroy(_downArrow.gameObject);
            }

            if (_winnerTxt)
            {
                Destroy(_winnerTxt.gameObject);
                Destroy(_loserTxt.gameObject);
            }

            endImage.gameObject.SetActive(false);

            if (!_border)
            {
                _border = Instantiate(playerCardBorder, new Vector3(0, 0, 0), Quaternion.identity);
            }

            _border.transform.SetParent(player1Card.transform, false);

            if (!_turnTxt)
            {
                _turnTxt = Instantiate(turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
            }

            _turnTxt.transform.SetParent(player1Card.transform, false);

            _downArrow = Instantiate(downArrowPr, downArrowPr.transform.position, Quaternion.identity);
            _downArrow.transform.SetParent(player1Card.transform, false);

            _border.transform.SetParent(player1Card.transform, false);

            _turnTxt.transform.SetParent(player1Card.transform, false);

            cardBorder.SetActive(true);

            if (_playerIndex == 1)
            {
                isMyTurn = true;
                _turnTxt.text = "Your Turn";
                player1CardText.text = "You";
            }
            else
            {
                isMyTurn = false;
                _turnTxt.text = "Player 1's Turn";
                player2CardText.text = "You";
            }

            // Integers
            boardLen = 0;

            _drawSpeed = 3f;
            _drawDelay = 0.1f;
            _showButtonsTimer = 1.5f;
            _linedotSize = 15;
            _lastGrid = grid;

            p1 = false;
            isMyTurn = true;

            _origin = Vector3.zero;

            // Booleans
            isGameOver = false;
            _hasDrawn = false;
            _showedEndImg = false;
            _animateLine = false;
            _hasPlayerLeft = false;
            
            // Reset the gap on the x axis every new column
            if (grid == 5)
            {
                _gapX = _boxOffset * (grid + 1);
            }
            else if (grid == 4)
            {
                _gapX = _boxOffset * (grid - 1);
            }
            else
            {
                _gapX = _boxOffset * grid;
            }

            // _lineGen = Instantiate(line, _lineSpawnPos, Quaternion.identity) as GameObject;
            // _lineGen.transform.SetParent(cardBorder.transform, false);
            // _lineRend = _lineGen.GetComponent<LineRenderer>();
            //
            // _lineRend.material = xMat;
            // _lineDotPr = redLineDotPr;
            //
            // // 1st line dot
            // _lineDot = Instantiate(_lineDotPr, _lineDot1Pos, Quaternion.identity);
            // _lineDot.transform.SetParent(cardBorder.transform, false);
            //
            // // 2nd line dot
            // _lineDot = Instantiate(_lineDotPr, _lineDot2Pos, Quaternion.identity);
            // _lineDot.transform.SetParent(cardBorder.transform, false);
            //
            // // 3rd line dot
            // _lineDot = Instantiate(_lineDotPr, _lineDot3Pos, Quaternion.identity);
            // _lineDot.transform.SetParent(cardBorder.transform, false);
            //
            // // 4th line dot
            // if (grid != 3)
            // {
            //     Debug.Log("Instantiate setting line dot 4 position");
            //     // 3rd line dot
            //     _lineDot = Instantiate(_lineDotPr, _lineDot4Pos, Quaternion.identity);
            //     _lineDot.transform.SetParent(cardBorder.transform, false);
            // }
            //
            // if (grid == 5)
            // {
            //     // 3rd line dot
            //     _lineDot = Instantiate(_lineDotPr, _lineDot5Pos, Quaternion.identity);
            //     _lineDot.transform.SetParent(cardBorder.transform, false);
            // }
            //
            // _animateLine = true;
        }

        #endregion

        private void InitBoxGrid()
        {
            Debug.Log("InitBoxGrid()");
            // Give size of boxes
            card.GetComponent<RectTransform>().sizeDelta = new Vector2(_boxSize, _boxSize);

            for (int i = 0; i < grid; i++)
            {
                // Reset the gap on the x axis every new column
                if (grid == 5)
                {
                    _gapX = _boxOffset * (grid + 1);
                }
                else if (grid == 4)
                {
                    _gapX = _boxOffset * (grid - 1);
                }
                else
                {
                    _gapX = _boxOffset * grid;
                }

                for (int j = 0; j < grid; j++)
                {
                    _box = Instantiate(card, new Vector3(_gapX, _boxGridHeight, 0), Quaternion.identity) as GameObject;
                    _box.transform.SetParent(cardBorder.transform, false);
                    _box.name = _colCount + "" + _rowCount;
                    _boxScript = _box.GetComponent<TagBoxMp>();

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
            
            if (grid == 5)
            {
                _gapX = _boxSize / 3 - _boxOffset;
            }
            else if (grid == 4)
            {
                _gapX = _boxOffset * (grid - 1);
            }
            else
            {
                _gapX = _boxOffset * grid;
            }
            
            _boxGridHeight = 50;
            _cardBorderTopGap = _boxGridHeight;
        }

        // Animate line
        private void Update()
        {
            // Go back to menu if back was pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                LeaveGame();
            }

            if (_animateLine)
            {
                if (_counter < _dist)
                {
                    _counter += _drawDelay / _drawSpeed;
                    var x = Mathf.Lerp(0, _dist, _counter);

                    var a = _origin;
                    var b = _destination;

                    var aLine = x * Vector3.Normalize(b - a) + a;

                    _lineRend.SetPosition(1, aLine);
                }
                else
                {
                    _animateLine = false;
                }
            }

            if (isGameOver && !_showedEndImg)
            {
                if (_showButtonsTimer > 0)
                {
                    _showButtonsTimer -= Time.deltaTime;
                }
                else
                {
                    Debug.Log("Show End image");
                    // Disable
                    cardBorder.SetActive(false);

                    Destroy(_border.gameObject);
                    Destroy(_turnTxt.gameObject);
                    Destroy(_downArrow.gameObject);

                    if (!_hasDrawn)
                    {
                        // If player wins
                        if (_hasWon)
                        {
                            endTxt.text = "You win";

                            _winnerTxt = Instantiate(turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                            _winnerTxt.text = "Winner";

                            _loserTxt = Instantiate(turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                            _loserTxt.text = "Loser";

                            // If player card = X
                            if (_playerIndex == 1)
                            {
                                _winnerTxt.transform.SetParent(player1Card.transform, false);
                                _loserTxt.transform.SetParent(player2Card.transform, false);
                            }
                            else
                            {
                                // If this player card = O
                                _winnerTxt.transform.SetParent(player2Card.transform, false);
                                _loserTxt.transform.SetParent(player1Card.transform, false);
                            }

                            endImage.GetComponent<Image>().sprite = winnerSprite;
                        }
                        else
                        {
                            // If this player loses
                            endTxt.text = "You lose";

                            _winnerTxt = Instantiate(turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                            _winnerTxt.text = "Winner";

                            _loserTxt = Instantiate(turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                            _loserTxt.text = "Loser";

                            // If player card = X
                            if (_playerIndex == 1)
                            {
                                _loserTxt.transform.SetParent(player1Card.transform, false);
                                _winnerTxt.transform.SetParent(player2Card.transform, false);
                            }
                            else
                            {
                                // If this player card = O
                                _winnerTxt.transform.SetParent(player1Card.transform, false);
                                _loserTxt.transform.SetParent(player2Card.transform, false);
                            }

                            endImage.GetComponent<Image>().sprite = loserSprite;
                        }
                    }
                    else
                    {
                        _turnTxt = Instantiate(turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                        _turnTxt.transform.SetParent(player2Card.transform, false);
                        _turnTxt.text = "Draw";

                        _turnTxt = Instantiate(turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                        _turnTxt.transform.SetParent(player1Card.transform, false);

                        endImage.GetComponent<Image>().sprite = handshakeSprite;
                        endTxt.text = "Draw";
                    }

                    endImage.gameObject.SetActive(true);
                    _showedEndImg = true;
                }
            }
        }

        // Winner Checker
        private int HasMatched()
        {
            var n = grid;
            var rowWinner = 0;
            var colWinner = 0;
            var diagWinner = 0;
            var antiDiagWinner = 0;
            var hasTied = 3;

            for (var i = 0; i < n; i++)
            {
                if (i < n - 1)
                {
                    // Detect Diagonal and Anti Diagonal
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

                for (var j = 0; j < n; j++)
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
                    _lineSpawnPos = new Vector3(_gapX - 5 + (_boxSize - (_boxSize / 2)),
                        _cardBorderTopGap + (_boxSize * (1 + i) + (_boxOffset * i)) - (_boxSize / 2), -1);
                    _lineDrawPos = new Vector3((_boxSize * (grid - 1)) + (_boxOffset * 2) * (grid - 1) + 5, 0, 0);

                    _destination = _lineDrawPos;
                    _dist = Vector3.Distance(_origin, _destination);

                    // 1st line dot
                    _lineDot1Pos = new Vector3(_lineSpawnPos.x + _linedotSize / 2, _lineSpawnPos.y, _lineSpawnPos.z);

                    // 2nd line dot
                    _lineDot2Pos = new Vector3(_lineSpawnPos.x + _boxSize + (_boxOffset * 2) + _linedotSize / 2,
                        _lineSpawnPos.y, _lineSpawnPos.z);

                    // last line dot
                    _lineDot3Pos = new Vector3(_lineSpawnPos.x + _lineDrawPos.x, _lineSpawnPos.y, _lineSpawnPos.z);

                    if (grid != 3)
                    {
                        // Set the 3rd dot position values
                        _lineDot4Pos = new Vector3(_lineSpawnPos.x + (_boxSize * 2) + ((_boxOffset * 2) * 2) + 7,
                            _lineSpawnPos.y, _lineSpawnPos.z);
                    }

                    if (grid == 5)
                    {
                        // Set the 3rd dot position values
                        _lineDot5Pos = new Vector3(_lineSpawnPos.x + (_boxSize * 3) + ((_boxOffset * 2) * 3) + 7,
                            _lineSpawnPos.y, _lineSpawnPos.z);
                    }

                    return rowWinner;
                }

                if (colWinner > -1)
                {
                    // Draw lines
                    _lineSpawnPos = new Vector3(
                        _gapX + (_boxSize * (1 + i)) - (_boxSize / 2) + ((_boxOffset * 2) * i) + 1,
                        _cardBorderTopGap + _boxSize - (_boxSize / 2) - 5, -1);
                    _lineDrawPos = new Vector3(0, (_boxSize * (grid - 1)) + _boxOffset * (grid - 1) + 10, 0);

                    _destination = _lineDrawPos;
                    _dist = Vector3.Distance(_origin, _destination);

                    // 1st line dot
                    _lineDot1Pos = new Vector3(_lineSpawnPos.x, _lineSpawnPos.y + _linedotSize / 2, _lineSpawnPos.z);

                    // 2nd line dot
                    _lineDot2Pos = new Vector3(_lineSpawnPos.x,
                        _lineSpawnPos.y + _boxSize + _boxOffset + 3.5f, _lineSpawnPos.z);

                    // 3rd line dot
                    _lineDot3Pos = new Vector3(_lineSpawnPos.x, _lineSpawnPos.y + _lineDrawPos.y - _linedotSize / 2,
                        _lineSpawnPos.z);

                    if (grid != 3)
                    {
                        // Set the 3rd dot position values
                        _lineDot4Pos = new Vector3(_lineSpawnPos.x,
                            _lineSpawnPos.y + (_boxSize * 2) + (_boxOffset * 2) + 5, _lineSpawnPos.z);
                    }

                    if (grid == 5)
                    {
                        // Set the 3rd dot position values
                        _lineDot5Pos = new Vector3(_lineSpawnPos.x,
                            _lineSpawnPos.y + (_boxSize * 3) + (_boxOffset * 3) + 5, _lineSpawnPos.z);
                    }
                    
                    return colWinner;
                }

                rowWinner = 0;
                colWinner = 0;
            }

            if (diagWinner > -1)
            {
                float lineSize = (_boxSize * (grid - 1)) + (_boxOffset * 2) * (grid - 1);

                if (grid != 5)
                {
                    _lineSpawnPos = new Vector3((_boxOffset * 2) + 10 + _boxSize / 2,
                        _cardBorderTopGap + (_boxSize * grid + _boxOffset * (grid - 1)) - _boxSize / 2, -1);
                }
                else
                {
                    _lineSpawnPos = new Vector3((_boxOffset * 2) + 20 + _boxSize / 2,
                        _cardBorderTopGap + (_boxSize * grid + _boxOffset * (grid - 1)) - _boxSize / 2 - 2, -1);
                }

                if (grid == 3)
                {
                    _lineDrawPos = new Vector3(lineSize, -lineSize + 15, 0);
                }
                else if (grid == 4)
                {
                    _lineDrawPos = new Vector3(lineSize, -lineSize + 30, 0);
                }
                else
                {
                    _lineDrawPos = new Vector3(lineSize, -lineSize + 20, 0);
                }

                _destination = _lineDrawPos;
                _dist = Vector3.Distance(_origin, _destination);

                // 1st line dot
                _lineDot1Pos = _lineSpawnPos;

                // 2nd line dot
                _lineDot2Pos = new Vector3(_lineSpawnPos.x + (_boxSize + _boxOffset * 2),
                    _lineSpawnPos.y - (_boxSize + _boxOffset), _lineSpawnPos.z);

                // 3rd line dot
                _lineDot3Pos = new Vector3(_lineSpawnPos.x + _lineDrawPos.x, _lineSpawnPos.y + _lineDrawPos.y,
                    _lineSpawnPos.z);

                if (grid == 4)
                {
                    Debug.Log("Grid = " + grid + " setting line dot 4 position, _boxGridHeight = " + _boxGridHeight);
                    // Set the 3rd dot position values
                    _lineDot4Pos = new Vector3(_lineSpawnPos.x + (_boxSize * 2) + _boxOffset * 2 * 2,
                        _boxGridHeight + _boxSize * 2 - _boxSize / 2 + _boxOffset, _lineSpawnPos.z);
                }

                if (grid == 5)
                {
                    _lineDot4Pos = new Vector3(_lineSpawnPos.x + _boxSize * 2 + _boxOffset * 2 * 2,
                        _boxGridHeight + _boxSize * 3 - _boxSize / 2 + _boxOffset * 2 - 2, _lineSpawnPos.z);

                    _lineDot5Pos = new Vector3(_lineSpawnPos.x + (_boxSize * 3) + _boxOffset * 2 * 3,
                        _boxGridHeight + _boxSize * 2 - _boxSize / 2 + _boxOffset - 2, _lineSpawnPos.z);
                }

                return diagWinner;
            }

            if (antiDiagWinner > -1)
            {
                var lineSize = _boxSize * (grid - 1) + (_boxOffset * 2) * (grid - 1);

                if (grid == 3)
                {
                    _lineSpawnPos =
                        new Vector3((_boxOffset * 2) + 10 + (_boxSize * grid) + (_boxOffset * grid) - _boxSize / 2 + 10,
                            _cardBorderTopGap + (_boxSize * grid + _boxOffset * (grid - 1)) - _boxSize / 2, -1);
                    _lineDrawPos = new Vector3(-lineSize, -lineSize + 15, 0);
                }
                else if (grid == 4)
                {
                    _lineSpawnPos =
                        new Vector3((_boxOffset * 2) + 20 + (_boxSize * grid) + (_boxOffset * grid) - _boxSize / 2 + 10,
                            _cardBorderTopGap + (_boxSize * grid + _boxOffset * (grid - 1)) - _boxSize / 2, -1);
                    _lineDrawPos = new Vector3(-lineSize, -lineSize + 28, 0);
                }
                else
                {
                    _lineSpawnPos =
                        new Vector3((_boxOffset * 2) + 25 + (_boxSize * grid) + (_boxOffset * grid) - _boxSize / 2 + 10,
                            _cardBorderTopGap + (_boxSize * grid + _boxOffset * (grid - 1)) - _boxSize / 2, -1);
                    _lineDrawPos = new Vector3(-lineSize, -lineSize + 18, 0);
                }

                _destination = _lineDrawPos;
                _dist = Vector3.Distance(_origin, _destination);

                // 1st line dot
                _lineDot1Pos = _lineSpawnPos;

                if (grid == 5)
                {
                    _lineDot2Pos = new Vector3(_lineSpawnPos.x - (_boxSize + _boxOffset * 2),
                        _lineSpawnPos.y - (_boxSize + _boxOffset * 2) + 5, _lineSpawnPos.z);
                }
                else if (grid == 3)
                {
                    _lineDot2Pos = new Vector3(_lineSpawnPos.x - (_boxSize + _boxOffset * 2),
                        _lineSpawnPos.y - (_boxSize + _boxOffset * 2) + 8, _lineSpawnPos.z);
                }
                else
                {
                    _lineDot2Pos = new Vector3(_lineSpawnPos.x - (_boxSize + _boxOffset * 2),
                        _lineSpawnPos.y - (_boxSize + _boxOffset * 2) + 10, _lineSpawnPos.z);
                }


                // 3rd line dot
                _lineDot3Pos = new Vector3(_lineSpawnPos.x + _lineDrawPos.x, _lineSpawnPos.y + _lineDrawPos.y,
                    _lineSpawnPos.z);

                if (grid == 4)
                {
                    // Set the 3rd dot position values
                    _lineDot4Pos = new Vector3(_lineSpawnPos.x - (_boxSize * 2 + _boxOffset * 2 * 2),
                        _lineSpawnPos.y - (_boxSize * 2 + _boxOffset * 3) + 10, _lineSpawnPos.z);
                }

                if (grid == 5)
                {
                    _lineDot4Pos = new Vector3(_lineSpawnPos.x - (_boxSize * 2 + _boxOffset * 2 * 2),
                        _lineSpawnPos.y - (_boxSize * 2 + _boxOffset * 3) + 5, _lineSpawnPos.z);

                    _lineDot5Pos = new Vector3(_lineSpawnPos.x - (_boxSize * 3 + _boxOffset * 2 * 3),
                        _lineSpawnPos.y - (_boxSize * 3 + _boxOffset * 4) + 5, _lineSpawnPos.z);
                }
            
                return antiDiagWinner;
            }

            if (hasTied > -1)
            {
                return 3;
            }

            return -1;
        }
        // Change player and check winner
        private void ChangePlayer()
        {
            if (p1)
            {
                p1 = false;
                _border.transform.SetParent(player2Card.transform, false);
                _turnTxt.transform.SetParent(player2Card.transform, false);
                _downArrow.transform.SetParent(player2Card.transform, false);
                if (_playerIndex == 2)
                {
                    _turnTxt.text = "Your Turn";
                }
                else
                {
                    _turnTxt.text = "Player 2's Turn";
                }
            }
            else
            {
                Debug.Log("Changing player to 1");
                p1 = true;
                _border.transform.SetParent(player1Card.transform, false);
                _turnTxt.transform.SetParent(player1Card.transform, false);
                _downArrow.transform.SetParent(player1Card.transform, false);
                if (_playerIndex == 1)
                {
                    _turnTxt.text = "Your Turn";
                }
                else
                {
                    _turnTxt.text = "Player 1's Turn";
                }
            }
            p1 = false;
        }

        // Check if someone won
        public void IsGameOver()
        {
            // X = 1, O = 2, Tie = 3
            // if (boardLen >= grid * 2 - 1)
            if (boardLen >= grid)
            {
                int result = HasMatched();

                if (result != -1)
                {
                    if (result == 3)
                    {
                        Debug.Log("Calling Game Over");
                        GameOver(result, false);
                    }
                    else
                    {
                        Debug.Log("Calling Game Over");
                        GameOver(result, true);
                    }
                }
            }
        }

        public void CallRpc(int boxColNum, int boxRowNum)
        {
            base.photonView.RPC("MarkBox", RpcTarget.All, _playerIndex, boxColNum, boxRowNum);
            ChangePlayer();
        }

        public void LeaveGame()
        {
            SceneManager.Instance.ChangeScene(transform, menu.transform);

            // If the other and current player leaves then destroy the room
            PhotonNetwork.LeaveRoom();
        }

        [PunRPC]
        public void MarkBox(int playerIndex, int boxColNum, int boxRowNum)
        {
            if (playerIndex == _playerIndex)
            {
                isMyTurn = false;
                return;
            }

            _box = GameObject.Find(boxColNum + "" + boxRowNum);
            _boxScript = _box.GetComponent<TagBoxMp>();

            _boxScript.UseBox(playerIndex);
            ChangePlayer();
            isMyTurn = true;
            IsGameOver();
        }

        [PunRPC]

        // If player leaves in the middle of the game
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            // If Game Is'nt over
            if (!isGameOver)
            {
                // If the opponent left then current player won
                GameOver(_playerIndex, false);
            }

            _hasPlayerLeft = true;
        }

        private void GameOver(int winner, bool hasMatched)
        {
            // Since there cant be 2 or more game overs return;
            if (isGameOver)
            {
                return;
            }

            // If there is a winner
            if (winner != 3)
            {
                // If player has matched draw lines
                if (hasMatched)
                {
                    Debug.Log("Initiating line at = " + _lineSpawnPos);
                    _lineGen = Instantiate(line, _lineSpawnPos, Quaternion.identity) as GameObject;
                    _lineGen.transform.SetParent(cardBorder.transform, false);
                    _lineRend = _lineGen.GetComponent<LineRenderer>();

                    // If Player 1 Wins
                    if (winner == 1)
                    {
                        _lineRend.material = xMat;
                        _lineDotPr = redLineDotPr;
                    }
                    else
                    {
                        // If Player 2 Wins
                        _lineRend.material = oMat;
                        _lineDotPr = blueLineDotPr;
                    }

                    // 1st line dot
                    _lineDot = Instantiate(_lineDotPr, _lineDot1Pos, Quaternion.identity);
                    _lineDot.transform.SetParent(cardBorder.transform, false);

                    // 2nd line dot
                    _lineDot = Instantiate(_lineDotPr, _lineDot2Pos, Quaternion.identity);
                    _lineDot.transform.SetParent(cardBorder.transform, false);

                    // 3rd line dot
                    _lineDot = Instantiate(_lineDotPr, _lineDot3Pos, Quaternion.identity);
                    _lineDot.transform.SetParent(cardBorder.transform, false);

                    // 4th line dot
                    if (grid != 3)
                    {
                        // 3rd line dot
                        _lineDot = Instantiate(_lineDotPr, _lineDot4Pos, Quaternion.identity);
                        _lineDot.transform.SetParent(cardBorder.transform, false);
                    }

                    if (grid == 5)
                    {
                        // 3rd line dot
                        _lineDot = Instantiate(_lineDotPr, _lineDot5Pos, Quaternion.identity);
                        _lineDot.transform.SetParent(cardBorder.transform, false);
                    }
                }


                // Only show confetti to the winner
                if (winner == _playerIndex)
                {
                    _windowConfetti =
                        Instantiate(confettiWindow, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                    _windowConfetti.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
                    _windowConfetti.transform.SetParent(_canvas.transform, false);
                    WindowConfetti windowScript = _windowConfetti.GetComponent<WindowConfetti>();

                    // If Player 1 Wins
                    if (winner == 1)
                    {
                        windowScript.pfConfetti = xConfetti;
                    }
                    else
                    {
                        // If Player 2 Wins
                        windowScript.pfConfetti = oConfetti;
                    }

                    _hasWon = true;
                }
                else
                {
                    _hasWon = false;
                }
            }
            else
            {
                _hasDrawn = true;
            }

            if (hasMatched)
            {
                _animateLine = true;
            }

            _showedEndImg = false;
            isGameOver = true;
        }
    }
}