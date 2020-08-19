using System.Collections;
using System.Collections.Generic;
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
        private float _invincibilityDuration;
        private float _timeLapse;
        private GameObject _hitBox;

        private List<GameObject> _pillowSpawners;
        private float _attackDuration;
        private float _projectileSpeed;
        
        
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            if (IsMine)
            {
                _pillowSpawners = new List<GameObject>();
                foreach (Transform child in transform)
                {
                    if (child.CompareTag("ProjectileSpawner"))
                    {
                        _pillowSpawners.Add(child.gameObject);
                    }
                }
                _pillowSpawnPosition = transform.Find("PillowSpawner").transform;
                _hitBox = transform.Find("PlayerHitbox").gameObject;
                _timeLapse = statistics.attackRate;
                _maxPillowNumber = statistics.projectileNumber;
                for (int i = 3; i>=_maxPillowNumber; i--)
                {
                    _pillowSpawners[i].SetActive(false);
                }
                _attackDuration = PlayerData.attackDuration[CharacterName];
                _projectileSpeed = PlayerData.projectileSpeed[CharacterName];
            }
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
        

        protected override void Attack()
        {
            foreach (var elem in _pillowSpawners)
            {
                if (elem.activeSelf)
                {
                    _canSummonPillow = false;
                    GameObject pillow = PhotonNetwork.Instantiate(Path.Combine("Players", "Pillow"),
                        elem.transform.position, Quaternion.identity);
                    pillow.transform.SetParent(gameObject.transform);
                    Pillow pillowScript = pillow.GetComponent<Pillow>();
                    pillowScript.SetPlayerPosition(transform);
                    pillowScript.SetDamage(CurrentAttack);
                    pillowScript.SetDuration(_attackDuration);
                    pillowScript.SetSpeed(_projectileSpeed);
                    pillowScript.SetPinkie(this);
                }
                
            }
            
        }

        public void SetCanSummonPillow(bool canSummon)
        {
            
            _canSummonPillow = canSummon;
        }

        private void UseSecondary()
        {
            _canDisappear = false;
            SpriteRenderer.color = new Color(255,255,255,0.5f);
            _hitBox.SetActive(false);
            StartCoroutine(nameof(InvincibilityTime));
        }

        private IEnumerator InvincibilityTime()
        {
            yield return new WaitForSeconds(statistics.secondarySkillDuration);
            for (int i = 0; i < 4; i++)
            {
                SpriteRenderer.color = new Color(255,255,255, 0.8f);
                yield return new WaitForSeconds(0.25f);
                SpriteRenderer.color = new Color(255,255,255, 0.5f);
                yield return new WaitForSeconds(0.25f);
            }
            SpriteRenderer.color = new Color(255,255,255,1);
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
