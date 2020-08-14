using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Mechanics.Enemies
{
    public class Mushroom : Movable
    {
        private Vector2 _direction;
        private Animator _animator;
        [SerializeField] private float speed;
        [SerializeField] private bool initialDirection;
        [SerializeField] private float lifePoints;
        private bool _hit;
        
        
        // Start is called before the first frame update
        void Start()
        {
            _animator = GetComponent<Animator>();
            _direction = initialDirection
                ? Vector2.left
                : Vector2.right;
            
            Vector3 scale = Tr.localScale;
            scale.x = -_direction.x;
            Tr.localScale = scale;
        }

        private void Update()
        {
            
        }
        
        protected override void FixedUpdate()
        {
            if(!Physics2D.OverlapPoint(Tr.position + new Vector3(0, -1, 0), LayerMask.GetMask("Obstacle"))) return;    //if falling, it does nothing
            base.FixedUpdate();
            if(!_hit) MoveDynamic(_direction, speed);
            if (!Physics2D.OverlapPoint(Tr.position + new Vector3(_direction.x, -1, 0), LayerMask.GetMask("Obstacle")) || Physics2D.OverlapCircle(Tr.position + new Vector3(_direction.x, 0, 0), .1f, LayerMask.GetMask("Obstacle")))
            {
                _direction = Vector2.Reflect(_direction, Vector2.right);
                Vector3 newScale = Tr.localScale;
                newScale.x = -_direction.x;
                Tr.localScale = newScale;
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("DamagePlayer"))
            {
                if(!_hit) Damage(other.GetComponent<IDamageInflictor>().GetDamage());
            }
        }
        
        private void Damage(float damage)
        {
            _animator.SetTrigger("Hit");
            _hit = true;
            if (damage < lifePoints)
            {
                lifePoints = lifePoints - damage;
                StartCoroutine (nameof(Stop));
            }
            else StartCoroutine (nameof(Die));
        }
        
        private IEnumerator Stop()
        {
            Rb.velocity = Vector2.zero;
            yield return new WaitForSeconds (_animator.GetCurrentAnimatorStateInfo(0).length);
            _hit = false;
        }

        private IEnumerator Die()
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
