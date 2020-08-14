using System.Collections;
using TMPro;
using UnityEngine;

namespace Mechanics.Enemies
{
    public class Enemy : Movable
    {
        protected Animator Animator;
        protected bool Hit;
        [SerializeField] protected float lifePoints;
        
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        
        protected void Damage(float damage)
        {
            Animator.SetTrigger("Hit");
            Hit = true;
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
            Hit = false;
        }

        protected IEnumerator Die()
        {
            Rb.velocity = Vector2.zero;
            GetComponent<Collider2D>().enabled = false;
            for (float ft = 1f; ft >= 0; ft -= 0.01f) 
            {
                Color c = GetComponent<Renderer>().material.color;
                c.a = ft;
                GetComponent<Renderer>().material.color = c;
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}
