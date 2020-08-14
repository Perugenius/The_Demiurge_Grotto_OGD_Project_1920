using System.IO;
using Mechanics.Players.PlayerAttacks;
using Photon.Pun;
using UnityEngine;

namespace Mechanics.Players
{
    public class Kinja : PlayableCharacter
    {
        private static readonly int DoubleJump = Animator.StringToHash("DoubleJump");
        private int _maxJumpsNumber;
        private int _jumpsNumber;
        private Transform _attackSpawner;
        
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            _maxJumpsNumber = statistics.maxJumps;
            _attackSpawner = transform.Find("AttackPoint").transform;
        }

        // Update is called once per frame
        protected override void Update()
        {
            if (gameObject.GetPhotonView().IsMine || localTesting)
            {
                if (Input.GetButtonDown("Jump") && _jumpsNumber < _maxJumpsNumber && IsJumping)
                {
                    Jump(45);
                    _jumpsNumber++;
                    Animator.SetBool(IsJumpingAnim, true);
                    Animator.SetTrigger(DoubleJump);
                }

                if (Input.GetButtonDown("Attack") && IsJumping)
                {
                    Attack();
                }

                base.Update();
            }
        }

        public void SetJumpsNumber(int num)
        {
            _jumpsNumber = num;
        }

        protected override void Attack()
        {
            GameObject shuriken = PhotonNetwork.Instantiate(Path.Combine("Players", "Shuriken"), _attackSpawner.position, Quaternion.identity);
            Shuriken shurikenScript = shuriken.GetComponent<Shuriken>();
            shurikenScript.SetDamage(CurrentAttack);
            shurikenScript.SetSpeed(Vector2.down*12);
        }
    }
}
