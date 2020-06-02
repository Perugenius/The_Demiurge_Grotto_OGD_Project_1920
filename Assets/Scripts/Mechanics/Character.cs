using System.Collections;
using Scriptable_Objects;
using UnityEngine.PlayerLoop;

namespace Mechanics
{
    public abstract class Character : Movable
    {
        public CharacterStatistics statistics;
        protected float CurrentHealth;
        protected float CurrentSpeed;
        protected float CurrentAttack;

        protected bool IsJumping;

        public void SetIsJumping(bool j)
        {
            IsJumping = j;
        }

        public bool GetIsJumping()
        {
            return IsJumping;
        }

        public void TakeDamage(float damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        protected abstract void Die();
        protected abstract void Attack();

    }
}
