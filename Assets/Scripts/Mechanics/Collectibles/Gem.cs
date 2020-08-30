using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Mechanics.Collectibles
{
    public class Gem : MonoBehaviour
    {
        public int value = 1;
        [SerializeField] private Animator _animator;
        [SerializeField] private Text _text;
        private static readonly int IsCollected = Animator.StringToHash("isCollected");
        private CollectiblesManager _collectiblesManager;
        private bool _isCollected = false;
        private SpriteRenderer _spriteRenderer;
        private bool _responseDelivered = false;
        private int _timeout = 100;

        // Start is called before the first frame update
        void Start()
        {
            _collectiblesManager = GameObject.Find("CollectiblesManager").GetComponent<CollectiblesManager>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PhotonView photonView = other.gameObject.GetPhotonView();
            photonView = (photonView == null) ? other.transform.parent.gameObject.GetPhotonView() : photonView;
            if (!photonView.IsMine) return;
            
            if (PhotonNetwork.IsMasterClient)
            {
                if(_isCollected) return;
                _isCollected = true;

                //collect for others players
                GetComponent<PhotonView>().RPC("Collect", RpcTarget.Others);
            
                //collect for local player
                Collect();
            }
            else
            {
                GetComponent<PhotonView>().RPC("AlreadyCollected", RpcTarget.MasterClient);
                StartCoroutine(WaitAnswer());
            }
        }
        
        private IEnumerator WaitAnswer()
        {
            if (!_responseDelivered && _timeout>0)
            {
                _timeout--; 
                yield return new WaitForEndOfFrame();
            }

            if (!_isCollected && _timeout>0)
            {
                _isCollected = true;

                //collect for others players
                GetComponent<PhotonView>().RPC("Collect", RpcTarget.Others);
            
                //collect for local player
                Collect();
            }
        }

        private IEnumerator WaitBeforeLogQuantity()
        {
            yield return new WaitForSeconds(0.5f);
            _spriteRenderer.enabled = false;
            LogQuantity();
        }

        private void LogQuantity()
        {
            _text.text = value.ToString();
            StartCoroutine(WaitBeforeDestroy());
        }

        private IEnumerator WaitBeforeDestroy()
        {
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(DestroyUiQuantity());
        }

        private IEnumerator DestroyUiQuantity()
        {
            while (_text.gameObject.transform.localScale.x > 0.001f)
            {
                _text.gameObject.transform.localScale -= new Vector3(0.001f,0.001f,0.001f);
                yield return new  WaitForEndOfFrame();
            }
            Destroy(gameObject.transform.parent.gameObject);
        }

        [PunRPC]
        public void Collect()
        {
            _isCollected = true;
            Debug.Log("Gem " + value + " collected");
            _collectiblesManager.CollectGem(value);
            transform.localScale = new Vector3(1,1,1);
            _animator.SetBool(IsCollected, true);
            StartCoroutine(WaitBeforeLogQuantity());
        }
        
        [PunRPC]
        public void AlreadyCollected()
        {
            GetComponent<PhotonView>()
                .RPC(_isCollected ? "AlreadyCollectedTrueResponse" : "AlreadyCollectedFalseResponse", RpcTarget.Others);
        }

        [PunRPC]
        public void AlreadyCollectedTrueResponse()
        {
            _responseDelivered = true;
            _isCollected = true;
        }

        [PunRPC]
        public void AlreadyCollectedFalseResponse()
        {
            _responseDelivered = true;
            _isCollected = false;
        }
    }
}
