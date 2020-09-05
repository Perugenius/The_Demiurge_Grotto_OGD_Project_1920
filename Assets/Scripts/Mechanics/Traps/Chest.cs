using System;
using System.Collections;
using Mechanics.Players;
using Photon.Pun;
using UnityEngine;

namespace Mechanics.Traps
{
    public class Chest : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private bool offlineMode = false;
        [SerializeField] private bool destroyAtTouch = false;

        private static readonly int IsHit = Animator.StringToHash("isHit");

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(destroyAtTouch)
            {
                DestroyChest("i");
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if(offlineMode)
            {
                PlayableCharacter playableCharacter= other.gameObject.GetComponent<PlayableCharacter>();
                if (playableCharacter != null && playableCharacter is Voodoo voodoo)
                {
                    if (voodoo.IsDashing)
                    {
                        DestroyChest("1");
                    }
                }
                return;
            }
            
            //PhotonView photonView = PhotonView.Get(this);
            if (other.gameObject.GetPhotonView().IsMine)
            {
                PlayableCharacter playableCharacter= other.gameObject.GetComponent<PlayableCharacter>();
                if (playableCharacter != null && playableCharacter is Voodoo voodoo)
                {
                    if (voodoo.IsDashing)
                    {

                        GetComponent<PhotonView>().RPC("DestroyChest", RpcTarget.Others, "1");
                        DestroyChest("1");
                    }
                }
            }
        }
    
        [PunRPC]
        private void DestroyChest(string i)
        {
            animator.SetBool(IsHit, true);
            Debug.Log("Chest hit");
            StartCoroutine(WaitAnimationEnd());
        }

        private IEnumerator WaitAnimationEnd()
        {
            yield return new WaitForSeconds(3f/20f);
            gameObject.transform.parent.Find("Pieces").gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    
    
    }
}
