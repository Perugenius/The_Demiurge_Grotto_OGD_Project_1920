using Photon.Pun;
using UnityEngine;

namespace Mechanics
{
    public class CheckPoint : MonoBehaviour
    {
        private Transform _tr;

        private void Start()
        {
            _tr = GetComponent<Transform>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PhotonView playerPhotonView = other.gameObject.GetComponent<PhotonView>();
            if(playerPhotonView!=null && !playerPhotonView.IsMine) return;
        
            //TODO
            /*Player player = other.gameObject.GetComponent<Player>()*/;
            /*if (player != null)
        {
            player.CheckPoint(_tr.position);
            gameObject.SetActive(false);
        }*/
        }
    }
}
