using UnityEngine;
using UnityEngine.Serialization;

namespace Mechanics.Enemies
{
    public class Chicken : Movable
    {
        private Vector2 _direction;
        private Animator _animator;
        [SerializeField] private float speed;
        [SerializeField] private bool initialDirection;
        [SerializeField] private float tempSpeed;
        private bool run;
        
        
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
            if(!Physics2D.OverlapPoint(Tr.position + new Vector3(0, -1.1f, 0), LayerMask.GetMask("Obstacle"))) return;    //if falling, it does nothing
            base.FixedUpdate();
            MoveDynamic(_direction, speed);
            if (!Physics2D.OverlapPoint(Tr.position + new Vector3(_direction.x, -1.1f, 0), LayerMask.GetMask("Obstacle"))
                || Physics2D.OverlapPoint(Tr.position + new Vector3(_direction.x, 0, 0), LayerMask.GetMask("Obstacle")))
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
                    _animator.SetBool("Run",false);
                }
            }
            RaycastHit2D hit = Physics2D.Raycast(transform.position, _direction, 100, LayerMask.GetMask("PlayerPhysic"));
            if (hit.collider && !run)
            {
                var t = speed;
                speed = tempSpeed;
                tempSpeed = t;
                _animator.SetBool("Run",true);
            }
        }
    }
}
