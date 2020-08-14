using System;
using UnityEngine;

namespace Mechanics.Players
{
    public class PlayerHitBox : MonoBehaviour
    {

        private PlayableCharacter _father;

        private void Start()
        {
            _father = gameObject.GetComponentInParent<PlayableCharacter>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_father.IsTakingDamage1)
            {
                _father.TakeDamage(other);
            }
        }
    }
}
