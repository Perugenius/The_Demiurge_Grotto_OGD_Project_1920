using Core.SaveLoadData;
using UnityEngine;

namespace Mechanics.Players
{
    public class PlayerDamageCollider : MonoBehaviour,IDamageInflictor
    {

        private PlayableCharacter _father;
        private float _damage;
    
        // Start is called before the first frame update
        void Start()
        {
            _father = gameObject.GetComponentInParent<PlayableCharacter>();
            _damage = SaveSystem.LoadPlayerData().attack[_father.CharacterName1];
        }

        // Update is called once per frame
        public float GetDamage()
        {
            return _damage;
        }
    }
}
