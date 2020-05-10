using System.Collections;
using System.Collections.Generic;
using Mechanics;
using Photon.Pun.Demo.Cockpit;
using UnityEngine;

public class StoneFace : Movable
{
    [SerializeField] private bool isDown;
    private bool _isDown;
    [SerializeField] private float _distance;
    private float initSpeed = 1f;
    private float _acceleration = 500f;
    [SerializeField] private Animator _animator;
    private static readonly int IsDown = Animator.StringToHash("isDown");
    private bool _isWaiting = false;

    // Start is called before the first frame update
    void Start()
    {
        _isDown = isDown;
        _animator.SetBool(IsDown,true);
    }

    public void SwitchState()
    {
        if (MoveFixedDistanceAccelerated) return;

        if (_isDown)
        {
            SetFixedDistanceAccelerated(Vector2.up, initSpeed, _distance, _acceleration);
            _isDown = false;
            _animator.SetBool(IsDown,false);
            if (!_isWaiting)
            {
                _isWaiting = true;
                StartCoroutine(WaitEndTransition());
            }
        } else
        {
            SetFixedDistanceAccelerated(Vector2.down, initSpeed, _distance, _acceleration);
            _isDown = true;
            _animator.SetBool(IsDown,true);
            if (!_isWaiting)
            {
                _isWaiting = true;
                StartCoroutine(WaitEndTransition());
            }
        }
    }

    private IEnumerator WaitEndTransition()
    {
        while (MoveFixedDistanceAccelerated)
        {
            yield return null;
        }
        
        if(_isDown != isDown)SwitchState();

        _isWaiting = false;
    }
}
