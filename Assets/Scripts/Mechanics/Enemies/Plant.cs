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
        
        // Start is called before the first frame update
        void Start()
        {
            _tr = GetComponent<Transform>();
            _animator = GetComponent<Animator>();
            
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
        void Update()
        {
            RaycastHit2D left = Physics2D.Raycast(_tr.position, Vector2.left, 100, LayerMask.GetMask("PlayerPhysic","Obstacle"));
            RaycastHit2D right = Physics2D.Raycast(_tr.position, Vector2.right, 100, LayerMask.GetMask("PlayerPhysic","Obstacle"));
            if (left.collider && left.collider.gameObject.layer == LayerMask.NameToLayer("PlayerPhysic") && !_shooting)
            {
                _tr.Rotate(0f,Vector3.Angle(_direction,Vector2.left),0f);
                _direction = Vector2.left;
                _shooting = true;
                _animator.SetBool("Shooting", true);
            }
            if (right.collider && right.collider.gameObject.layer == LayerMask.NameToLayer("PlayerPhysic") && !_shooting)
            {
                _tr.Rotate(0f,Vector3.Angle(_direction,Vector2.right),0f);
                _direction = Vector2.right;
                _shooting = true;
                _animator.SetBool("Shooting", true);
            }

            if (left.collider.gameObject.layer != LayerMask.NameToLayer("PlayerPhysic") &&
                right.collider.gameObject.layer != LayerMask.NameToLayer("PlayerPhysic") && _shooting)
            {
                _shooting = false;
                _animator.SetBool("Shooting", false);
            }
            
            if (_shooting)
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

                Instantiate(bullet, firePoint.position, _tr.rotation);
                //PhotonNetwork.Instantiate("PlantBullet", firePoint.position, _tr.rotation);
            }
        }
    }
}
