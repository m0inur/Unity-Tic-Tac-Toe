using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace My_Photon.Rooms
{
    public class RoomListingsMenu : MonoBehaviourPunCallbacks {
        [SerializeField]
        private Transform content;
        [SerializeField]
        private RoomListing roomListing;

        private List<RoomListing> _listings = new List<RoomListing> ();

        public override void OnJoinedRoom () {
            content.DestroyChildren ();
            _listings.Clear ();
        }

        // Need a SingletonRefrence to use "OnRoomListUpdate"
        public override void OnRoomListUpdate (List<RoomInfo> roomList) {
            foreach (var info in roomList)
            {
                if (info.Name != "PunfishQuickmatch2131")
                {
                    // Remove from list
                    if (info.RemovedFromList) {
                        int index = _listings.FindIndex (x => x.RoomInfo.Name == info.Name);

                        if (index != -1) {
                            Destroy (_listings[index].gameObject);
                            _listings.RemoveAt (index);
                        }
                    } else {
                        int index = _listings.FindIndex (x => x.RoomInfo.Name == info.Name);

                        if (index == -1) {
                            RoomListing listing = (RoomListing) Instantiate (roomListing, content);
                            if (listing != null) {
                                listing.SetRoomInfo (info);
                                _listings.Add (listing);
                            }
                        }
                    }
                }
            }
        }
    }
}