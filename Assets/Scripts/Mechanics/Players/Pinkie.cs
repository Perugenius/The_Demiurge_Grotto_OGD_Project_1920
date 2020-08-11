using System.IO;
using Mechanics.Players.PlayerAttacks;
using Photon.Pun;
using UnityEngine;

namespace Mechanics.Players
{
    public class Pinkie : PlayableCharacter
    {
        private Transform _pillowSpawnPosition;
        
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
            GameObject pillow = PhotonNetwork.Instantiate(Path.Combine("Players", "Pillow"),
                _pillowSpawnPosition.position, Quaternion.identity);
            pillow.transform.SetParent(gameObject.transform);
            Pillow pillowScript = pillow.GetComponent<Pillow>();
            pillowScript.SetDamage(CurrentAttack);
            pillowScript.SetSpeed(3);
        }
    }
}
