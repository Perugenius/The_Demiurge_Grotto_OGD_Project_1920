using UnityEngine;

namespace Mechanics.Players
{
    public class LeftWallJumpTrigger : MonoBehaviour
    {
        [SerializeField] private Steve steveScript;

        private void OnTriggerEnter2D(Collider2D other)
        {
            steveScript.IsOnWall = true;
            steveScript.WallSide = WallSide.LeftWall;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            steveScript.IsOnWall = true;
            steveScript.WallSide = WallSide.LeftWall;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            steveScript.IsOnWall = false;
            steveScript.SetIsJumping(true);
        }
    }
}
