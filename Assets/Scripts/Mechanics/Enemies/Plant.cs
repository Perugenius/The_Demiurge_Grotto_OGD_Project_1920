using UnityEngine;

namespace Mechanics.Enemies
{
    public class Plant : MonoBehaviour
    {
        private Vector2 _direction;
        private Transform _tr;
        private bool _shooting;
        
        // Start is called before the first frame update
        void Start()
        {
            _tr = GetComponent<Transform>();
            RaycastHit2D left = Physics2D.Raycast(_tr.position, Vector2.left,100, LayerMask.GetMask("PlayerPhysic","Obstacle"));
            RaycastHit2D right = Physics2D.Raycast(_tr.position, Vector2.right, 100, LayerMask.GetMask("PlayerPhysic","Obstacle"));
            Debug.Log(left.distance);
            Debug.Log(right.distance);
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
            if (left.collider && left.collider.gameObject.layer == LayerMask.NameToLayer("PlayerPhysic"))
            {
                Debug.Log("left");
                _tr.Rotate(0f,Vector3.Angle(_direction,Vector2.left),0f);
                _direction = Vector2.left;
                _shooting = true;
            }
            if (right.collider && right.collider.gameObject.layer == LayerMask.NameToLayer("PlayerPhysic"))
            {
                Debug.Log("right");
                _tr.Rotate(0f,Vector3.Angle(_direction,Vector2.right),0f);
                _direction = Vector2.right;
                _shooting = true;
            }

            if (left.collider.gameObject.layer != LayerMask.NameToLayer("PlayerPhysic") &&
                right.collider.gameObject.layer != LayerMask.NameToLayer("PlayerPhysic") && _shooting)
            {
                Debug.Log("boh");
                _shooting = false;
            }
        }
    }
}
