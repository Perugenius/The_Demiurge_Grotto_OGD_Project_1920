using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mechanics.Players
{
    public class RightWallJumpTrigger : MonoBehaviour
    {
        [SerializeField] private Steve steveScript;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle") && steveScript.GetIsJumping())
            {
                steveScript.IsOnWall = true;
                steveScript.WallSide = WallSide.RightWall;
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle") && steveScript.GetIsJumping())
            {
                steveScript.IsOnWall = true;
                steveScript.WallSide = WallSide.RightWall;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if ((LayerMask.GetMask("Obstacle") & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
            {
                steveScript.IsOnWall = false;
                steveScript.SetIsJumping(true);
            }
        }
    }
}
