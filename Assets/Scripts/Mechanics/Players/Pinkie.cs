using UnityEngine;

namespace Mechanics.Players
{
    public class Pinkie : PlayableCharacter
    {
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            if (Input.GetButtonDown("Attack"))
            {
                Attack();
            }
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
