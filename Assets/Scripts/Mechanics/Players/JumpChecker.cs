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
            bool disactiveTrigger = false;
            string otherName = other.gameObject.name;
            if (otherName.Contains("Room") || otherName.Contains("Camera") || otherName.Contains("Transition") ||
                otherName.Contains("CheckPoint"))
                disactiveTrigger = true;
            if (!disactiveTrigger && !other.gameObject.CompareTag("FanAir") && !other.gameObject.CompareTag("JumpDisabled") && !other.gameObject.CompareTag("MessaggeTrigger"))
            {
                if (_script.GetIsJumping()){ _script.SetIsJumping(false);}
                if(_script is Kinja kinja) kinja.SetJumpsNumber(0);
                else if (_script is Steve steve) steve.IsOnWall = false;
            }
            else if (other.gameObject.CompareTag("JumpDisabled"))
            {
                _script.SetIsJumping(true);
                if(_script is Kinja kinja) kinja.SetJumpsNumber(1);
                else if (_script is Steve steve) steve.IsOnWall = false;
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            bool disactiveTrigger = false;
            string otherName = other.gameObject.name;
            if (otherName.Contains("Room") || otherName.Contains("Camera") || otherName.Contains("Transition") ||
                otherName.Contains("CheckPoint"))
                disactiveTrigger = true;
            if (!disactiveTrigger && !other.gameObject.CompareTag("FanAir") && !other.gameObject.CompareTag("JumpDisabled") && !other.gameObject.CompareTag("MessaggeTrigger"))
            {
                Debug.Log(other.gameObject.name);
                if (_script.GetIsJumping()) _script.SetIsJumping(false);
                if(_script is Kinja kinja) kinja.SetJumpsNumber(0);
                else if (_script is Steve steve) steve.IsOnWall = false;
            }
            else if (other.gameObject.CompareTag("JumpDisabled"))
            {
                _script.SetIsJumping(true);
                if(_script is Kinja kinja) kinja.SetJumpsNumber(1);
                else if (_script is Steve steve) steve.IsOnWall = false;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            bool disactiveTrigger = false;
            string otherName = other.gameObject.name;
            if (otherName.Contains("Room") || otherName.Contains("Camera") || otherName.Contains("Transition") ||
                otherName.Contains("CheckPoint"))
                disactiveTrigger = true;
            if (!disactiveTrigger && !other.gameObject.CompareTag("FanAir") && !other.gameObject.CompareTag("JumpDisabled") && !other.gameObject.CompareTag("MessaggeTrigger"))
            {
                _script.SetIsJumping(true);
                if (_script is Kinja kinja) kinja.SetJumpsNumber(1);
            }
        }
    }
}
