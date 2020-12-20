using System.Collections;
using Animations;
using DG.Tweening;
using Multiplayer_Game;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace My_Photon.Rooms
{
    public class WaitingRoomController : MonoBehaviourPunCallbacks
    {
        public GameObject multiplayerGame;
        public GameObject privateMultiplayer;

        public Text underscoreText;
        public Text roomIdTxt;
        public Text player1CardText;
        public Text player2CardText;

        public string roomID;
        private string _currentTxt;
        
        public int grid;

        public float showRoomIDTxtDelay;

        private int _playerIndex;
        private float _loadSceneWait;
        private float _textFadeSpeed;
        private float _textFadeWait;
        private float _roomIDNumberGap;

        private bool _hasPlayerLeftRoom;
        private bool _hasCoroutineEnded;
        private bool _hasInitSetup;
        private bool _hasInitPlayerCards;
        private bool _hasUsedShakeAnim;

        private void Start()
        {
            _hasInitSetup = false;
            showRoomIDTxtDelay = 0.2f;
            _textFadeWait = 0.1f;
            _textFadeSpeed = 0.30f;
        }

        private void Setup()
        {
            PlayerCardAnimation.Instance.player1Card.gameObject.SetActive(false);
            PlayerCardAnimation.Instance.player2Card.gameObject.SetActive(false);

            // roomID = "9876";
            StartCoroutine(Fade());
            StartCoroutine(TextTypeAnimation());
            
            _playerIndex = PhotonNetwork.IsMasterClient ? 1 : 2;
            
            // If its the master client show only Player 1 Card
            if (_playerIndex == 1)
            {
                player1CardText.text = "You";
            }
            else
            {
                // else text
                player1CardText.text = "Player 1";
                player2CardText.text = "You";
            }
            
            _loadSceneWait = 2f;
            _roomIDNumberGap = 25;
            
            _hasPlayerLeftRoom = false;
            _hasInitPlayerCards = false;
            _hasUsedShakeAnim = false;
        }

        private new void OnDisable()
        {
            _hasInitSetup = false;
        }

        private void Update () {
            if (transform.position.x == 0 && !_hasInitPlayerCards)
            {
                if (_playerIndex == 2)
                {
                    // Show Player 2 Card
                    PlayerCardAnimation.Instance.ShowPlayerCard(false);
                }

                // Show Player 1 Card
                PlayerCardAnimation.Instance.ShowPlayerCard(true);
                _hasInitPlayerCards = true;
            }

            if (!_hasInitSetup)
            {
                Setup();
                _hasInitSetup = true;
            }

            // if back was pressed leave room
            if (Input.GetKeyDown (KeyCode.Escape))
            {
                LeaveRoom();
            }
        }

        // Go to Create Or Join Room
        public void LeaveRoom()
        {
            if (!PlayerCardAnimation.Instance.player2Card.gameObject.activeSelf)
            {
                PlayerCardAnimation.Instance.player2Card.gameObject.SetActive(false);
            }
            
            SceneManager.Instance.ChangeScene(gameObject.transform, privateMultiplayer.transform);
            PhotonNetwork.LeaveRoom();
            _hasPlayerLeftRoom = true;
        }

        private IEnumerator TextTypeAnimation()
        {
            var underscoreTxtRect = underscoreText.GetComponent<RectTransform>();
            var underscoreTxtAnchoredPos = underscoreTxtRect.anchoredPosition;
            var underscoreTxtX = 61.8f;
            underscoreText.GetComponent<RectTransform>().anchoredPosition = new Vector2(underscoreTxtX, underscoreTxtAnchoredPos.y);

            for (var i = 0; i <= roomID.Length; i++)
            {
                _currentTxt = roomID.Substring(0, i);
                roomIdTxt.text = _currentTxt;
                yield return new WaitForSeconds(showRoomIDTxtDelay);
                if (i != roomID.Length - 2)
                {
                    underscoreTxtX += _roomIDNumberGap;
                    underscoreText.GetComponent<RectTransform>().anchoredPosition = new Vector2(underscoreTxtX, underscoreTxtAnchoredPos.y);
                }
            }
        }

        private IEnumerator Fade()
        {
            var textColor = underscoreText.color;
            
            for (var x = 0; x != roomID.Length - 2; x++)
            {
                for (float i = 0; i <= 1.0f; i += _textFadeSpeed)
                {
                    underscoreText.color = new Color(textColor.r, textColor.g, textColor.b, i);
                    yield return new WaitForSeconds(_textFadeWait);
                }

                underscoreText.color = new Color(textColor.r, textColor.g, textColor.b, 1);

                for (float i = 1; i >= 0.25f; i -= _textFadeSpeed)
                {
                    underscoreText.color = new Color(textColor.r, textColor.g, textColor.b, i);
                    yield return new WaitForSeconds(_textFadeWait);
                }

                underscoreText.color = new Color(textColor.r, textColor.g, textColor.b, 0.25f);
            }
            
            // Make underscore fully transparent
            underscoreText.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log("Player entered room");
            player2CardText.text = "Player 2";
            _hasPlayerLeftRoom = false;
            
            // Show Player 2 Card
            PlayerCardAnimation.Instance.ShowPlayerCard(false);
            photonView.RPC("SetGridValue", RpcTarget.All, grid);
            photonView.RPC("LoadScene", RpcTarget.All);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log("Player left");
            // Set Player left room to true and disable 2nd player card
            _hasPlayerLeftRoom = true;
            
            // Hide Player 2 Card
            PlayerCardAnimation.Instance.HidePlayerCard(false);
            
            Debug.Log("Am I master = " + PhotonNetwork.IsMasterClient + " and am i 2nd player = " + _playerIndex);
            // If Im the new master and the 2nd player then change me into X player card
            if (PhotonNetwork.IsMasterClient && _playerIndex == 2)
            {
                Debug.Log("Master client left");
                // If shake animation hasn't been used yet
                if (!_hasUsedShakeAnim)
                {
                    PlayerCardAnimation.Instance.ChangePlayer1Card();
                    _hasUsedShakeAnim = true;
                }
                
                _playerIndex = 1;
            }
            else
            {
                player1CardText.text = "You";
            }
            
            player2CardText.text = "Player 2";
        }

        [PunRPC]
        public void LoadScene()
        {
            Debug.Log("LoadScene RPC called");
            StartCoroutine(LoadSceneCoroutine());
        }

        [PunRPC]
        public void SetGridValue(int gridVal)
        {
            grid = gridVal;
        }
        
        // Crappy code 101
        private IEnumerator LoadSceneCoroutine()
        {
            yield return new WaitForSeconds(_loadSceneWait);
            // If the other player hasn't left
            if (!_hasPlayerLeftRoom)
            {
                Debug.Log("Player has not left, joining the game grid = " + grid);
                // Make current room not join-able and invisible
                PhotonNetwork.CurrentRoom.IsVisible = false;
                PhotonNetwork.CurrentRoom.IsOpen = false;
                
                var multiplayerGameScript = multiplayerGame.GetComponent<TicTacToeCreatorMp>();
                multiplayerGameScript.grid = grid;
                
                SceneManager.Instance.ChangeScene(transform, multiplayerGame.transform);
            }
        }
    }
}