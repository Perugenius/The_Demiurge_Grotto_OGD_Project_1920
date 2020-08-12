using System.Collections;
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

        private bool _canDisappear = true;
        private SpriteRenderer _renderer;
        private float _invincibilityDuration;
        private float _timeLapse;
        private GameObject _hitBox;
        
        
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            _pillowSpawnPosition = transform.Find("PillowSpawner").transform;
            _renderer = GetComponent<SpriteRenderer>();
            _hitBox = transform.Find("PlayerHitbox").gameObject;
            _timeLapse = 4;
        }

        // Update is called once per frame
        protected override void Update()
        {
            if (gameObject.GetPhotonView().IsMine || localTesting)
            {
                base.Update();
                if (Input.GetButtonDown("Attack") && _canSummonPillow)
                {
                    Attack();
                }

                if (Input.GetButtonDown("SecondarySkill") && _canDisappear)
                {
                    UseSecondary();
                }
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
            pillowScript.SetSpeed(180);
            pillowScript.SetPinkie(this);
        }

        public void SetCanSummonPillow(bool canSummon)
        {
            _canSummonPillow = canSummon;
        }

        private void UseSecondary()
        {
            _canDisappear = false;
            _renderer.color = new Color(255,255,255,0.5f);
            _hitBox.SetActive(false);
            StartCoroutine(nameof(InvincibilityTime));
        }

        private IEnumerator InvincibilityTime()
        {
            yield return new WaitForSeconds(4);
            for (int i = 0; i <= 4; i++)
            {
                _renderer.color = new Color(255,255,255, 0.8f);
                yield return new WaitForSeconds(0.25f);
                _renderer.color = new Color(255,255,255, 0.5f);
                yield return new WaitForSeconds(0.25f);
            }
            _renderer.color = new Color(255,255,255,1);
            _hitBox.SetActive(true);
            StartCoroutine(nameof(TimeLapse));

        }

        private IEnumerator TimeLapse()
        {
            yield return  new WaitForSeconds(_timeLapse);
            _canDisappear = true;
        }
    }
}
