﻿using System.Collections;
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
            yield return new WaitForSeconds (5);
            Destroy(gameObject);
        }
    }
}
