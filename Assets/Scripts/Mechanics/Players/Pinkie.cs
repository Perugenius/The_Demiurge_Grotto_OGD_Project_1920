using System.IO;
using Mechanics.Players.PlayerAttacks;
using Photon.Pun;
using UnityEngine;

namespace Mechanics.Players
{
    public class Pinkie : PlayableCharacter
    {
        private Transform _pillowSpawnPosition;

        private bool _canSummonPillow = true;
        private int _maxPillowNumber;
        
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            _pillowSpawnPosition = transform.Find("PillowSpawner").transform;
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            if (Input.GetButtonDown("Attack") && _canSummonPillow)
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
            _canSummonPillow = false;
            GameObject pillow = PhotonNetwork.Instantiate(Path.Combine("Players", "Pillow"),
                _pillowSpawnPosition.position, Quaternion.identity);
            pillow.transform.SetParent(gameObject.transform);
            Pillow pillowScript = pillow.GetComponent<Pillow>();
            pillowScript.SetPlayerPosition(transform);
            pillowScript.SetDamage(CurrentAttack);
            pillowScript.SetDuration(5);
            pillowScript.SetSpeed(90);
            pillowScript.SetPinkie(this);
        }

        public void SetCanSummonPillow(bool canSummon)
        {
            _canSummonPillow = canSummon;
        }
    }
}
