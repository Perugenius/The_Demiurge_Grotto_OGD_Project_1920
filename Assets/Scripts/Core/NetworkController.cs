using Photon.Pun;
using UnityEngine;

namespace Core
{
    public class NetworkController : MonoBehaviourPunCallbacks
    {
        // Start is called before the first frame update
        void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to the " + PhotonNetwork.CloudRegion + " server");
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
