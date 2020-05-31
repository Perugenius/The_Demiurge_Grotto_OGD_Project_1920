using UnityEngine;

namespace Mechanics.Traps
{
    public class StoneFaceDeathTrigger : MonoBehaviour
    {
        [SerializeField] private StoneFace stoneFace;
        private void OnTriggerEnter2D(Collider2D other)
        { 
            stoneFace.Die();
            Destroy(gameObject);
        }
    }
}
