﻿using System;
using System.Collections;
using Multiplayer_Game;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace My_Photon.Rooms
{
    public class WaitingRoomController : MonoBehaviourPunCallbacks
    {
        public GameObject multiplayerGame;
        public GameObject privateMultiplayer;

        public Image player2Card;

        public Text underscoreText;
        public Text roomIdTxt;
        public Text player1CardText;
        public Text player2CardText;

        public string roomID;
        private string currentTxt;
        
        public int grid;

        public float showRoomIDTxtDelay;

        private int _playerIndex;
        private float _loadSceneWait;
        private float _textFadeSpeed;
        private float _textFadeWait;
        private float _roomIDNumberGap;

        private bool _hasPlayerLeftRoom;
        private bool _hasCoroutineEnded;
        private bool _hasInitRoomIDText;
        private bool _hasInitSetup;

        private void Start()
        {
            _hasInitSetup = false;
            showRoomIDTxtDelay = 0.2f;
            _textFadeWait = 0.1f;
            _textFadeSpeed = 0.30f;
            _roomIDNumberGap = 25;
        }

        private void Setup()
        {
            StartCoroutine(Fade());
            StartCoroutine(TextTypeAnimation());
            Debug.Log("Setup()");
            _playerIndex = PhotonNetwork.IsMasterClient ? 1 : 2;

            _hasInitRoomIDText = false;
            
            // If its the master client show only Player 1 Card
            if (_playerIndex == 1)
            {
                player1CardText.text = "You";
            }
            else
            {
                // else show both cards and change text
                player2Card.gameObject.SetActive(true);
                player1CardText.text = "Player 1";
                player2CardText.text = "You";
            }
            
            _loadSceneWait = 2f;
            _hasPlayerLeftRoom = false;
            _hasInitRoomIDText = false;
        }

        private new void OnDisable()
        {
            Debug.Log("Setting player2Card to false");
            player2Card.gameObject.SetActive(false);
            _hasInitSetup = false;
        }

        private void Update () {
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
            gameObject.SetActive(false);
            privateMultiplayer.SetActive(true);
            
            // If the master client leaves the room
            if (PhotonNetwork.IsMasterClient)
            {
                // Kick both players out the room
                base.photonView.RPC("LeaveRoomRPC", RpcTarget.All);
            }
            else
            {
                Debug.Log("Player has left the room");
                PhotonNetwork.LeaveRoom();
                _hasPlayerLeftRoom = true;
                Debug.Log("Setting player2Card to false");
                player2Card.gameObject.SetActive(false);
            }
        }

        private IEnumerator TextTypeAnimation()
        {
            var underscoreTxtRect = underscoreText.GetComponent<RectTransform>();
            var underscoreTxtAnchoredPos = underscoreTxtRect.anchoredPosition;
            var underscoreTxtX = underscoreTxtAnchoredPos.x;
            
            for (var i = 0; i <= roomID.Length; i++)
            {
                currentTxt = roomID.Substring(0, i);
                roomIdTxt.text = currentTxt;
                yield return new WaitForSeconds(showRoomIDTxtDelay);
                if (i != roomID.Length - 2)
                {
                    Debug.Log("Giving gap I = " + i);
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

                for (float i = 1; i >= 0f; i -= _textFadeSpeed)
                {
                    underscoreText.color = new Color(textColor.r, textColor.g, textColor.b, i);
                    yield return new WaitForSeconds(_textFadeWait);
                }

                underscoreText.color = new Color(textColor.r, textColor.g, textColor.b, 0);
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log("Player entered room");

            // Show Player 2 Card when player joins
            player2Card.gameObject.SetActive(true);
            player2CardText.text = "Player 2";
            _hasPlayerLeftRoom = false;
            base.photonView.RPC("LoadScene", RpcTarget.All);

            // Set grid value for second player
            if (PhotonNetwork.IsMasterClient)
            {
                base.photonView.RPC("SetGridValue", RpcTarget.Others, grid);
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            // If the master client leaves the room
            if (otherPlayer.IsMasterClient)
            {
                Debug.Log("Master client left");
                // Kick both players out the room
                base.photonView.RPC("LeaveRoomRPC", RpcTarget.All);
            }
            else
            {
                Debug.Log("Player has left the room");
                // If player 2 has left
                _hasPlayerLeftRoom = true;
                player2Card.gameObject.SetActive(false);
                Debug.Log("Setting player2Card to false");
            }
        }

        [PunRPC]
        public void LeaveRoomRPC()
        {
            PhotonNetwork.LeaveRoom();
            gameObject.SetActive(false);
            privateMultiplayer.SetActive(true);
        }

        [PunRPC]
        public void LoadScene()
        {
            Debug.Log("LoadScene RPC called");
            StartCoroutine(LoadSceneCoroutine());
        }

        [PunRPC]
        public void SetGridValue(int _grid)
        {
            grid = _grid;
        }
        
        // Crappy code 101
        private IEnumerator LoadSceneCoroutine()
        {
            Debug.Log("Started Coroutine, wait time = " + _loadSceneWait);
            yield return new WaitForSeconds(_loadSceneWait);
            Debug.Log("Coroutine Finished, _hasPlayerLeftRoom = " + _hasPlayerLeftRoom);
            // If the other player hasn't left
            if (!_hasPlayerLeftRoom)
            {
                Debug.Log("Player has not left");
                // Make current room not joinable and invisible
                PhotonNetwork.CurrentRoom.IsVisible = false;
                PhotonNetwork.CurrentRoom.IsOpen = false;
                
                var multiplayerGameScript = multiplayerGame.GetComponent<TicTacToeCreatorMp>();
                multiplayerGameScript.grid = grid;
                
                gameObject.SetActive(false);
                multiplayerGame.SetActive(true);
            }
        }
    }
}