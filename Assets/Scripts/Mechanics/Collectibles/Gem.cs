using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Mechanics.Collectibles
{
    public class Gem : MonoBehaviour
    {
        public int value = 1;
        [SerializeField] private Animator _animator;
        private static readonly int IsCollected = Animator.StringToHash("isCollected");
        private CollectiblesManager _collectiblesManager;

        // Start is called before the first frame update
        void Start()
        {
            _collectiblesManager = GameObject.Find("CollectiblesManager").GetComponent<CollectiblesManager>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PhotonView photonView = other.gameObject.GetPhotonView();
            photonView = (photonView == null) ? other.transform.parent.gameObject.GetPhotonView() : photonView;
            if (!photonView.IsMine) return;
            
            //collect for others players
            transform.parent.gameObject.GetComponent<PhotonView>().RPC("Collect", RpcTarget.Others);
            
            //collect for local player
            Collect();
            
            transform.localScale = new Vector3(1,1,1);
            _animator.SetBool(IsCollected, true);
            StartCoroutine(WaitBeforeDestroy());
        }

        private IEnumerator WaitBeforeDestroy()
        {
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
        }

        [PunRPC]
        public void Collect()
        {
            _collectiblesManager.CollectGem(value);
        }
    }
}
