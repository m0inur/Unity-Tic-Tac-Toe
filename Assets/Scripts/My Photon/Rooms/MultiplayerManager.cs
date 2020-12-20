using System.Collections.Generic;
using AI;
using ExitGames.Client.Photon;
using Multiplayer_Game;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace My_Photon.Rooms
{
    public class MultiplayerManager : MonoBehaviourPunCallbacks
    {
        
        private RoomInfo[] _roomsList;
        public Text searchText;

        public GameObject multiplayerController;
        public GameObject multiplayerGame;
        public GameObject singlePlayer;

        private TicTacToeCreatorAI _tictactoeCreatorAIScript;

        private string _roomName;

        public int grid;

        private float _joinAiGameCounter;
        private float _tryJoinRoomCounter;

        private bool _hasConnectedToMaster;
        private bool _hasCalledSetup;
        private bool _hasPlayerJoined;
        private bool _startAiCounter;
        private bool _tryJoinRoom;
        private bool _hasCreatedRoom;
        private bool _hasFoundRoom;
        private bool _startJoinRoomCounter;
        private bool _hasJoinedRoom;
        
        private RoomOptions _randomRoomOption;

        private void Start()
        {
            Setup();
        }

        private void OnDisable()
        {
            if (_hasJoinedRoom)
            {
                // If a room has been joined disable the current scene
                multiplayerController.SetActive(false);
            }
            _hasCalledSetup = false;
        }

        private void Setup()
        {
            _tictactoeCreatorAIScript = singlePlayer.GetComponent<TicTacToeCreatorAI>();

            // Reset Everything
            _tryJoinRoomCounter = 3f;
            _joinAiGameCounter = 10f;

            _hasConnectedToMaster = false;
            _startJoinRoomCounter = false;
            _hasPlayerJoined = false;
            _startAiCounter = false;
            _hasCreatedRoom = false;
            _hasFoundRoom = false;
            _hasJoinedRoom = false;
            
            searchText.text = "Searching for Opponent";
            
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

            if (_hasConnectedToMaster && !_startJoinRoomCounter)
            {
                if (PhotonNetwork.InLobby)
                {
                    _startJoinRoomCounter = true;
                }
            }
            
            // Keep trying to join a room until the counter runs out and joined lobby to access rooms
            if (_tryJoinRoomCounter > 0 && _startJoinRoomCounter)
            {
                _tryJoinRoomCounter -= Time.deltaTime;
                
                if (PhotonNetwork.IsConnectedAndReady)
                {
                    // join a random filtered room
                    QuickMatch();
                }
            }

            // If counter is done, no room was created and no room was found
            if (_tryJoinRoomCounter <= 0 && !_hasCreatedRoom && !_hasFoundRoom && PhotonNetwork.IsConnectedAndReady)
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
                    _tictactoeCreatorAIScript.isFakeMp = true;
                    Debug.Log("Joining fake multiplayer");
                    
                    SceneManager.Instance.ChangeScene(multiplayerController.transform, singlePlayer.transform);

                    if (PhotonNetwork.InRoom)
                    {
                        PhotonNetwork.CurrentRoom.IsVisible = false;
                        PhotonNetwork.CurrentRoom.IsOpen = false;
                        PhotonNetwork.LeaveRoom();
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
            for (var i = 0; i < 6; i++)
            {
                _roomName += UnityEngine.Random.Range(0, 9);
            }
            
            Debug.Log("Creating room: " + _roomName);
            
            var options = new RoomOptions
            {
                BroadcastPropsChangeToAll = true, PublishUserId = true, MaxPlayers = System.Convert.ToByte(grid)
            };
            
            // Vary max players on different grids to join correct room with same grid option
            PhotonNetwork.JoinOrCreateRoom(_roomName, options, TypedLobby.Default);
            PhotonNetwork.ConnectUsingSettings();
            
            searchText.text = "Waiting for Opponent";
            _hasCreatedRoom = true;
        }

        // Join Random Room
        private void QuickMatch()
        {
            // Joins random room with same grid option
            PhotonNetwork.JoinRandomRoom(null, System.Convert.ToByte(grid), MatchmakingMode.FillRoom, null, null);
        }

        [PunRPC]
        public void JoinGame()
        {
            multiplayerGame.GetComponent<TicTacToeCreatorMp>().grid = grid;
            SceneManager.Instance.ChangeScene(multiplayerController.transform, multiplayerGame.transform);
            _hasJoinedRoom = true;
        }
        
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            // Remove the current room so no other player can join
            PhotonNetwork.CurrentRoom.RemovedFromList = true;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            
            // If player joined my room play game
            base.photonView.RPC("JoinGame", RpcTarget.All);
            _hasPlayerJoined = true;
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("Created Room Successfully");
        }
        
        public override void OnJoinedRoom()
        {
            _hasFoundRoom = true;
            Debug.Log("Joined a room");
        }
        
        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to master");
            _hasConnectedToMaster = true;
        }
        
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("Failed Joining a room");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("Room Creation Failed");
        }
    }
}