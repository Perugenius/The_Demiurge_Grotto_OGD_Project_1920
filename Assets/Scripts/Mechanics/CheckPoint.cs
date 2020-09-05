using Mechanics.Players;
using Photon.Pun;
using UnityEngine;

namespace Mechanics
{
    public class CheckPoint : MonoBehaviour
    {
        [SerializeField] private bool offlineMode = false;
        private Transform _tr;

        private void Start()
        {
            _tr = GetComponent<Transform>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!offlineMode)
            {
                PhotonView playerPhotonView = other.gameObject.GetComponent<PhotonView>();
                if(playerPhotonView!=null && !playerPhotonView.IsMine) return;
            }

            PlayableCharacter player = other.gameObject.GetComponent<PlayableCharacter>();
            if (player != null)
            {
                player.CheckPoint1 = _tr.position;
                //gameObject.SetActive(false);  //Checkpoints need always to be active, otherwise during backtracking a
                                                //player could respawn several rooms far from the point were he is dead.  
            }
        }
    }
}
