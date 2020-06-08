using UnityEngine;

namespace Mechanics.Players
{
    public class Kinja : PlayableCharacter
    {

        private int _maxJumpsNumber;
        private int _jumpsNumber;
        
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            _maxJumpsNumber = statistics.maxJumps;
        }

        // Update is called once per frame
        protected override void Update()
        {
            if (Input.GetButtonDown("Jump") && _jumpsNumber < _maxJumpsNumber && IsJumping)
            {
                Jump(62);
                _jumpsNumber++;
                Animator.SetBool(IsJumpingAnim, true);
            }
            base.Update();
        }

        public void SetJumpsNumber(int num)
        {
            _jumpsNumber = num;
        }
        
        protected override void Die()
        {
            throw new System.NotImplementedException();
        }

        protected override void Attack()
        {
            throw new System.NotImplementedException();
        }
    }
}
