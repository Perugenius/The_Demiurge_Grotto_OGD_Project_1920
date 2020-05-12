using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mechanics
{
    public class Trampoline : MonoBehaviour
    {
        [FormerlySerializedAs("jumpHeight")] [SerializeField] private float jumpForce = 40;
        [FormerlySerializedAs("_animator")] [SerializeField] private Animator animator;
        private static readonly int Jump = Animator.StringToHash("jump");

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
            Movable otherMovable = other.gameObject.GetComponent<Movable>();
            if (otherMovable != null && other.gameObject.GetPhotonView().IsMine) otherMovable.Jump(jumpForce);
            animator.SetBool(Jump, true);
            StartCoroutine(WaitEndJumpAnim());
        }

        private IEnumerator WaitEndJumpAnim()
        {
            yield return new WaitForSeconds(0.4f);
            animator.SetBool(Jump, false);
        }
    }
}
