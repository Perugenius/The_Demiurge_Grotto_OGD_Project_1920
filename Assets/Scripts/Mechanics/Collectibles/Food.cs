using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Mechanics.Collectibles
{
    public class Food : MonoBehaviour
    {
        public int value = 1;
        [SerializeField] private Animator _animator;
        private static readonly int IsCollected = Animator.StringToHash("isCollected");

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.GetPhotonView().IsMine) return;
            //TODO call player
            _animator.SetBool(IsCollected, true);
            StartCoroutine(WaitBeforeDestroy());
        }

        private IEnumerator WaitBeforeDestroy()
        {
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
        }
    }
}