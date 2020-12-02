using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace My_Photon.Rooms
{
    public class MultiplayerManager : MonoBehaviourPunCallbacks
    {
        public GameSettings gameSettings;
        private RoomInfo[] _roomsList;
        public int roomLength;

        private float _tryJoinRoomCounter;
        
        private bool _tryJoinRoom;
        private bool _hasCreatedRoom;

        private void Start()
        {
            _tryJoinRoomCounter = 5f;
            _hasCreatedRoom = false;
            
            if (roomLength > 0)
            {
                Debug.Log("Try to join a room");
                _tryJoinRoom = true;
            } 
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
                    Debug.Log("Joining a random room");
                    // join a random room
                    QuickMatch();
                }
            }

            if (_tryJoinRoomCounter <= 0 && !_hasCreatedRoom)
            {
                Debug.Log("No room found");
                // If no room exists create one
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
        }

        // Join Random Room
        private static void QuickMatch()
        {
            PhotonNetwork.JoinRandomRoom();
        }
        
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
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

        public override void OnCreatedRoom()
        {
            Debug.Log("Created Room Successfully");
        }
    }
}