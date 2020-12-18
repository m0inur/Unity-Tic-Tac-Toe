using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace My_Photon
{
    public class TestConnect : MonoBehaviourPunCallbacks {
        // Start is called before the first frame update
        private void Start () {
            Debug.Log ("Connecting to Photon", this);
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.NickName = MasterManager.GameSettings.NickName;
            PhotonNetwork.GameVersion = MasterManager.GameSettings.GameVersion;
            Connect();
        }

        private void Connect()
        {
            PhotonNetwork.ConnectUsingSettings ();
        }

        public override void OnConnectedToMaster () {
            if (!PhotonNetwork.InLobby) {
                PhotonNetwork.JoinLobby ();
            }

            Debug.Log ("Connected to Photon and joined lobby", this);
        }

        public override void OnDisconnected (DisconnectCause cause) {
            // If player did not disconnect by quitting app then reconnect
            if (cause.ToString() != "DisconnectedByClientLogic")
            {
                
            }
            Debug.Log("Disconnected to photon because: " + cause.ToString());
        }
    }
}