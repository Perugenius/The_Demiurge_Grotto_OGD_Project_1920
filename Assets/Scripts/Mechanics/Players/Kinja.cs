﻿using System.IO;
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
        private int _projectileNumber;
        private float _projectileSpeed;
        
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            if (IsMine || localTesting)
            {
                _maxJumpsNumber = PlayerData.secondarySkillLevel[CharacterName] +1;
                _attackSpawner = transform.Find("AttackPoint").transform;
                _projectileNumber = PlayerData.projectileNumber[CharacterName];
                _projectileSpeed = PlayerData.projectileSpeed[CharacterName];
            }
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
            for (int i = 1; i <= _projectileNumber; i++)
            {
                Animator.SetTrigger(DoubleJump);
                GameObject shuriken = PhotonNetwork.Instantiate(Path.Combine("Players", "Shuriken"),
                    _attackSpawner.position, Quaternion.identity);
                Shuriken shurikenScript = shuriken.GetComponent<Shuriken>();
                shurikenScript.SetDamage(CurrentAttack);
                Vector2 direction = Vector2.down;
                switch (i)
                {
                    case 1:
                    {
                        direction = Vector2.down;
                        break;
                    }
                    case 2:
                    {
                        direction = new Vector2(FaceDirection.x/2,-1).normalized;
                        break;
                    }
                    case 3:
                    {
                        direction = new Vector2(-FaceDirection.x/2,-1).normalized;
                        break;
                    }
                    case 4:
                    {
                        direction = new Vector2(FaceDirection.x, -1).normalized;
                        break;
                    }
                    case 5:
                    {
                        direction = new Vector2(-FaceDirection.x, -1).normalized;
                        break;
                    }
                }
                shurikenScript.SetSpeed(direction * _projectileSpeed);
            }
        }
    }
}
