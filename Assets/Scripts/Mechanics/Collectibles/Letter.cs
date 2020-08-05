using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Mechanics.Collectibles
{
    public enum LetterType
    {
        Teammate, Eldaan
    }
    public class Letter : MonoBehaviour
    {
        public LetterType type = LetterType.Teammate;
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
            PhotonView photonView = other.gameObject.GetPhotonView();
            photonView = (photonView == null) ? other.transform.parent.gameObject.GetPhotonView() : photonView;
            if (!photonView.IsMine) return;
            //TODO call player
            transform.localScale = new Vector3(1,1,1);
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
