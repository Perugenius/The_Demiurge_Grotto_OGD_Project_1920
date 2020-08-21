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
        private bool _isCollected = false;

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
            
            if(_isCollected) return;
            _isCollected = true;

            //collect for others players
            GetComponent<PhotonView>().RPC("Collect", RpcTarget.Others);
            
            
            //collect for local player
            Collect();
        }

        private IEnumerator WaitBeforeDestroy()
        {
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
        }

        [PunRPC]
        public void Collect()
        {
            Debug.Log("Gem " + value + " collected");
            _collectiblesManager.CollectGem(value);
            transform.localScale = new Vector3(1,1,1);
            _animator.SetBool(IsCollected, true);
            StartCoroutine(WaitBeforeDestroy());
        }
    }
}
