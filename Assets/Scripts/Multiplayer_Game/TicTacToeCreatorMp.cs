﻿using Confetti;
using My_Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Multiplayer_Game
{
    public class TicTacToeCreatorMp : MonoBehaviourPunCallbacks {
        #region Variables
        public static TicTacToeCreatorMp Instance;
        
        public GameObject menu;
        public GameObject confettiWindow;
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

        public int[, ] Board;
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

        private int _rowCount;
        private int _colCount;
        private int _playerIndex = 0;

        private int _n;
        private bool _hasDrawn = false;
        private bool _showedEndImg = false;
        private bool _animateLine = false;
        public bool isMyTurn = false;
        private bool _hasWon;
        #endregion

        #region Initially set variables board

        private void Start () {
            Board = new int[grid, grid];
            endImage.gameObject.SetActive (false);
            Instance = this;
        
            _border = Instantiate (playerCardBorder, new Vector3 (0, 0, 0), Quaternion.identity);
            _border.transform.SetParent (player1Card.transform, false);

            _turnTxt = Instantiate (turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
            _turnTxt.transform.SetParent (player1Card.transform, false);

            _downArrow = Instantiate (downArrowPr, downArrowPr.transform.position, Quaternion.identity);
            _downArrow.transform.SetParent (player1Card.transform, false);

            _playerIndex = PhotonNetwork.IsMasterClient ? 1 : 2;
            // Integers
            _rowCount = 0;
            _colCount = 0;
            boardLen = 0;
            _boxOffset = 10;

            _drawSpeed = 5;
            _drawDelay = 0.1f;
            _boxGridHeight = 50;
            _showButtonsTimer = 1.5f;
            _linedotSize = 15;
            _cardBorderTopGap = _boxGridHeight;
            _n = grid;

            p1 = true;
            _hasDrawn = false;
        
            _origin = new Vector3 (0, 0, 0);

            // Canvas & UI
            _canvas = FindObjectOfType<Canvas> ();
            _canvasW = _canvas.GetComponent<RectTransform> ().rect.width;
            _canvasH = _canvas.GetComponent<RectTransform> ().rect.height;

            _cardBoarderRt = cardBorder.GetComponent<RectTransform> ();
            // Booleans
            isGameOver = false;

            _playerIndex = PhotonNetwork.IsMasterClient ? 1 : 2;
        
            if (_playerIndex == 1)
            {
                isMyTurn = true;
                _turnTxt.text = "Your Turn";
                GameObject.Find("Player 1 Card Text").GetComponent<Text>().text = "You";
            }
            else
            {
                _turnTxt.text = "Player 1's Turn";
                GameObject.Find("Player 2 Card Text").GetComponent<Text>().text = "You";
            }
        
            _boxSize = 170;
        
            // Initialize boxes
            InitBoxGrid ();
        }
        
        private void OnDisable() 
        {
            Board = new int[grid, grid];
            // If someone has won
            if (_hasWon)
            {
                GameObject.Destroy(_windowConfetti);
                GameObject.Destroy(_cardPlayerWinnerTxt);
            }

            // If Board has been used then destroy all children and create new board
            if (boardLen > 0)
            {
                foreach (Transform child in cardBorder.transform) {
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
            _turnTxt.text = "Your Turn";

            // Integers
            _boxOffset = 10;
            boardLen = 0;

            _drawSpeed = 12;
            _drawDelay = 0.1f;
            _showButtonsTimer = 1.5f;
            _linedotSize = 15;
            _counter = 0;
            
            p1 = true;

            _origin = new Vector3(0, 0, 0);
            
            // Booleans
            isGameOver = false;
            _hasDrawn = false;
            _showedEndImg = false;
            _animateLine = false;
        }

        #endregion

        #region Init grid
        private void InitBoxGrid () {
            // Give size of boxes
            card.GetComponent<RectTransform> ().sizeDelta = new Vector2 (_boxSize, _boxSize);

            for (int i = 0; i < grid; i++) {
                // Reset the gap on the x axis every time
                _gapX = _boxOffset * 2;
                // gapX = boxSize / 2 + boxOffset;

                for (int j = 0; j < grid; j++) {
                    _box = Instantiate (card, new Vector3 (_gapX + 10, _boxGridHeight, 0), Quaternion.identity) as GameObject;
                    _box.transform.SetParent (cardBorder.transform, false);
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
        }

        #endregion

        // Animate line
        private void Update () {
            // Go back to menu if back was pressed
            if (Input.GetKeyDown (KeyCode.Escape)) {
                GoToMenu ();
            }

            if (_animateLine) {
                if (_counter < _dist) {
                    _counter += _drawDelay / _drawSpeed;
                    var x = Mathf.Lerp (0, _dist, _counter);

                    var a = _origin;
                    var b = _destination;

                    var aLine = x * Vector3.Normalize (b - a) + a;

                    _lineRend.SetPosition (1, aLine);
                } else {
                    _animateLine = false;
                }
            }

            if (isGameOver && !_showedEndImg) {
                if (_showButtonsTimer > 0) {
                    _showButtonsTimer -= Time.deltaTime;
                } else {
                    Debug.Log("Show End image");
                    // Disable
                    cardBorder.SetActive (false);

                    Destroy(_border.gameObject);
                    Destroy(_turnTxt.gameObject);
                    Destroy(_downArrow.gameObject);
                    
                    if (!_hasDrawn) {
                        _lineGen.gameObject.SetActive (false);
                        // If player wins
                        if (_hasWon) {
                            endTxt.text = "You win";

                            _winnerTxt = Instantiate (turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                            _winnerTxt.text = "Winner";

                            _loserTxt = Instantiate (turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                            _loserTxt.text = "Loser";
                            
                            // If player card = X
                            if (_playerIndex == 1)
                            {
                                _winnerTxt.transform.SetParent (player1Card.transform, false);
                                _loserTxt.transform.SetParent (player2Card.transform, false);
                            }
                            else
                            {
                                // If this player card = O
                                _winnerTxt.transform.SetParent (player2Card.transform, false);
                                _loserTxt.transform.SetParent (player1Card.transform, false);
                            }
                            
                            endImage.GetComponent<Image> ().sprite = winnerSprite;
                        } else {
                            // If this player loses
                            endTxt.text = "You lose";

                            _winnerTxt = Instantiate (turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                            _winnerTxt.text = "Winner";

                            _loserTxt = Instantiate (turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                            _loserTxt.text = "Loser";

                            // If player card = X
                            if (_playerIndex == 1)
                            {
                                _loserTxt.transform.SetParent (player1Card.transform, false);
                                _winnerTxt.transform.SetParent (player2Card.transform, false);
                            }
                            else
                            {
                                // If this player card = O
                                _winnerTxt.transform.SetParent (player1Card.transform, false);
                                _loserTxt.transform.SetParent (player2Card.transform, false);
                            }
                            
                            endImage.GetComponent<Image> ().sprite = loserSprite;
                        }
                    } else {
                        _turnTxt = Instantiate (turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                        _turnTxt.transform.SetParent (player2Card.transform, false);
                        _turnTxt.text = "Draw";

                        _turnTxt = Instantiate (turnTxtPr, turnTxtPr.transform.position, Quaternion.identity);
                        _turnTxt.transform.SetParent (player1Card.transform, false);

                        endImage.GetComponent<Image> ().sprite = handshakeSprite;
                        endTxt.text = "Draw";
                    }

                    endImage.gameObject.SetActive (true);
                    _showedEndImg = true;
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
                        diagWinner = Board[i, (n - 1) - i];
                        if (Board[i, (n - 1) - i] == 0 || Board[i, (n - 1) - i] != Board[i + 1, (n - 1) - i - 1]) {
                            diagWinner = -1;
                        }
                    }

                    if (antiDiagWinner > -1) {
                        antiDiagWinner = Board[i, i];
                        if (Board[i, i] == 0 || Board[i, i] != Board[i + 1, i + 1]) {
                            antiDiagWinner = -1;
                        }
                    }
                }

                for (int j = 0; j < n; j++) {
                    if (j < n - 1) {
                        if (rowWinner > -1) {
                            rowWinner = Board[i, j];
                            // If the row has a gap or doesnt match value then this row cant match
                            if (Board[i, j] == 0 || Board[i, j] != Board[i, j + 1]) {
                                rowWinner = -1;
                            }
                        }

                        if (colWinner > -1) {
                            colWinner = Board[j, i];
                            if (Board[j, i] == 0 || Board[j, i] != Board[j + 1, i]) {
                                colWinner = -1;
                            }
                        }
                    }

                    if (Board[i, j] == 0) {
                        hasTied = -1;
                    }
                }

                if (rowWinner > -1) {
                    // lineSpawnPos = new Vector3 ((boxOffset * 2) + 5 + (boxSize - (boxSize / 2)), 0, -1);
                    _lineSpawnPos = new Vector3 ((_boxOffset * 2) + 5 + (_boxSize - (_boxSize / 2)), _cardBorderTopGap + (_boxSize * (1 + i) + (_boxOffset * i)) - (_boxSize / 2), -1);
                    _lineDrawPos = new Vector3 ((_boxSize * (grid - 1)) + (_boxOffset * 2) * (grid - 1) + 5, 0, 0);

                    _destination = _lineDrawPos;
                    _dist = Vector3.Distance (_origin, _destination);

                    // 1st line dot
                    _lineDot1Pos = new Vector3 (_lineSpawnPos.x + _linedotSize / 2, _lineSpawnPos.y, _lineSpawnPos.z);

                    // 2nd line dot
                    _lineDot2Pos = new Vector3 (_lineSpawnPos.x + _boxSize + (_boxOffset * 2) + _linedotSize / 2, _lineSpawnPos.y, _lineSpawnPos.z);

                    // 3rd line dot
                    _lineDot3Pos = new Vector3 (_lineSpawnPos.x + _lineDrawPos.x, _lineSpawnPos.y, _lineSpawnPos.z);

                    return rowWinner;
                }

                if (colWinner > -1) {
                    // Draw lines
                    _lineSpawnPos = new Vector3 (10 + (_boxSize * (1 + i)) - (_boxSize / 2) + (_boxOffset * 2) * (1 + i), _cardBorderTopGap + _boxSize - (_boxSize / 2) - 5, -1);
                    _lineDrawPos = new Vector3 (0, (_boxSize * (grid - 1)) + _boxOffset * (grid - 1) + 10, 0);

                    _destination = _lineDrawPos;
                    _dist = Vector3.Distance (_origin, _destination);
                    
                    Debug.Log(colWinner + " won, set dist and destination");

                    // 1st line dot
                    _lineDot1Pos = new Vector3 (_lineSpawnPos.x, _lineSpawnPos.y + _linedotSize / 2, _lineSpawnPos.z);

                    // 2nd line dot
                    _lineDot2Pos = new Vector3 (_lineSpawnPos.x, _lineSpawnPos.y + _boxSize + (_boxOffset * 2) - _linedotSize / 2, _lineSpawnPos.z);

                    // 3rd line dot
                    _lineDot3Pos = new Vector3 (_lineSpawnPos.x, _lineSpawnPos.y + _lineDrawPos.y - _linedotSize / 2, _lineSpawnPos.z);

                    return colWinner;
                }

                rowWinner = 0;
                colWinner = 0;
            }

            if (diagWinner > -1) {
                float lineSize = (_boxSize * (grid - 1)) + (_boxOffset * 2) * (grid - 1);
                _lineSpawnPos = new Vector3 ((_boxOffset * 2) + 10 + _boxSize / 2, _cardBorderTopGap + (_boxSize * grid + _boxOffset * (grid - 1)) - _boxSize / 2, -1);
                _lineDrawPos = new Vector3 (lineSize, -lineSize + 15, 0);

                _destination = _lineDrawPos;
                _dist = Vector3.Distance (_origin, _destination);

                // 1st line dot
                _lineDot1Pos = _lineSpawnPos;

                // 2nd line dot
                _lineDot2Pos = new Vector3 (_lineSpawnPos.x + (_boxSize + _boxOffset * 2), _lineSpawnPos.y - (_boxSize + _boxOffset * 2) + 6, _lineSpawnPos.z);

                // 3rd line dot
                _lineDot3Pos = new Vector3 (_lineSpawnPos.x + _lineDrawPos.x, _lineSpawnPos.y + _lineDrawPos.y, _lineSpawnPos.z);

                return diagWinner;
            }

            if (antiDiagWinner > -1) {
                float lineSize = (_boxSize * (grid - 1)) + (_boxOffset * 2) * (grid - 1);
                _lineSpawnPos = new Vector3 ((_boxOffset * 2) + 10 + (_boxSize * grid) + (_boxOffset * grid) - _boxSize / 2 + 10, _cardBorderTopGap + (_boxSize * grid + _boxOffset * (grid - 1)) - _boxSize / 2, -1);
                _lineDrawPos = new Vector3 (-lineSize, -lineSize + 15, 0);

                _destination = _lineDrawPos;
                _dist = Vector3.Distance (_origin, _destination);

                // 1st line dot
                _lineDot1Pos = _lineSpawnPos;

                // 2nd line dot
                _lineDot2Pos = new Vector3 (_lineSpawnPos.x - (_boxSize + _boxOffset * 2), _lineSpawnPos.y - (_boxSize + _boxOffset * 2) + 8, _lineSpawnPos.z);

                // 3rd line dot
                _lineDot3Pos = new Vector3 (_lineSpawnPos.x + _lineDrawPos.x, _lineSpawnPos.y + _lineDrawPos.y, _lineSpawnPos.z);

                return antiDiagWinner;
            }

            if (hasTied > -1) {
                return 3;
            }

            return -1;
        }

        // Change player and check winner
        public void ChangePlayer () {
            if (p1) {
                p1 = false;
                _border.transform.SetParent (player2Card.transform, false);
                _turnTxt.transform.SetParent (player2Card.transform, false);
                _downArrow.transform.SetParent (player2Card.transform, false);
                if (_playerIndex == 2)
                { 
                    _turnTxt.text = "Your Turn";
                }
                else
                {
                    _turnTxt.text = "Player 2's Turn";
                }
            } else {
                Debug.Log("Changing player to 1");
                p1 = true;
                _border.transform.SetParent (player1Card.transform, false);
                _turnTxt.transform.SetParent (player1Card.transform, false);
                _downArrow.transform.SetParent (player1Card.transform, false);
                if (_playerIndex == 1)
                { 
                    _turnTxt.text = "Your Turn";
                }
                else
                {
                    _turnTxt.text = "Player 1's Turn";
                }
            }
        }
    
        // Check if someone won
        public void IsGameOver()
        {
            // X = 1, O = 2, Tie = 3
            if (boardLen >= grid * 2 - 1)
            {
                int result = HasMatched();

                if (result != -1)
                {
                    if (result == 3)
                    {
                        GameOver(result, false);
                    }
                    else
                    {
                        GameOver(result, true);
                    }
                }
            }
        }

        public void CallRpc(int boxColNum, int boxRowNum)
        {
            base.photonView.RPC("MarkBox", RpcTarget.All,_playerIndex ,boxColNum, boxRowNum);
            ChangePlayer();
        }

        public void GoToMenu () {
            gameObject.SetActive(false);
            menu.SetActive(true);
            PhotonNetwork.LeaveRoom();
        }

        public void PlayAgain () {
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
    
        [PunRPC]
        public void MarkBox(int playerIndex, int boxColNum, int boxRowNum)
        {
            Debug.Log("Rpc Mark box called");
            if (playerIndex == _playerIndex)
            {
                isMyTurn = false;
                return;
            }

            _box = GameObject.Find(boxColNum + "" + boxRowNum);
            _boxScript = _box.GetComponent<TagBoxMp> ();

            _boxScript.UseBox(playerIndex);
            ChangePlayer();
            isMyTurn = true;
            IsGameOver();
        }
        
        // If player leaves in the middle of the game
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            // If the opponent left then current player won
            GameOver(_playerIndex, false);
        }

        private void GameOver (int winner, bool hasMatched) {
            Debug.Log("Game Over: Player " + winner + " won");
            // If there is a winner
            if (winner != 3)
            {
                // If player has matched draw lines
                if (hasMatched)
                {
                    _lineGen = Instantiate(line, _lineSpawnPos, Quaternion.identity) as GameObject;
                    _lineGen.transform.SetParent(cardBorder.transform, false);
                    _lineRend = _lineGen.GetComponent<LineRenderer>();
                }

                
                // Only show confetti to the winner
                if (winner == _playerIndex)
                {
                    Debug.Log("player won");
                    GameObject confettiWindowObj = Instantiate(confettiWindow, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                    confettiWindowObj.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
                    confettiWindowObj.transform.SetParent(_canvas.transform, false);
                    WindowConfetti windowScript = confettiWindowObj.GetComponent<WindowConfetti>();
                    
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
            }
            else {
                _hasDrawn = true;
            }

            if (hasMatched)
            {
                _animateLine = true;
            }
            
            // Leave Current room
            PhotonNetwork.LeaveRoom();
            _showedEndImg = false;
            isGameOver = true;
        }
    }
}