using System;
using System.Collections;
using Core;
using Photon.Pun;
using UnityEngine;

namespace Mechanics.Collectibles
{
    public enum LetterType
    {
        Teammate, Eldaan
    }
    public class Letter : MonoBehaviour
    {
        public LetterType type = LetterType.Teammate;
        [SerializeField] private Animator _animator;
        private static readonly int IsCollected = Animator.StringToHash("isCollected");
        private CollectiblesManager _collectiblesManager;
        private bool _isCollected = false;
        private bool _responseDelivered = false;
        private int _timeout = 100;
        [SerializeField] private bool offlineMode = false;

        private void Start()
        {
            _collectiblesManager = GameObject.Find("CollectiblesManager").GetComponent<CollectiblesManager>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (offlineMode)
            {
                if(_isCollected) return;
                _isCollected = true;
                Collect();
                return;
            }
            
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

        [PunRPC]
        private void Collect()
        {
            _isCollected = true;
            if (type == LetterType.Teammate)
            {
                _collectiblesManager.CollectTeammateLetter();
                transform.localScale = new Vector3(1,1,1);
                _animator.SetBool(IsCollected, true);
                GameManager.Instance.ShowMessage("New companion's letter found!");
                StartCoroutine(WaitBeforeDestroy());
            }
            else
            {
                _collectiblesManager.CollectEldaanLetter();
                transform.localScale = new Vector3(1,1,1);
                _animator.SetBool(IsCollected, true);
                StartCoroutine(WaitBeforeDestroy());
                if(offlineMode) GameObject.Find("TutorialManager").GetComponent<TutorialManager>().ShowVictoryScreen();
                else GameManager.Instance.ShowVictoryScreen();
            }
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

        private IEnumerator WaitBeforeDestroy()
        {
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
        }
    }
}
