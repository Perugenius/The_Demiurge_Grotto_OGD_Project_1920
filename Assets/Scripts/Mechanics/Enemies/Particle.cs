using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Mechanics.Enemies
{
    public class Particle : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine (nameof(Disappear));
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    
        private IEnumerator Disappear(){
            yield return new WaitForSeconds (2);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
