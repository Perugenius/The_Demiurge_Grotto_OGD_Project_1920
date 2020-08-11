using System.Collections;
using UnityEngine;

namespace Mechanics.Enemies
{
    public class Pig : Movable
    {
        private Vector2 _direction;
        private Animator _animator;
        [SerializeField] private float speed;
        [SerializeField] private bool initialDirection;
        [SerializeField] private float tempSpeed;
        [SerializeField] private float lifePoints;
        private bool _run;
        private Transform _player;
        private bool _following;
        
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
                    _animator.SetBool("Run",false);
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
            MoveDynamic(_direction, speed);
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
            _run = true;
            _animator.SetBool("Run",true);
        }
        
        private void Damage(float damage)
        {
            _animator.SetBool("Hit",true);
            if (damage < lifePoints)
            {
                Anger();
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
