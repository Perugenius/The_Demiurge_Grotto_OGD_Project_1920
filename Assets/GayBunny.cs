using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GayBunny : MonoBehaviour
{
    private Transform _tr;
    // Start is called before the first frame update
    void Start()
    {
        _tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W)) _tr.position = _tr.position + Vector3.up*0.1f;
        if (Input.GetKey(KeyCode.S)) _tr.position = _tr.position + Vector3.down*0.1f;
        if (Input.GetKey(KeyCode.D)) _tr.position = _tr.position + Vector3.right*0.1f;
        if (Input.GetKey(KeyCode.A)) _tr.position = _tr.position + Vector3.left*0.1f;
    }
}
