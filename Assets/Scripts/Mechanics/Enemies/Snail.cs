using System.Collections;
using UnityEngine;

namespace Mechanics.Enemies
{
    public class Snail : Enemy
    {
        private Vector2 _direction;
        [SerializeField] private float speed;
        [SerializeField] private bool initialDirection;
        private bool _withdrawn;
        //private bool _hit;
        
        
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
            if(!Physics2D.OverlapPoint(Tr.position + new Vector3(0, -1, 0), LayerMask.GetMask("Obstacle"))) return;    //if falling, it does nothing
            base.FixedUpdate();
            if(!_withdrawn) MoveDynamic(_direction, speed);
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
                if(!_withdrawn) Damage(other.GetComponent<IDamageInflictor>().GetDamage());
            }
        }

        private void Withdraw()
        {
            Rb.velocity = Vector2.zero;
            _withdrawn = true;
            Animator.SetBool("Withdraw",true);
            StartCoroutine (nameof(Waiting));
        }
        
        private IEnumerator Waiting(){
            yield return new WaitForSeconds (3);
            _withdrawn = false;
            Animator.SetBool("Withdraw",false);
        }
        
        private void Damage(float damage)
        {
            Animator.SetTrigger("Hit");
            //_hit = true;
            if (damage < lifePoints)
            {
                lifePoints = lifePoints - damage;
                Withdraw();
            }
            else StartCoroutine (nameof(Die));
        }
        
        /*private IEnumerator Stop()
        {
            yield return new WaitForSeconds (_animator.GetCurrentAnimatorStateInfo(0).length);
            _hit = false;
        }*/

        private IEnumerator Die(){
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
