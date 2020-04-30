using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingExample : MonoBehaviour
{
    // An object is taken from the pool, NB ==> the object pool is created in GameManger class
    void Start()
    {
        GameObject go = ObjectPoolingManager.Instance.GetObject("PoolingExample");
        go.transform.position = new Vector2(5,5);
        go.transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
