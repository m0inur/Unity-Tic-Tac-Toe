using System;
using Confetti;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace LocalMultiplayer
{
    public class TicTacToeCreatorLm : MonoBehaviour
    {
        #region Variables

        public static TicTacToeCreatorLm Instance;

        public GameObject confettiWindowPr;
        public GameObject cardBorderProps;
        public GameObject cardBorder;
        public GameObject line;
        public GameObject card;
        public GameObject menu;
        
        public Material xMat;
        public Material oMat;
        public Transform xConfetti;
        public Transform oConfetti;

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

        private GameObject _lineGen;
        private LineRenderer _lineRend;
        private Canvas _canvas;
        private Image _downArrow;
        private Image _border;
        private Text _turnTxt;
        private Text _cardPlayerWinnerTxt;
        private Image _lineDot;
        private Image _lineDotPr;
        private WindowConfetti _windowConfettiScript;

        private Text _playerCard1EndTxt;
        private Text _playerCard2EndTxt;

        private Button _menuButton;
        private Button _playAgainButton;

        public int[,] Board;
        public int grid;
        public int boardLen;

        public bool isGameOver;
        public bool p1;

        private RectTransform _rt;
        private GameObject _box;
        private GameObject _windowConfetti;

        private Vector3 _lineDot1Pos;
        private Vector3 _lineDot2Pos;
        private Vector3 _lineDot3Pos;

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
        private int _lastGrid;

        private bool _hasDrawn;
        private bool _spawnButtons;
        private bool _animateLine;
        private bool _hasWon;

        #endregion

        #region Variable Setup

        private void Start()
        {
            // Initialize once
            
            Instance = this;

            // Canvas & UI
            _canvas = FindObjectOfType<Canvas>();
            _canvasW = _canvas.GetComponent<RectTransform>().rect.width;
            _canvasH = _canvas.GetComponent<RectTransform>().rect.height;
        }
        
        private void OnEnable()
        {
            Setup();
        }

        private void OnDisable()
        {
            // If someone has won
            if (_hasWon)
            {
                GameObject.Destroy(_windowConfetti);
                GameObject.Destroy(_cardPlayerWinnerTxt);
            }
        }

        private void Setup()
        {
            // If it has not been set yet
            if (_cardBorderXGaps == 0)
            {
                _cardBorderXGaps = cardBorderProps.GetComponent<RectTransform>().anchoredPosition.x - 250;
                _cardBorderWidth = cardBorderProps.GetComponent<RectTransform>().rect.width;
            }
            // Re-enable Card Border
            cardBorder.SetActive(true);

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
                _boxSize =  _cardBorderWidth / grid - _boxOffset * (grid - 1);
            }
            else
            {
                _boxSize = 170;
            }

            // If Board has been used Or if last and current grid doesn't match 
            if (boardLen > 0 || _lastGrid != grid)
            {
                Debug.Log("Destroy and create cards");
                foreach (Transform child in cardBorder.transform) {
                    GameObject.Destroy(child.gameObject);
                }
                
                // Initialize boxes
                InitBoxGrid();
            }
            
            // If Down arrow is not null
            if (_downArrow)
            {
                // Disable arrow after being disabled
                GameObject.Destroy(_downArrow.gameObject);
            }

            // If End text's are not yet destroyed
            if (_playerCard1EndTxt)
            {
                Destroy(_playerCard1EndTxt.gameObject);
                Destroy(_playerCard2EndTxt.gameObject);
            }

            endImage.gameObject.SetActive(false);

            // If there isn't a border instantiate it
            if (!_border)
            {
                _border = Instantiate(playerCardBorder, new Vector3(0, 0, 0), Quaternion.identity);
            }


            // If there isn't a turn txt instantiate it
            if (!_turnTxt)
            {
                _turnTxt = Instantiate(turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
            }
            
            _downArrow = Instantiate(downArrowPr, downArrowPr.transform.position, Quaternion.identity);
            
            // Set Parents
            _border.transform.SetParent(player1Card.transform, false);
            _turnTxt.transform.SetParent(player1Card.transform, false);
            _downArrow.transform.SetParent(player1Card.transform, false);

            Debug.Log("Resetting board, grid = " + grid);
            // Reset Board
            Board = new int[grid, grid];

            _turnTxt.text = "Your Turn";

            // Integers
            _boxOffset = 10;
            boardLen = 0;

            _drawSpeed = 3;
            _drawDelay = 0.1f;
            _showButtonsTimer = 1.5f;
            _linedotSize = 15;
            _counter = 0;
            _lastGrid = grid;

            _origin = new Vector3(0, 0, 0);
            
            // Booleans
            isGameOver = false;
            _hasDrawn = false;
            _spawnButtons = false;
            _animateLine = false;
            p1 = true;
        }

        #endregion
        
        private void InitBoxGrid()
        {
            // Give size of boxes
            card.GetComponent<RectTransform>().sizeDelta = new Vector2(_boxSize, _boxSize);
            
            _boxGridHeight = 50;
            _rowCount = 0;
            _colCount = 0;
            _cardBorderTopGap = _boxGridHeight;
            
            for (var i = 0; i < grid; i++)
            {
                // Reset the gap on the x axis every new column
                if (grid == 5)
                {
                    _gapX = _boxSize / 3 - _boxOffset;
                } else if (grid == 4)
                {
                    _gapX = _boxOffset * (grid - 1);
                }
                else
                {
                    _gapX = _boxOffset * grid;
                }

                for (var j = 0; j < grid; j++)
                {
                    _box = Instantiate(card, new Vector3(_gapX, _boxGridHeight, 0),
                        Quaternion.identity) as GameObject;
                    _box.transform.SetParent(cardBorder.transform, false);
                    var boxScript = _box.GetComponent<TagBoxLm>();
                    
                    boxScript.boxColNum = _colCount;
                    boxScript.boxRowNum = _rowCount;

                    // Update gap on X axis after evey box
                    _gapX += _boxSize + _boxOffset * 2;
                    _rowCount++;
                }

                _boxGridHeight += _boxSize + _boxOffset;
                _colCount++;
                _rowCount = 0;
            }
        }

        // Animate line
        private void Update()
        {
            // Go back to menu if back was pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Menu();
            }

            if (_animateLine && !_hasDrawn)
            {
                    if (_counter < _dist) {
                        _counter += _drawDelay / _drawSpeed;
                        var x = Mathf.Lerp (0, _dist, _counter);

                        var a = _origin;
                        var b = _destination;

                        var aLine = x * Vector3.Normalize (b - a) + a;

                        _lineRend.SetPosition (1, aLine);
                    }
                else
                {
                    _animateLine = false;
                }
            }

            if (isGameOver && !_spawnButtons)
            {
                if (_showButtonsTimer > 0)
                {
                    _showButtonsTimer -= Time.deltaTime;
                }
                else
                {
                    // Destroy
                    GameObject.Destroy(_border.gameObject);
                    GameObject.Destroy(_turnTxt.gameObject);
                    GameObject.Destroy(_downArrow.gameObject);
                    cardBorder.SetActive(false);

                    // If it didnt draw check who won
                    if (!_hasDrawn)
                    {
                        _lineGen.gameObject.SetActive(false);
                        if (!p1)
                        {
                            endTxt.text = "Player 1 Won";

                            _playerCard1EndTxt = Instantiate(turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                            _playerCard1EndTxt.transform.SetParent(player1Card.transform, false);
                            _playerCard1EndTxt.text = "Winner";

                            _playerCard2EndTxt = Instantiate(turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                            _playerCard2EndTxt.transform.SetParent(player2Card.transform, false);
                            _playerCard2EndTxt.text = "Loser";
                        }
                        else
                        {
                            endTxt.text = "Player 2 Won";

                            _playerCard1EndTxt = Instantiate(turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                            _playerCard1EndTxt.transform.SetParent(player2Card.transform, false);
                            _playerCard1EndTxt.text = "Winner";

                            _playerCard2EndTxt = Instantiate(turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                            _playerCard2EndTxt.transform.SetParent(player1Card.transform, false);
                            _playerCard2EndTxt.text = "Loser";
                        }

                        endImage.GetComponent<Image>().sprite = winnerSprite;
                    }
                    else
                    {
                        _playerCard1EndTxt = Instantiate(turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                        _playerCard1EndTxt.transform.SetParent(player2Card.transform, false);
                        _playerCard1EndTxt.text = "Draw";

                        _playerCard2EndTxt = Instantiate(turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                        _playerCard2EndTxt.transform.SetParent(player1Card.transform, false);
                        _playerCard2EndTxt.text = "Draw";

                        endImage.GetComponent<Image>().sprite = handshakeSprite;
                        endTxt.text = "Draw";
                    }

                    endImage.gameObject.SetActive(true);
                    _spawnButtons = true;
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

        // Change player and check winner
        public void ChangePlayer()
        {
            if (p1)
            {
                p1 = false;
                _border.transform.SetParent(player2Card.transform, false);
                _turnTxt.transform.SetParent(player2Card.transform, false);
                _turnTxt.text = "Player 2's Turn";
                _downArrow.transform.SetParent(player2Card.transform, false);
            }
            else
            {
                p1 = true;
                _border.transform.SetParent(player1Card.transform, false);
                _turnTxt.transform.SetParent(player1Card.transform, false);
                _turnTxt.text = "Your Turn";
                _downArrow.transform.SetParent(player1Card.transform, false);
            }

            if (boardLen >= grid * 2 - 1)
            {
                int result = HasMatched();

                if (result != -1)
                {
                    if (result == 3)
                    {
                        GameOver(true);
                    }
                    else
                    {
                        GameOver(false);
                    }
                }
            }
        }

        public void Menu()
        {
            menu.SetActive(true);
            gameObject.SetActive(false);
        }

        public void Restart()
        {
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
        
        private void GameOver(bool hasTied)
        {
            // If is already over
            if (isGameOver)
            {
                return;
            }

            if (!hasTied)
            {
                _hasWon = true;
                
                // Draw lines
                _lineGen = Instantiate(line, _lineSpawnPos, Quaternion.identity) as GameObject;
                _lineGen.transform.SetParent(cardBorder.transform, false);
                _lineRend = _lineGen.GetComponent<LineRenderer>();

                // Start the confetti
                _windowConfetti = Instantiate(confettiWindowPr, new Vector3(0, 0, 0), Quaternion.identity);
                _windowConfetti.transform.SetParent(_canvas.transform, false);
                _windowConfettiScript = _windowConfetti.GetComponent<WindowConfetti>();
                
                if (!p1)
                {
                    endTxt.text = "Winner";

                    _lineRend.material = xMat;
                    _windowConfettiScript.pfConfetti = xConfetti;
                    _lineDotPr = redLineDotPr;
                }
                else
                {
                    endTxt.text = "Winner";

                    _lineRend.material = oMat;
                    _windowConfettiScript.pfConfetti = oConfetti;
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
            }
            else
            {
                _hasDrawn = true;
                endTxt.text = "It's a Draw!";
            }

            // Disable back button
            isGameOver = true;
            _animateLine = true;
        }
    }
}