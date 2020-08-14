using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mechanics.Enemies
{
    public class Chicken : Enemy
    {
        private Vector2 _direction;
        [SerializeField] private float speed;
        [SerializeField] private bool initialDirection;
        [SerializeField] private float tempSpeed;
        private bool _run;
        
        
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
            if(!Physics2D.OverlapPoint(Tr.position + new Vector3(0, -1.1f, 0), LayerMask.GetMask("Obstacle"))) return;    //if falling, it does nothing
            base.FixedUpdate();
            if(!Hit) MoveDynamic(_direction, speed, .5f);
            float distance;
            if (_run) distance = 3;
            else distance = 1.5f;
            if (!Physics2D.OverlapPoint(Tr.position + new Vector3(_direction.x * distance, -1.1f, 0), LayerMask.GetMask("Obstacle"))
                || Physics2D.OverlapCircle(Tr.position + new Vector3(_direction.x, 0, 0), .1f, LayerMask.GetMask("Obstacle")))
            {
                _direction = Vector2.Reflect(_direction, Vector2.right);
                Vector3 newScale = Tr.localScale;
                newScale.x = -_direction.x;
                Tr.localScale = newScale;
                if(speed > tempSpeed)
                {
                    var t = speed;
                    speed = tempSpeed;
                    tempSpeed = t;
                    Animator.SetBool("Run",false);
                    _run = false;
                }
            }
            RaycastHit2D hit = Physics2D.Raycast(transform.position, _direction, 100, LayerMask.GetMask("PlayerPhysic","Obstacle"));
            if (hit.collider && !_run && hit.collider.gameObject.layer == LayerMask.NameToLayer("PlayerPhysic"))
            {
                var t = speed;
                speed = tempSpeed;
                tempSpeed = t;
                Animator.SetBool("Run",true);
                _run = true;
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("DamagePlayer"))
            {
                if(!Hit) Damage(other.GetComponent<IDamageInflictor>().GetDamage());
            }
        }
    }
}
