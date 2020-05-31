using System.Collections;
using UnityEngine;

namespace Mechanics.Traps
{
    public class Sand : MonoBehaviour
    {
        [SerializeField] private float intensity = 20f;
        [SerializeField] private float gravityReduceFactor = 8f;
        [SerializeField] private float particleFrequency = 3f;
        [SerializeField] private float particleIntensity = 10f;
        [SerializeField] private GameObject particle;
        //[SerializeField] private float resistanceRate = 12f;

        private int _count;


        /*private void OnTriggerStay2D(Collider2D other)
        {
            _count++;
            Debug.Log(other.gameObject.name);
            Movable player = other.gameObject.GetComponent<Movable>();
            Debug.Log("Movable = " + ((player!=null)?player.ToString():"null"));
            if (player != null)
            {
                Vector2 resistanceForceDirection = (player.GetMovingDirection().y > 0) ? Vector2.down: Vector2.up;
                float resistanceForceIntensity = (player.GetMovingDirection().y > 0) ? intensity: intensity*(_count>=600?1:0.1f/(_count/60f));
                player.AddForce(resistanceForceDirection, intensity);
            } 
        }*/
        
        private void OnTriggerStay2D(Collider2D other)
        {
            _count++;
            Movable player = other.gameObject.GetComponent<Movable>();
            if (player != null)
            {
                Vector2 resistanceForceDirection = (player.GetMovingDirection().y > 0) ? Vector2.down: Vector2.up;
                float resistanceForceIntensity = (player.GetMovingDirection().y > 0) ? intensity: intensity*0.60f;
                player.AddForce(resistanceForceDirection, resistanceForceIntensity);
            }

            if (_count >= Mathf.Floor(60f / particleFrequency) && player!=null)
            {
                _count = 0;
                CreateParticle(player.gameObject.transform.position-new Vector3(0,0.5f,0));
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
            if (player != null && player.GetComponent<Rigidbody2D>()!=null)
                player.GetComponent<Rigidbody2D>().gravityScale = player.GetComponent<Rigidbody2D>().gravityScale / gravityReduceFactor;
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            GameObject player = other.gameObject;
            if (player != null && player.GetComponent<Rigidbody2D>()!=null)
                player.GetComponent<Rigidbody2D>().gravityScale = player.GetComponent<Rigidbody2D>().gravityScale * gravityReduceFactor;
        }
    }
}
