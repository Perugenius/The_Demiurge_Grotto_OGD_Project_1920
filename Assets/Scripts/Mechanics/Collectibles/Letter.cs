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

        private void Start()
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
            
            transform.localScale = new Vector3(1,1,1);
            _animator.SetBool(IsCollected, true);
            StartCoroutine(WaitBeforeDestroy());
        }

        [PunRPC]
        private void Collect()
        {
            if (type == LetterType.Teammate) _collectiblesManager.CollectTeammateLetter();
            else
            {
                _collectiblesManager.CollectEldaanLetter();
                GameManager.Instance.ExitDungeon();
            }
        }

        private IEnumerator WaitBeforeDestroy()
        {
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
        }
    }
}
