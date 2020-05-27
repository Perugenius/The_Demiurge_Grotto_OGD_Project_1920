using System.Collections;
using System.Collections.Generic;
using Mechanics;
using UnityEngine;

public class ChestPiece : MonoBehaviour
{
    [SerializeField] private Vector2 impulseDirection;
    [SerializeField] private float impulseIntensity = 1f;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(rb.velocity.x,0);
        rb.AddForce(impulseDirection.normalized * impulseIntensity,ForceMode2D.Impulse);
        StartCoroutine(WaitBeforeDestroying());
    }

    private IEnumerator WaitBeforeDestroying()
    {
        yield return new WaitForSeconds(30);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
