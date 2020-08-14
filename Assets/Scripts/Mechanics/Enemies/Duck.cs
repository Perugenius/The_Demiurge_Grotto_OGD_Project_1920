using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Mechanics.Enemies
{
    
    public class Duck : Enemy
    {
        private Vector2 _direction;
        [SerializeField] private bool initialDirection;
        [SerializeField] private float height;
        [SerializeField] private float thrust;
        private bool _jumping;
        private bool _ready;

        // Start is called before the first frame update
        void Start()
        {
            _ready = true;
            Animator = GetComponent<Animator>();
            _direction = initialDirection
                ? Vector2.left
                : Vector2.right;
            
            Vector3 scale = Tr.localScale;
            scale.x = -_direction.x;
            Tr.localScale = scale;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        protected override void FixedUpdate()
        {
            if (!_jumping && _ready && !Hit)
            {
                //Rb.AddForce(new Vector2(_direction.x * 3, height),ForceMode2D.Impulse);
                JumpLateral(height, _direction * thrust);
                //Rb.velocity = new Vector2(_direction.x * .3f, height);
                _jumping = true;
                Animator.SetBool("Jumping", true);
            }
            
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, LayerMask.GetMask("Obstacle"));
            RaycastHit2D hitLeft = Physics2D.Raycast(transform.position + .5f * Vector3.left, Vector2.down, 1.5f, LayerMask.GetMask("Obstacle"));
            RaycastHit2D hitRight = Physics2D.Raycast(transform.position - .5f * Vector3.left, Vector2.down, 1.5f, LayerMask.GetMask("Obstacle"));
            if ((hit || hitLeft || hitRight) && _ready && Rb.velocity.y < 0)
            {
                _ready = false;
                _jumping = false;
                Animator.SetBool("Jumping", false);
                StartCoroutine (nameof(Cooldown));
            }
            
            if (Physics2D.OverlapPoint(Tr.position + new Vector3(_direction.x, 0, 0), LayerMask.GetMask("Obstacle")))
            {
                _direction = Vector2.Reflect(_direction, Vector2.right);
                Rb.velocity = Vector2.Reflect(Rb.velocity, Vector2.right);
                Vector3 newScale = Tr.localScale;
                newScale.x = -_direction.x;
                Tr.localScale = newScale;
            }
        }

        private IEnumerator Cooldown(){
            yield return new WaitForSeconds (1);
            _ready = true;
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
