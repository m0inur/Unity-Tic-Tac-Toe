using System.Collections.Generic;
using AI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace My_Photon.Rooms
{
    public class MultiplayerManager : MonoBehaviourPunCallbacks
    {
        
        private RoomInfo[] _roomsList;
        public Text searchText;

        public GameObject menu;
        public GameObject multiplayerGame;
        public GameObject singlePlayer;

        private TicTacToeCreatorAI tictactoeCreatorAIScript;

        private string _quickmatchRoomName;

        private int _roomLength;

        private float _joinAiGameCounter;
        private float _tryJoinRoomCounter;

        private bool _hasCalledSetup;
        private bool _hasPlayerJoined;
        private bool _startAiCounter;
        private bool _tryJoinRoom;
        private bool _hasCreatedRoom;
        private bool _hasFoundRoom;
        private bool _startJoinRoomCounter;
        private RoomOptions _options;
        
        private void Start()
        {
            Setup();
        }

        public void OnDisable()
        {
            _hasCalledSetup = false;
        }

        private void Setup()
        {
            Debug.Log("Setup()");
            _quickmatchRoomName = "PunfishQuickmatch2131";
            tictactoeCreatorAIScript = singlePlayer.GetComponent<TicTacToeCreatorAI>();

            // Reset Everything
            _tryJoinRoomCounter = 2f;
            _joinAiGameCounter = 2f;
            _roomLength = 0;

            _startJoinRoomCounter = false;
            _hasPlayerJoined = false;
            _startAiCounter = false;
            _hasCreatedRoom = false;
            _hasFoundRoom = false;

            searchText.text = "Searching for Opponent";
            
            _options = new RoomOptions ();
            _options.BroadcastPropsChangeToAll = true;
            _options.PublishUserId = true;
            _options.MaxPlayers = 5;
            
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }

            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
                PhotonNetwork.ConnectUsingSettings ();
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings ();
            }
            

            _hasCalledSetup = true;
        }

        private void Update()
        {
            // Setup
            if (!_hasCalledSetup)
            {
                Setup();
                _hasCalledSetup = true;
            }
            
            // if back was pressed leave room
            if (Input.GetKeyDown (KeyCode.Escape))
            {
                Leave();
            }

            // Keep trying to join a room until the counter runs out and joined lobby to access rooms
            if (_tryJoinRoomCounter > 0 && _startJoinRoomCounter)
            {
                _tryJoinRoomCounter -= Time.deltaTime;
                
                // If we are in lobby and there is a room to join
                if (PhotonNetwork.InLobby && _tryJoinRoom)
                {
                    Debug.Log("Calling QuickMatch");
                    _hasFoundRoom = true;
                    // join a random room
                    QuickMatch();
                }
            }

            // If counter is done, no room was created and no room was found
            if (_tryJoinRoomCounter <= 0 && !_hasCreatedRoom && !_hasFoundRoom)
            {
                // create room
                CreateRoom();
                _hasCreatedRoom = true;
                _startAiCounter = true;
            }

            // If start ai counter is true and no player has joined
            if (_startAiCounter && !_hasPlayerJoined)
            {
                // Countdown
                _joinAiGameCounter -= Time.deltaTime;

                // Join game after counter runs out
                if (_joinAiGameCounter < 0)
                {
                    tictactoeCreatorAIScript.isFakeMp = true;

                    gameObject.SetActive(false);
                    singlePlayer.SetActive(true);

                    if (PhotonNetwork.InRoom)
                    {
                        PhotonNetwork.CurrentRoom.IsVisible = false;
                        PhotonNetwork.CurrentRoom.IsOpen = false;
                    }
                    
                    _startAiCounter = false;
                }
            }
            else
            {
                _joinAiGameCounter = 10f;
            }
        }

        private void CreateRoom()
        {
            if (!PhotonNetwork.IsConnected) {
                Debug.Log("Returning because PhotonNetwork is not connected");
                return;
            }

            Debug.Log("Creating room");
            PhotonNetwork.JoinOrCreateRoom (_quickmatchRoomName, _options, TypedLobby.Default);
            PhotonNetwork.ConnectUsingSettings ();
            
            searchText.text = "Waiting for Opponent";
            _hasCreatedRoom = true;
        }

        // Join Random Room
        private void QuickMatch()
        {
            Debug.Log("QuickMatch()");
            PhotonNetwork.JoinRandomRoom(null, _options.MaxPlayers);
        }

        public void Leave()
        {
            if (PhotonNetwork.InRoom) {
                PhotonNetwork.LeaveRoom();
            }

            gameObject.SetActive(false);
            menu.SetActive(true);
        }

        [PunRPC]
        public void JoinGame()
        {
            gameObject.SetActive(false);
            multiplayerGame.SetActive(true);
        }
        
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            // If player joined my room play game and make current room invisible
            base.photonView.RPC("JoinGame", RpcTarget.All);
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            _hasPlayerJoined = true;
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            if (roomList.Count > 0)
            {
                _roomLength = roomList.Count;
            }

            foreach (var info in roomList)
            {
                if (info.Name != _quickmatchRoomName)
                {
                    _roomLength--;
                }
            }
            
            Debug.Log("Room length = " + roomList.Count);

            // Try to join a room if there is one
            if (_roomLength > 0)
            {
                _tryJoinRoom = true;
                Debug.Log("Try to join a room");
            }
            else
            {
                _tryJoinRoom = false;
            }
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("Created Room Successfully");
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to master");
            _startJoinRoomCounter = true;
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("Room Creation Failed");
        }
    }
}