using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace My_Photon.Rooms
{
    public class CreateRoom : MonoBehaviourPunCallbacks
    {
        public GameObject waitingRoom;
        public GameObject privateMultiplayer;
        public GameObject pMButtons;
        
        public Button grid3Button;
        public Button grid4Button;
        public Button grid5Button;

        private string _roomId;
        private int _grid;

        private void Start()
        {
            _roomId = "";
            
            grid3Button.onClick.AddListener(OnClick_ButtonGrid3);
            grid4Button.onClick.AddListener(OnClick_ButtonGrid4);
            grid5Button.onClick.AddListener(OnClick_ButtonGrid5);
        }

        private void Update()
        {
            // if back was pressed leave room
            if (Input.GetKeyDown (KeyCode.Escape))
            {
                LeaveRoom();
            }
        }

        public void LeaveRoom()
        {
            SceneManager.Instance.ChangeScene(gameObject.transform, pMButtons.transform);
        }

        private new void OnDisable()
        {
            _roomId = "";
            
            gameObject.SetActive(false);
            pMButtons.SetActive(true);
        }

        public void OnClick_CreateRoom () {
            if (!PhotonNetwork.IsConnected) {
                Debug.Log("You are not connected");
                PhotonNetwork.Reconnect();
            } 

            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }

            _roomId = "";

            var options = new RoomOptions
            {
                PublishUserId = true, 
                MaxPlayers = 2,
                IsVisible = false
            };

            for (var i = 0; i < 4; i++)
            {
                _roomId += UnityEngine.Random.Range(0, 9);
            }
        
            PhotonNetwork.JoinOrCreateRoom (_roomId, options, TypedLobby.Default);
            PhotonNetwork.ConnectUsingSettings ();
        }

        private void OnClick_ButtonGrid3()
        {
            _grid = 3;
        }
        
        private void OnClick_ButtonGrid4()
        {
            _grid = 4;
        }
        
        private void OnClick_ButtonGrid5()
        {
            _grid = 5;
        }

        // If a room has been successfully created
        public override void OnCreatedRoom ()
        {
            if (_roomId.Length == 4)
            {
                ChangeView();
            }
        }

        // If a room is failed to create
        public override void OnCreateRoomFailed (short returnCode, string message) {
            Debug.Log ("Room creations failed: " + message);
        }

        private void ChangeView()
        {
            SceneManager.Instance.ChangeScene(privateMultiplayer.transform, waitingRoom.transform);

            var waitingRoomControllerScript = waitingRoom.GetComponent<WaitingRoomController>();
            waitingRoomControllerScript.roomID = _roomId;
            waitingRoomControllerScript.grid = _grid;
        }
    }
}