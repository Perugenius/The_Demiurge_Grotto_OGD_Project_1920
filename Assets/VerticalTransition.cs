using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalTransition : MonoBehaviour
{
    private CameraFocusOnPlayer _mainCamera;
    private Transform _tr;

    private void Start()
    {
        _tr = GetComponent<Transform>();
        StartCoroutine(WaitForCamera());
    }
    
    private IEnumerator WaitForCamera()
    {
        GameObject camera = GameObject.Find("Main Camera(Clone)");
        while (camera == null)
        {
            yield return new WaitForSeconds(1);
            camera = GameObject.Find("Main Camera(Clone)");
        }

        _mainCamera = camera.GetComponent<CameraFocusOnPlayer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(_mainCamera!=null)_mainCamera.PlayerInVerticalTransition(_tr.position, other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(_mainCamera!=null)_mainCamera.PlayerOutVerticalTransition(other.gameObject);
    }
}
