using System;
using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(menuName = "CharacterStatistics")]
    public class CharacterStatistics : ScriptableObject
    {
        public String name;
        public float maxHealth;
        public float movSpeed;
        public float fightSpeed;
        public float rangeOfView;
        public float jumpHeight;
        public float attack;
        public int level;
        public int maxJumps;
        public float attackSpeed;
        public float attackRate;
        public int projectileNumber;
        public float attackRange;
        public float attackDuration;
        public float secondarySkillDuration;
        public float secondarySkillTimeRecharge;

    }
}
