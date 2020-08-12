﻿using System.Collections;
using System.IO;
using Photon.Pun;
using UnityEngine;

namespace Mechanics.Enemies
{
    public class Plant : MonoBehaviour
    {
        private Vector2 _direction;
        private Transform _tr;
        private bool _shooting;
        private Animator _animator;
        [SerializeField] private Transform firePoint;
        [SerializeField] private GameObject bullet;
        [SerializeField] private float lifePoints;
        private bool _bulletReady;
        private bool _waiting;
        private bool _ending;
        [SerializeField] private bool testing;
        private bool _hit;
        
        // Start is called before the first frame update
        void Start()
        {
            _tr = GetComponent<Transform>();
            _animator = GetComponent<Animator>();

            _bulletReady = true;
            
            RaycastHit2D left = Physics2D.Raycast(_tr.position, Vector2.left,100, LayerMask.GetMask("PlayerPhysic","Obstacle"));
            RaycastHit2D right = Physics2D.Raycast(_tr.position, Vector2.right, 100, LayerMask.GetMask("PlayerPhysic","Obstacle"));
            if (left.distance > right.distance)
            {
                _direction = Vector2.left;
            }
            else
            {
                _direction = Vector2.right;
                _tr.Rotate(0f,180,0f);
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            RaycastHit2D left = Physics2D.Raycast(_tr.position, Vector2.left, 100, LayerMask.GetMask("PlayerPhysic","Obstacle"));
            RaycastHit2D right = Physics2D.Raycast(_tr.position, Vector2.right, 100, LayerMask.GetMask("PlayerPhysic","Obstacle"));
            if (left.collider && left.collider.gameObject.layer == LayerMask.NameToLayer("PlayerPhysic") && !_shooting && !_waiting)
            {
                _tr.Rotate(0f,Vector3.Angle(_direction,Vector2.left),0f);
                _direction = Vector2.left;
                //_shooting = true;
                _animator.SetBool("Shooting", true);
                StartCoroutine (nameof(LoadingBullet));
            }
            if (right.collider && right.collider.gameObject.layer == LayerMask.NameToLayer("PlayerPhysic") && !_shooting && !_waiting)
            {
                _tr.Rotate(0f,Vector3.Angle(_direction,Vector2.right),0f);
                _direction = Vector2.right;
                //_shooting = true;
                _animator.SetBool("Shooting", true);
                StartCoroutine (nameof(LoadingBullet));
            }

            if (left.collider.gameObject.layer != LayerMask.NameToLayer("PlayerPhysic") &&
                right.collider.gameObject.layer != LayerMask.NameToLayer("PlayerPhysic") && _shooting)
            {
                _shooting = false;
                _ending = true;
                _animator.SetBool("Shooting", false);
            }
            
            if (_shooting || _ending)
            {
                if (_direction == Vector2.left && left.collider.gameObject.layer != LayerMask.NameToLayer("PlayerPhysic"))
                {
                    _tr.Rotate(0f,Vector3.Angle(_direction,Vector2.right),0f);
                    _direction = Vector2.right;
                }
                
                if (_direction == Vector2.right && right.collider.gameObject.layer != LayerMask.NameToLayer("PlayerPhysic"))
                {
                    _tr.Rotate(0f,Vector3.Angle(_direction,Vector2.left),0f);
                    _direction = Vector2.left;
                }

                if (_bulletReady && !_hit)
                {
                    if(testing) Instantiate(bullet, firePoint.position, _tr.rotation);
                    else
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            string path = Path.Combine("Enemies","PlantBullet");
                            PhotonNetwork.Instantiate(path, firePoint.position, _tr.rotation);
                        }
                    }
                    _bulletReady = false;
                    _ending = false;
                    StartCoroutine (nameof(Cooldown));
                }
                
            }
        }

        private IEnumerator Cooldown(){
            yield return new WaitForSeconds (_animator.GetCurrentAnimatorStateInfo(0).length);
            _bulletReady = true;
        }
        
        private IEnumerator LoadingBullet()
        {
            _waiting = true;
            //Debug.Log(_animator.GetCurrentAnimatorStateInfo(0).length/2f);
            yield return new WaitForSeconds (_animator.GetCurrentAnimatorStateInfo(0).length/2f);
            _waiting = false;
            _shooting = true;
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
            yield return new WaitForSeconds (_animator.GetCurrentAnimatorStateInfo(0).length);
            _hit = false;
        }

        private IEnumerator Die(){
            yield return new WaitForSeconds (_animator.GetCurrentAnimatorStateInfo(0).length);
            Destroy(gameObject);
        }
    }
}
