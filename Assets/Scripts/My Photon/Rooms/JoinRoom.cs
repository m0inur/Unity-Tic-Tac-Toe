﻿using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace My_Photon.Rooms
{
    public class JoinRoom : MonoBehaviourPunCallbacks
    {
        public GameObject privateMultiplayer;
        public GameObject waitingRoom;
        
        public Text roomID;
        public Text roomErrorTxt;

        private float _disableErrorTextWait;

        public void OnClick_JoinRoom() {
            if (!PhotonNetwork.IsConnected) {
                Debug.Log("You are not connected");
                PhotonNetwork.Reconnect();
                // return;
            } 
            
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }

            Debug.Log("Joining room = " + roomID.text);
            PhotonNetwork.JoinRoom(roomID.text);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Joined room");
            roomID.text = "";
            privateMultiplayer.SetActive(false);
            waitingRoom.SetActive(true);
            waitingRoom.GetComponent<WaitingRoomController>().roomID = roomID.text;
        }
        
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            roomErrorTxt.gameObject.SetActive(true);
            StartCoroutine(DisableErrorText());
        }

        private IEnumerator DisableErrorText()
        {
            _disableErrorTextWait = 5f;
            yield return new WaitForSeconds(_disableErrorTextWait);
            roomErrorTxt.gameObject.SetActive(false);
        }
    }
}