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
    public class WaitingRoom : MonoBehaviourPunCallbacks
    {
        public Image player2Card;
        
        public Text player1CardText;
        public Text player2CardText;
        
        private int _playerIndex;
        private float _loadSceneWait;
        
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
            // If createOrJoinRoom Game Object exists then it is Private_Multiplayer
            PhotonNetwork.LeaveRoom (true);
            PhotonNetwork.LoadLevel(SceneUtility.GetBuildIndexByScenePath("Assets/_Scenes/CreateOrJoinRoom.unity"));
        }

        public override void OnPlayerEnteredRoom (Player newPlayer) {
            // Show Player 2 Card when player joins
            player2Card.gameObject.SetActive(true);
            player2CardText.text = "Player 2";
            // StartCoroutine(LoadScene());
        }

        private IEnumerator LoadScene()
        {
            yield return new WaitForSeconds(_loadSceneWait);
            PhotonNetwork.LoadLevel(SceneUtility.GetBuildIndexByScenePath("Assets/_Scenes/Multiplayer_Game.unity"));
        }
    }
}