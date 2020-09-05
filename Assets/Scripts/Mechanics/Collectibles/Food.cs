using System.Collections;
using Mechanics.Players;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Mechanics.Collectibles
{
    public class Food : MonoBehaviour
    {
        public int value = 1;
        [SerializeField] private Animator _animator;
        [SerializeField] private Text _text;
        private static readonly int IsCollected = Animator.StringToHash("isCollected");
        private bool _isCollected = false;
        private SpriteRenderer _spriteRenderer;
        [SerializeField] private bool offlineMode = false;

        // Start is called before the first frame update
        void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (offlineMode)
            {
                Collect(other);
                return;
            }
            
            PhotonView photonView = other.gameObject.GetPhotonView();
            photonView = (photonView == null) ? other.transform.parent.gameObject.GetPhotonView() : photonView;
            if (!photonView.IsMine) return;
            Collect(other);
        }

        private void Collect(Collider2D playerCollider)
        {
            PlayableCharacter playableCharacter = playerCollider.gameObject.GetComponent<PlayableCharacter>();
            if(playableCharacter!=null && !_isCollected)
            {
                _isCollected = true;
                playableCharacter.RefillHealth(value);
                _animator.SetBool(IsCollected, true);
                StartCoroutine(WaitBeforeLogQuantity());
            }
        }
        
        private IEnumerator WaitBeforeLogQuantity()
        {
            yield return new WaitForSeconds(0.5f);
            _spriteRenderer.enabled = false;
            LogQuantity();
        }
        
        private void LogQuantity()
        {
            _text.text = value.ToString();
            StartCoroutine(WaitBeforeDestroy());
        }

        private IEnumerator WaitBeforeDestroy()
        {
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(DestroyUiQuantity());
        }

        private IEnumerator DestroyUiQuantity()
        {
            while (_text.gameObject.transform.localScale.x > 0.001f)
            {
                _text.gameObject.transform.localScale -= new Vector3(0.001f,0.001f,0.001f);
                yield return new  WaitForEndOfFrame();
            }
            Destroy(gameObject);
        }
    }
}