using System;
using System.Collections;
using Core;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Mechanics.Enemies
{
    public class Enemy : Movable
    {
        protected Animator Animator;
        protected bool Hit;
        [SerializeField] protected float lifePoints;
        public GameObject damagePlayer;
        [SerializeField] protected bool offline;
        
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        
        [PunRPC]
        public void Damage(float damage)
        {
            Animator.SetTrigger("Hit");
            Hit = true;
            damagePlayer.SetActive(false);
            if (damage < lifePoints)
            {
                lifePoints = lifePoints - damage;
                StartCoroutine (nameof(Stop));
            }
            else StartCoroutine (nameof(Die));
        }

        protected IEnumerator Stop()
        {
            Rb.velocity = Vector2.zero;
            yield return new WaitForSeconds (Animator.GetCurrentAnimatorStateInfo(0).length);
            damagePlayer.SetActive(true);
            Hit = false;
        }

        protected IEnumerator Die()
        {
            Rb.velocity = Vector2.zero;
            foreach (Collider2D enemycollider in GetComponents<Collider2D>())
            {
                enemycollider.enabled = false;
            }
            for (float ft = 1f; ft >= 0; ft -= 0.03f) 
            {
                Color c = GetComponent<Renderer>().material.color;
                c.a = ft;
                GetComponent<Renderer>().material.color = c;
                yield return null;
            }
            if(!offline && PhotonNetwork.IsMasterClient) PhotonNetwork.Destroy(gameObject);
            else if(offline) Destroy(gameObject);
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("DamagePlayer"))
            {
                if(offline) Damage(other.GetComponent<IDamageInflictor>().GetDamage());
                else
                {
                    PhotonView photonView = other.gameObject.GetComponent<PhotonView>();
                    if (photonView == null) photonView = other.transform.GetComponentInParent<PhotonView>();
                    if (!Hit && photonView.IsMine){
                        AudioManager.Instance.PlaySound("EnemyHit");
                        GetComponent<PhotonView>().RPC("Damage", RpcTarget.All, other.GetComponent<IDamageInflictor>().GetDamage());
                        
                    }
                }
                
            }
        }
    }
}
