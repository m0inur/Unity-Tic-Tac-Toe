using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace My_Photon.Rooms
{
    public class WaitingRoomController : MonoBehaviourPunCallbacks
    {
        public GameObject multiplayerGame;
        public GameObject menu;

        public Image player2Card;
        
        public Text player1CardText;
        public Text player2CardText;
        
        private int _playerIndex;
        private float _loadSceneWait;
        private bool _hasPlayerLeftRoom;
        private bool _hasCoroutineEnded;
        
        private void Start()
        {
            _playerIndex = PhotonNetwork.IsMasterClient ? 1 : 2;
            _loadSceneWait = 2f;
            
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
        }

        private void Update () {
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
            menu.SetActive(true);
            
            // If the master client leaves the room
            if (PhotonNetwork.IsMasterClient)
            {
                // Destroy the room
                PhotonNetwork.LeaveRoom(true);
            }
            else
            {
                _hasPlayerLeftRoom = true;
                player2Card.gameObject.SetActive(false);
            }
        }

        public override void OnPlayerEnteredRoom (Player newPlayer) {
            Debug.Log("Player entered room");
            // Show Player 2 Card when player joins
            player2Card.gameObject.SetActive(true);
            player2CardText.text = "Player 2";
            _hasPlayerLeftRoom = false;
            base.photonView.RPC("LoadScene", RpcTarget.All);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            // If the master client leaves the room
            if (otherPlayer.IsMasterClient)
            {
                // Destroy the room
                PhotonNetwork.LeaveRoom(true);
            }
            else
            {
                // If player 2 has left
                _hasPlayerLeftRoom = true;
                player2Card.gameObject.SetActive(false);
            }
        }

        [PunRPC]
        public void LoadScene()
        {
            StartCoroutine(MakeDelay(_loadSceneWait));
        }

        // Crappy code 101
        private IEnumerator MakeDelay(float wait)
        {
            yield return new WaitForSeconds(wait);
            gameObject.SetActive(false);
            multiplayerGame.SetActive(true);
        }
    }
}