using System;
using System.Collections.Generic;
using AI;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace My_Photon.Rooms
{
    public class MultiplayerManager : MonoBehaviourPunCallbacks
    {
        
        private RoomInfo[] _roomsList;
        public GameSettings gameSettings;

        public GameObject menu;
        public GameObject multiplayerGame;
        public GameObject singlePlayer;

        private TicTacToeCreatorAI tictactoeCreatorAIScript;

        private string _quickmatchRoomName;

        public int roomLength;

        private float _joinAiGameCounter;
        private float _tryJoinRoomCounter;

        private bool _hasPlayerJoined;
        private bool _startAiCounter;
        private bool _tryJoinRoom;
        private bool _hasCreatedRoom;
        private bool _hasFoundRoom;
        private RoomOptions _options;
        
        private void OnEnable()
        {
            _quickmatchRoomName = "PunfishQuickmatch2131";

            _tryJoinRoomCounter = 5f;
            _joinAiGameCounter = 10f;
            
            _startAiCounter = false;
            _hasCreatedRoom = false;
            _hasPlayerJoined = false;
            
            if (roomLength > 0)
            {
                _tryJoinRoom = true;
            } 
            
            tictactoeCreatorAIScript = singlePlayer.GetComponent<TicTacToeCreatorAI>();

            _options = new RoomOptions ();
            _options.BroadcastPropsChangeToAll = true;
            _options.PublishUserId = true;
            _options.IsVisible = false;
            _options.MaxPlayers = 5;
            
            base.photonView.RPC(_quickmatchRoomName, RpcTarget.All);
        }
        
        private void Update()
        {
            // Keep trying to join a room until the counter runs out and joined lobby to access rooms
            if (_tryJoinRoomCounter > 0)
            {
                _tryJoinRoomCounter -= Time.deltaTime;
                
                // If we are in lobby and there is a room to join
                if (PhotonNetwork.InLobby && _tryJoinRoom)
                {
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
                Debug.Log("Started Ai Counter");
            }

            // If start ai counter is true and no player has joined
            if (_startAiCounter && !_hasPlayerJoined)
            {
                // Countdown
                _joinAiGameCounter -= Time.deltaTime;

                // Join game after counter runs out
                if (_joinAiGameCounter < 0)
                {
                    Debug.Log("Joining Bot Game");
                    gameObject.SetActive(false);
                    singlePlayer.SetActive(true);
                    tictactoeCreatorAIScript.mode = 3;
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
                return;
            }

            PhotonNetwork.JoinOrCreateRoom (_quickmatchRoomName, _options, TypedLobby.Default);
            PhotonNetwork.ConnectUsingSettings ();
            _hasCreatedRoom = true;
        }

        // Join Random Room
        private void QuickMatch()
        {
            // PhotonNetwork.JoinRandomRoom();
            PhotonNetwork.JoinRandomRoom(null, _options.MaxPlayers);
        }

        public void Leave()
        {
            if (_hasCreatedRoom)
            {
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
            _hasPlayerJoined = true;
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("Room length = " + roomList.Count);
            
            if (roomList.Count > 0)
            {
                roomLength = roomList.Count;
            }

            foreach (var info in roomList)
            {
                if (info.Name != _quickmatchRoomName)
                {
                    roomLength--;
                }
            }
            
            Debug.Log("Room length = " + roomList.Count);

            // Try to join a room if there is one
            if (roomLength > 0)
            {
                _tryJoinRoom = true;
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
    }
}