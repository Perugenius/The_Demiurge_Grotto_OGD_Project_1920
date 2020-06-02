using System;
using UnityEngine;

namespace Mechanics.Players
{
    public class PlayerHitBox : MonoBehaviour
    {

        private Character _father;

        private void Start()
        {
            _father = gameObject.GetComponentInParent<Character>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            String layer = LayerMask.LayerToName(other.gameObject.GetComponent<LayerMask>());
            if (layer.Contains("DamageEnemy") || layer.Contains("DamageTrap"))
            {
                //TODO take damage;
            }
        }
    }
}
