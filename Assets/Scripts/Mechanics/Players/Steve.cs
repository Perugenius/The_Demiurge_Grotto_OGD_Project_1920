using System.Collections;
using System.IO;
using Mechanics.Players.PlayerAttacks;
using Photon.Pun;
using UnityEngine;

namespace Mechanics.Players
{
    public class Steve : PlayableCharacter
    {
        private Transform _spawnPosition;
        
        private float _fireRate = 1.5f;
        private bool _canAttack = true;
        private float _range = 5;
        private float _projectileSpeed = 10;
        
        
        
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            _spawnPosition = transform.Find("LaserSpawner");
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            if (Input.GetButtonDown("Attack") && _canAttack)
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
            _canAttack = false;
            GameObject laser =
                PhotonNetwork.Instantiate(Path.Combine("Players", "Laser"), _spawnPosition.position, Quaternion.identity);
            Laser laserScript = laser.GetComponent<Laser>();
            laserScript.Damage = CurrentAttack;
            laserScript.Direction = FaceDir;
            laserScript.Range = _range;
            laserScript.Speed = _projectileSpeed;
            StartCoroutine(nameof(AttackRecharge));
        }

        private IEnumerator AttackRecharge()
        {
            yield return new WaitForSeconds(_fireRate);
            _canAttack = true;
        }
    }
}
