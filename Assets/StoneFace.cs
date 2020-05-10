using System.Collections;
using System.Collections.Generic;
using Mechanics;
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
        if (MoveFixedDistanceAccelerated)
        {
            if (!_isWaiting)
            {
                _isWaiting = true;
                StartCoroutine(WaitEndTransition());
                return;
            }
            else
            {
                return;
            }
        }
        
        
        if (_isDown)
        {
            SetFixedDistanceAccelerated(Vector2.up, initSpeed, _distance, _acceleration);
            _isDown = false;
            _animator.SetBool(IsDown,false);
        } else
        {
            SetFixedDistanceAccelerated(Vector2.down, initSpeed, _distance, _acceleration);
            _isDown = true;
            _animator.SetBool(IsDown,true);
        }
    }

    private IEnumerator WaitEndTransition()
    {
        while (MoveFixedDistanceAccelerated)
        {
            yield return null;
        }

        _isWaiting = false;
        SwitchState();
    }
}
