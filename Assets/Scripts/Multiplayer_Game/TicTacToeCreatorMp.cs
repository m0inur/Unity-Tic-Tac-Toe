using Confetti;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer_Game
{
    public class TicTacToeCreatorMp : MonoBehaviourPunCallbacks {
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

        private void Start () {
            Instance = this;

            // Canvas & UI
            _canvas = FindObjectOfType<Canvas> ();
            _canvasW = _canvas.GetComponent<RectTransform> ().rect.width;
            _canvasH = _canvas.GetComponent<RectTransform> ().rect.height;
            _cardBoarderRt = cardBorder.GetComponent<RectTransform> ();
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
                _boxSize =  _cardBorderWidth / grid - _boxOffset * (grid - 1);
            }
            else
            {
                _boxSize = 170;
            }
            
            Debug.Log("Current grid = " + grid + ", Previous grid = " + _lastGrid);
            
            // If Board has been used then destroy all children and create new board
            if (boardLen > 0 || grid != _lastGrid)
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

            p1 = true;

            _origin = new Vector3(0, 0, 0);
            
            // Booleans
            isGameOver = false;
            _hasDrawn = false;
            _showedEndImg = false;
            _animateLine = false;
            _hasPlayerLeft = false;
        }

        #endregion

        private void InitBoxGrid ()
        {
            Debug.Log("InitBoxGrid()");
            // Give size of boxes
            card.GetComponent<RectTransform> ().sizeDelta = new Vector2 (_boxSize, _boxSize);

            for (int i = 0; i < grid; i++) {
                // Reset the gap on the x axis every new column
                if (grid == 5)
                {
                    _gapX = _boxOffset * (grid + 1);
                } else if (grid == 4)
                {
                    _gapX = _boxOffset * (grid - 1);
                }
                else
                {
                    _gapX = _boxOffset * grid;
                }

                for (int j = 0; j < grid; j++) {
                    _box = Instantiate (card, new Vector3 (_gapX, _boxGridHeight, 0), Quaternion.identity) as GameObject;
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

        // Animate line
        private void Update () {
            // Go back to menu if back was pressed
            if (Input.GetKeyDown (KeyCode.Escape)) {
                LeaveGame ();
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

                    Debug.Log("Row Matched with = " + rowWinner);
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
                    Debug.Log("Col Matched with = " + colWinner);

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
            base.photonView.RPC("MarkBox", RpcTarget.All,_playerIndex ,boxColNum, boxRowNum);
            ChangePlayer();
        }

        public void LeaveGame () {
            gameObject.SetActive(false);
            menu.SetActive(true);
            
            // If the other and current player leaves then destroy the room
            if (_hasPlayerLeft)
            {
                PhotonNetwork.LeaveRoom();
            }
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
            _boxScript = _box.GetComponent<TagBoxMp> ();

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

        private void GameOver (int winner, bool hasMatched) {
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
                }

                
                // Only show confetti to the winner
                if (winner == _playerIndex)
                {
                    _windowConfetti = Instantiate(confettiWindow, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
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
            else {
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