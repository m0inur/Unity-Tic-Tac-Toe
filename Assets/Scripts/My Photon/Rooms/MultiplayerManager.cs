using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace My_Photon.Rooms
{
    public class MultiplayerManager : MonoBehaviourPunCallbacks
    {
        public GameObject multiplayerGame;
        public GameSettings gameSettings;
        private RoomInfo[] _roomsList;
        public int roomLength;

        private float _tryJoinRoomCounter;
        
        private bool _tryJoinRoom;
        private bool _hasCreatedRoom;
        private bool _hasFoundRoom;

        private void Start()
        {
            _tryJoinRoomCounter = 5f;
            _hasCreatedRoom = false;
            
            if (roomLength > 0)
            {
                _tryJoinRoom = true;
            } 
            Debug.Log("Room length = " + roomLength);
            base.photonView.RPC("JoinGame", RpcTarget.All);
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
            }
        }

        private void CreateRoom()
        {
            if (!PhotonNetwork.IsConnected) {
                return;
            }

            RoomOptions options = new RoomOptions ();
            options.BroadcastPropsChangeToAll = true;
            options.PublishUserId = true;
            options.MaxPlayers = 2;
            
            PhotonNetwork.JoinOrCreateRoom (gameSettings.NickName + "'s room", options, TypedLobby.Default);
            PhotonNetwork.ConnectUsingSettings ();
            _hasCreatedRoom = true;
        }

        // Join Random Room
        private void QuickMatch()
        {
            PhotonNetwork.JoinRandomRoom();
            Debug.Log("Joined a room");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            // If player joined my room play game
            base.photonView.RPC("JoinGame", RpcTarget.All);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("Room length = " + roomLength);
            
            if (roomList.Count > 0)
            {
                roomLength = roomList.Count;
            }
            
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
        
        
        [PunRPC]
        public void JoinGame()
        {
            Debug.Log("Join Game");
            gameObject.SetActive(false);
            multiplayerGame.SetActive(true);
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("Created Room Successfully");
        }
    }
}