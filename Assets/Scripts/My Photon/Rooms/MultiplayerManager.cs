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

        private float _tryJoinRoomCounter;
        
        private bool _tryJoinRoom;
        private bool _hasCreatedRoom;
        
        private void Start()
        {
            _tryJoinRoomCounter = 5f;
            _hasCreatedRoom = false;
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
                    Debug.Log("Join a random room");
                    // join a random room
                    QuickMatch();
                }
            }
            else
            {
                // If no room exists create one
                 if (!_hasCreatedRoom)
                 {
                     CreateRoom();
                     _hasCreatedRoom = true;
                 }
            }
        }

        private void CreateRoom()
        {
            Debug.Log("Creating room");
            if (!PhotonNetwork.IsConnected) {
                return;
            }
            
        
            RoomOptions options = new RoomOptions ();
            options.BroadcastPropsChangeToAll = true;
            options.PublishUserId = true;
            options.MaxPlayers = 2;
            
            PhotonNetwork.JoinOrCreateRoom (gameSettings.NickName + "'s Room", options, TypedLobby.Default);
            PhotonNetwork.ConnectUsingSettings ();
        }

        // Join Random Room
        private static void QuickMatch()
        {
            PhotonNetwork.JoinRandomRoom();
        }
        
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("Room list length = " + roomList.Count);
            // Try to join a room if there is one
            if (roomList.Count > 0)
            {
                Debug.Log("Try to join a room");
                _tryJoinRoom = true;
            }
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("Created Room Successfully");
        }
    }
}