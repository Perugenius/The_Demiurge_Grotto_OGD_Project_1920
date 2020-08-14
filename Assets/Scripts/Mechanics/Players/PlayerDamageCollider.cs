using UnityEngine;

namespace Mechanics.Players
{
    public class PlayerDamageCollider : MonoBehaviour,IDamageInflictor
    {

        private Character _father;
        private float _damage;
    
        // Start is called before the first frame update
        void Start()
        {
            _father = gameObject.GetComponentInParent<Character>();
            _damage = _father.statistics.attack;
        }

        // Update is called once per frame
        public float GetDamage()
        {
            return _damage;
        }
    }
}
