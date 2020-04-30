using UnityEngine;

namespace Mechanics
{
    /// <summary>
    /// A simple controller for enemies.
    /// </summary>
    
    //If you want the script to require specific components:
    //[RequireComponent(typeof(AnimationController), typeof(Collider2D))]
    public class EnemyController : MonoBehaviour
    {

        void Awake()
        {
        }

        void Update()
        {
        }
        
        void OnCollisionEnter2D(Collision2D collision)
        {
        }
        
    }
}