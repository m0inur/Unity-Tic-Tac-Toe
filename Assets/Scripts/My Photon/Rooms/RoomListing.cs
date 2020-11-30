using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace My_Photon.Rooms
{
    public class RoomListing : MonoBehaviour {
        [SerializeField]
        private Text txt;

        public RoomInfo RoomInfo { get; private set; }

        public void SetRoomInfo (RoomInfo roomInfo) {
            RoomInfo = roomInfo;
            txt.text = roomInfo.Name;
        }

        public void OnClick_Button () {
            PhotonNetwork.JoinRoom (RoomInfo.Name);
        }
    }
}