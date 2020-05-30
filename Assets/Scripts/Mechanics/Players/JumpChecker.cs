using System;
using UnityEngine;

namespace Mechanics.Players
{
    
    
    
    public class JumpChecker : MonoBehaviour
    {

        private PlayableCharacter _script;
        
        void Start()
        {
            _script = gameObject.GetComponentInParent<PlayableCharacter>();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            _script.SetIsJumping(false);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _script.SetIsJumping(true);
        }
    }
}
