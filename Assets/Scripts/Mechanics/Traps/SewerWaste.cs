using System;
using System.Collections;
using Mechanics.Players;
using Photon.Pun;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

namespace Mechanics.Traps
{
    public class SewerWaste : MonoBehaviour
    {
        [SerializeField] private float intensity = 20f;
        [SerializeField] private float gravityReduceFactor = 8f;
        [SerializeField] private int numberOfParticles;
        [SerializeField] private float particleIntensity = 10f;
        [SerializeField] private float particlesShootPeriod = 3f;
        [SerializeField] private GameObject particle;
        private double _count;
        private bool _playerInside;

        private void OnTriggerStay2D(Collider2D other)
        {
            Movable player = other.gameObject.GetComponent<Movable>();
            if (player != null)
            {
                if (other.gameObject.CompareTag("Player"))
                {
                    //get player photonView 
                    PhotonView photonView = other.gameObject.GetComponent<PhotonView>();
                    photonView = photonView == null ? other.transform.parent.gameObject.GetComponent<PhotonView>() : photonView;
                
                    if(!photonView.IsMine) return;
                }
                
                Vector2 resistanceForceDirection = (player.GetMovingDirection().y > 0) ? Vector2.down: Vector2.up;
                float resistanceForceIntensity = (player.GetMovingDirection().y > 0) ? intensity: intensity*0.60f;
                player.AddForce(resistanceForceDirection, resistanceForceIntensity);
            }
        }

        private void CreateParticle(Vector3 position)
        {
            GameObject instantiatedParticle = Instantiate(particle, position, Quaternion.identity);
            instantiatedParticle.GetComponent<Movable>().AddForce(new Vector2(Random.Range(-0.5f,0.5f),1f), particleIntensity);
            StartCoroutine(WaitBeforeDeletingParticle(instantiatedParticle));
        }

        private IEnumerator WaitBeforeDeletingParticle(GameObject instantiatedParticle)
        {
            yield return new WaitForSeconds(3f);
            Destroy(instantiatedParticle);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            GameObject player = other.gameObject;

            //Create particles
            for (int i = 0; i < numberOfParticles && !_playerInside && Math.Abs(_count) < 0.1f; i++)
            {
                CreateParticle(player.gameObject.transform.position-new Vector3(0,0.5f,0));
            }
            
            if (player != null && player.GetComponent<Rigidbody2D>()!=null)
            {
                PlayableCharacter playableCharacter = player.GetComponent<PlayableCharacter>();
                if (playableCharacter != null) playableCharacter.Poisoned1 = true;
                _playerInside = true;
                player.GetComponent<Rigidbody2D>().gravityScale =
                    player.GetComponent<Rigidbody2D>().gravityScale / gravityReduceFactor;
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            GameObject player = other.gameObject;
            if (player != null && player.GetComponent<Rigidbody2D>()!=null)
            {
                PlayableCharacter playableCharacter = player.GetComponent<PlayableCharacter>();
                if (playableCharacter != null) playableCharacter.Poisoned1 = false;
                _playerInside = false;
                player.GetComponent<Rigidbody2D>().gravityScale =
                    player.GetComponent<Rigidbody2D>().gravityScale * gravityReduceFactor;
            }
        }

        private void Update()
        {
            if (_playerInside && _count <= particlesShootPeriod * 60) _count++;
            else _count = 0;
        }
    }
}
