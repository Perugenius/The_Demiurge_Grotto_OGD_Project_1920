using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Core
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    
        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            PhotonNetwork.LoadLevel(4);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            Debug.Log("Disconnection cause: " + cause);
            GameManager.Instance.ShowDisconnectionScreen();
            //PhotonNetwork.LeaveRoom();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            Debug.Log("Player " + otherPlayer.ActorNumber + "disconnected :(");
            GameManager.Instance.ShowDisconnectionScreen();
        }
    }
}
