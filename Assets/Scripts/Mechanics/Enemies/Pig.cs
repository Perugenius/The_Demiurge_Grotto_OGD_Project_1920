using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Mechanics.Enemies
{
    public class Pig : Enemy
    {
        private Vector2 _direction;
        [SerializeField] private float speed;
        [SerializeField] private bool initialDirection;
        [SerializeField] private float tempSpeed;
        private bool _run;
        private Transform _player;
        private bool _following;
        
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

        // Update is called once per frame
        protected override void FixedUpdate()
        {
            if(!Physics2D.OverlapPoint(Tr.position + new Vector3(0, -1.1f, 0), LayerMask.GetMask("Obstacle"))) return;    //if falling, it does nothing
            base.FixedUpdate();
            if ((_following || _run) && Mathf.Abs(_player.position.x - Tr.position.x) > 10f)
            {
                _following = false;
                if(_run)
                {
                    var t = speed;
                    speed = tempSpeed;
                    tempSpeed = t;
                    Animator.SetBool("Run",false);
                    _run = false;
                }
            }
            if (_following && _direction.x != Mathf.Sign(_player.position.x - Tr.position.x))
            {
                _direction = new Vector2(Mathf.Sign(_player.position.x - Tr.position.x), 0);
                Vector3 newScale = Tr.localScale;
                newScale.x = -_direction.x;
                Tr.localScale = newScale;
            }
            if(!Hit) MoveDynamic(_direction, speed);
            if (!_following && (!Physics2D.OverlapPoint(Tr.position + new Vector3(_direction.x * 1.5f, -1.1f, 0), LayerMask.GetMask("Obstacle"))
                || Physics2D.OverlapCircle(Tr.position + new Vector3(_direction.x, 0, 0), .1f, LayerMask.GetMask("Obstacle"))))
            {
                _direction = Vector2.Reflect(_direction, Vector2.right);
                Vector3 newScale = Tr.localScale;
                newScale.x = -_direction.x;
                Tr.localScale = newScale;
                /*if(speed > tempSpeed)
                {
                    var t = speed;
                    speed = tempSpeed;
                    tempSpeed = t;
                    _animator.SetBool("Run",false);
                    run = false;
                }*/
                RaycastHit2D hit = Physics2D.Raycast(transform.position, _direction, 20, LayerMask.GetMask("PlayerPhysic","Obstacle"));
                if (hit.collider && hit.collider.gameObject.layer == LayerMask.NameToLayer("PlayerPhysic"))
                {
                    _player = hit.transform;
                    _following = true;
                }
            }
        }
        

        private void Anger()
        {
            var t = speed;
            speed = tempSpeed;
            tempSpeed = t;
            _run = true;
            Animator.SetBool("Run",true);
        }
        
        [PunRPC]
        private void DamagePig(float damage)
        {
            Animator.SetTrigger("Hit");
            Hit = true;
            if (damage < lifePoints)
            {
                Anger();
                lifePoints = lifePoints - damage;
                StartCoroutine (nameof(StopPig));
            }
            else StartCoroutine (nameof(DiePig));
        }
        
        private IEnumerator StopPig()
        {
            Rb.velocity = Vector2.zero;
            yield return new WaitForSeconds (Animator.GetCurrentAnimatorStateInfo(0).length);
            Hit = false;
        }

        private IEnumerator DiePig(){
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
