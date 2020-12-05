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
        public GameObject menu;
        
        public Image player2Card;
        
        public Text player1CardText;
        public Text player2CardText;
        
        private int _playerIndex;
        private float _loadSceneWait;
        private bool hasPlayerLeftRoom;
        
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
                hasPlayerLeftRoom = true;
                player2Card.gameObject.SetActive(false);
            }
        }

        public override void OnPlayerEnteredRoom (Player newPlayer) {
            // Show Player 2 Card when player joins
            player2Card.gameObject.SetActive(true);
            player2CardText.text = "Player 2";
            hasPlayerLeftRoom = false;
            StartCoroutine(LoadScene());
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            // If the master client leaves the room
            if (PhotonNetwork.IsMasterClient)
            {
                // Destroy the room
                PhotonNetwork.LeaveRoom(true);
            }
            else
            {
                // If player 2 has left
                hasPlayerLeftRoom = true;
                player2Card.gameObject.SetActive(false);
            }
        }

        private IEnumerator LoadScene()
        {
            yield return new WaitForSeconds(_loadSceneWait);
            // If player hasn't left room continue to game
            if (!hasPlayerLeftRoom)
            {
                
            }
        }
    }
}