﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mechanics.Enemies
{
    public class Slime : Movable
    {
        private Vector2 _direction;
        private Animator _animator;
        [SerializeField] private float speed;
        [SerializeField] private bool initialDirection;
        [SerializeField] private Transform firePoint;
        [SerializeField] private List<GameObject> particle;
        [SerializeField] private float lifePoints;
        [SerializeField] private bool testing;
        private bool _particleReady;


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

            _particleReady = true;
        }

        private void Update()
        {
            
        }
        
        protected override void FixedUpdate()
        {
            if(!Physics2D.OverlapPoint(Tr.position + new Vector3(0, -1, 0), LayerMask.GetMask("Obstacle"))) return;    //if falling, it does nothing
            base.FixedUpdate();
            MoveDynamic(_direction, speed, .5f);
            if (!Physics2D.OverlapPoint(Tr.position + new Vector3(_direction.x * 2f, -1, 0), LayerMask.GetMask("Obstacle")) || Physics2D.OverlapCircle(Tr.position + new Vector3(_direction.x, 0, 0), .1f, LayerMask.GetMask("Obstacle")))
            {
                _direction = Vector2.Reflect(_direction, Vector2.right);
                Vector3 newScale = Tr.localScale;
                newScale.x = -_direction.x;
                Tr.localScale = newScale;
            }

            if (_particleReady)
            {
                if(testing) Instantiate(particle[Random.Range(0,4)], firePoint.position, Tr.rotation);
                else
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        string path = Path.Combine("Enemies","Particle" + Random.Range(1,5));
                        PhotonNetwork.Instantiate(path, firePoint.position, Tr.rotation);
                    }
                    
                }
                _particleReady = false;
                StartCoroutine (nameof(Cooldown));
            }
            
        }
        
        private IEnumerator Cooldown(){
            yield return new WaitForSeconds (.4f);
            _particleReady = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("DamagePlayer"))
            {
                Damage(other.GetComponent<IDamageInflictor>().GetDamage());
            }
        }

        private void Damage(float damage)
        {
            _animator.SetTrigger("Hit");
            if (damage < lifePoints)
            {
                lifePoints = lifePoints - damage;
            }
            else StartCoroutine (nameof(Die));
        }

        private IEnumerator Die(){
            yield return new WaitForSeconds (_animator.GetCurrentAnimatorStateInfo(0).length);
            Destroy(gameObject);
        }
    }
}
