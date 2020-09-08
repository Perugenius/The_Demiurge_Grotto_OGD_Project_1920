using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mechanics.Players.PlayerAttacks;
using Photon.Pun;
using UI;
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
        private float _invincibilityCoolDown;

        private List<GameObject> _pillowSpawners;
        private float _attackDuration;
        private float _projectileSpeed;
        private float _secondarySkillDuration;
        private IEnumerator _invincibilityCoroutine;
        private Bar _skillBar;
        private bool _isInvincible;

        public GameObject pillowPrefab;
        
        
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            if (IsMine || localTesting)
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
                _invincibilityCoolDown = 4f;
                _maxPillowNumber = PlayerData.projectileNumber[CharacterName];
                _secondarySkillDuration = PlayerData.secondarySkillLevel[CharacterName]/2 +1;
                for (int i = 3; i>=_maxPillowNumber; i--)
                {
                    _pillowSpawners[i].SetActive(false);
                }
                _attackDuration = PlayerData.attackDuration[CharacterName];
                _projectileSpeed = PlayerData.projectileSpeed[CharacterName];
                AttackBar.SetDecreasingDuration(_attackDuration);
                _skillBar.SetDecreasingDuration(_secondarySkillDuration+2);
                _skillBar.SetIncreasingDuration(_invincibilityCoolDown);
            }
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            if (IsMine || localTesting)
            {
                if (Input.GetButtonDown("Attack") && _canSummonPillow && CanAttack)
                {
                    CanAttack = false;
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
            if (_isInvincible)
            {
                StopCoroutine(_invincibilityCoroutine);
                _isInvincible = false;
                _skillBar.ActualSize = 0;
                _skillBar.IsDecreasing = false;
                SpriteRenderer.color = new Color(255,255,255,1);
                Hitbox.SetActive(true);
                StartCoroutine(nameof(TimeLapse));
            }
            _canDisappear = false;
            foreach (var elem in _pillowSpawners)
            {
                if (elem.activeSelf)
                {
                    _canSummonPillow = false;
                    GameObject pillow;
                    if (!localTesting)
                    {
                        pillow = PhotonNetwork.Instantiate(Path.Combine("Players", "Pillow"),
                            elem.transform.position, Quaternion.identity);
                    }
                    else
                    {
                        pillow = GameObject.Instantiate(pillowPrefab,
                            elem.transform.position, Quaternion.identity);
                    }
                    pillow.transform.SetParent(gameObject.transform);
                    Pillow pillowScript = pillow.GetComponent<Pillow>();
                    if (localTesting)
                        pillowScript.Offline = true;
                    pillowScript.SetPlayerPosition(transform);
                    pillowScript.SetDamage(CurrentAttack);
                    pillowScript.SetDuration(_attackDuration);
                    pillowScript.SetSpeed(_projectileSpeed);
                    pillowScript.SetPinkie(this);
                }
            }
            AttackBar.IsDecreasing = true;

        }

        public void SetCanSummonPillow(bool canSummon)
        {
            _canSummonPillow = canSummon;
            _canDisappear = true;
            StartCoroutine(nameof(AttackTimeLapse));
        }

        private void UseSecondary()
        {
            if (!IsDying)
            {
                _canDisappear = false;
                SpriteRenderer.color = new Color(255, 255, 255, 0.5f);
                Hitbox.SetActive(false);
                _invincibilityCoroutine = InvincibilityTime();
                _skillBar.IsDecreasing = true;
                PhotonView.RPC(nameof(PinkieRemoteSecondarySkill), RpcTarget.Others, _secondarySkillDuration);
                StartCoroutine(_invincibilityCoroutine);
            }
        }

        [PunRPC]
        public void PinkieRemoteSecondarySkill(float duration)
        {
            _secondarySkillDuration = duration;
            _invincibilityCoroutine = InvincibilityTime();
            StartCoroutine(_invincibilityCoroutine);
        }
        
        private IEnumerator InvincibilityTime()
        {
            _isInvincible = true;
            SpriteRenderer.color = new Color(255, 255, 255, 0.5f);
            yield return new WaitForSeconds(_secondarySkillDuration);
            for (int i = 0; i < 4; i++)
            {
                SpriteRenderer.color = new Color(255,255,255, 0.8f);
                yield return new WaitForSeconds(0.25f);
                SpriteRenderer.color = new Color(255,255,255, 0.5f);
                yield return new WaitForSeconds(0.25f);
            }
            SpriteRenderer.color = new Color(255,255,255,1);
            Hitbox.SetActive(true);
            _isInvincible = false;
            StartCoroutine(nameof(TimeLapse));
        }

        private IEnumerator TimeLapse()
        {
            yield return  new WaitForSeconds(_invincibilityCoolDown);
            _canDisappear = true;
        }

        public Bar SkillBar
        {
            get => _skillBar;
            set => _skillBar = value;
        }
    }
}
