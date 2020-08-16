using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Mechanics.Enemies
{
    public class BlueBird : Enemy
    {
        private Vector2 _direction;
        [SerializeField] private float speed;
        [SerializeField] private bool initialDirection;
        
        
        // Start is called before the first frame update
        void Start()
        {
            Animator = GetComponent<Animator>();
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
            base.FixedUpdate();
            if(!Hit) MoveDynamic(_direction, speed);
            if (Physics2D.OverlapCircle(Tr.position + new Vector3(_direction.x, 0, 0), .1f, LayerMask.GetMask("Obstacle")))
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
                if(!Hit && other.gameObject.GetComponent<PhotonView>().IsMine) GetComponent<PhotonView>().RPC("Damage", RpcTarget.All, other.GetComponent<IDamageInflictor>().GetDamage());
            }
        }
    }
}
