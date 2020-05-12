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
        _mainCamera = GameObject.Find("Main Camera").GetComponent<CameraFocusOnPlayer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _mainCamera.PlayerInVerticalTransition(_tr.position, other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _mainCamera.PlayerOutVerticalTransition(other.gameObject);
    }
}
