using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Mechanics.Enemies
{
    
    public class Duck : Movable
    {
        private Vector2 _direction;
        [SerializeField] private bool initialDirection;
        [SerializeField] private float height;
        private bool _jumping;
        
        // Start is called before the first frame update
        void Start()
        {
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
            if (!_jumping)
            {
                Rb.AddForce(new Vector2(_direction.x * .3f, height),ForceMode2D.Impulse);
                _jumping = true;
            }
            
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 100, LayerMask.GetMask("Obstacle"));
            if (hit && hit.distance < 1.5f)
            {
                _jumping = false;
            }
            
            if (Physics2D.OverlapPoint(Tr.position + new Vector3(_direction.x, 0, 0), LayerMask.GetMask("Obstacle")))
            {
                _direction = Vector2.Reflect(_direction, Vector2.right);
                Vector3 newScale = Tr.localScale;
                newScale.x = -_direction.x;
                Tr.localScale = newScale;
            }
        }
    }
}
