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
            if (!other.gameObject.name.Contains("Room") && !other.gameObject.name.Contains("Camera") && !other.gameObject.CompareTag("FanAir"))
            {
                if (_script.GetIsJumping()){ _script.SetIsJumping(false);}
                if(_script is Kinja kinja) kinja.SetJumpsNumber(0);
                else if (_script is Steve steve) steve.IsOnWall = false;
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.gameObject.name.Contains("Room") && !other.gameObject.name.Contains("Camera") && !other.gameObject.CompareTag("FanAir"))
            {
                if (_script.GetIsJumping()) _script.SetIsJumping(false);
                if(_script is Kinja kinja) kinja.SetJumpsNumber(0);
                else if (_script is Steve steve) steve.IsOnWall = false;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.gameObject.name.Contains("Room") && !other.gameObject.name.Contains("Camera") && !other.gameObject.CompareTag("FanAir"))
            {
                _script.SetIsJumping(true);
                if (_script is Kinja kinja) kinja.SetJumpsNumber(1);
            }
        }
    }
}
