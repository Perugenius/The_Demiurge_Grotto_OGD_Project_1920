using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private StoneFace stoneFace;
    private int _playersInside = 0;
    [SerializeField] private GameObject activeBody;
    [SerializeField] private GameObject inactiveBody;

    private void OnTriggerEnter2D(Collider2D other)
    {
        _playersInside++;
        if(_playersInside > 1) return;
        activeBody.SetActive(true);
        inactiveBody.SetActive(false);
        stoneFace.SwitchState();
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if(_playersInside > 0) _playersInside--;
        if(_playersInside > 0) return;
        activeBody.SetActive(false);
        inactiveBody.SetActive(true);
        stoneFace.SwitchState();
    }
}
