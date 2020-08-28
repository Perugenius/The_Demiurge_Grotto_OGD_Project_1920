using System.Collections;
using Mechanics.Players;
using Photon.Pun;
using UnityEngine;

namespace Mechanics.Collectibles
{
    public class Food : MonoBehaviour
    {
        public int value = 1;
        [SerializeField] private Animator _animator;
        private static readonly int IsCollected = Animator.StringToHash("isCollected");
        private bool _isCollected = false;

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
            PhotonView photonView = other.gameObject.GetPhotonView();
            photonView = (photonView == null) ? other.transform.parent.gameObject.GetPhotonView() : photonView;
            if (!photonView.IsMine) return;
            PlayableCharacter playableCharacter = other.gameObject.GetComponent<PlayableCharacter>();
            if(playableCharacter!=null && !_isCollected)
            {
                _isCollected = true;
                playableCharacter.RefillHealth(value);
                _animator.SetBool(IsCollected, true);
                StartCoroutine(WaitBeforeDestroy());
            }
        }

        private IEnumerator WaitBeforeDestroy()
        {
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
        }
    }
}