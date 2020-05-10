using System;
using System.Collections;
using System.Collections.Generic;
using Mechanics;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [SerializeField] private float jumpHeight = 40;
    [SerializeField] private Animator _animator;
    private static readonly int Jump = Animator.StringToHash("jump");

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Movable otherMovable = other.gameObject.GetComponent<Movable>();
        if (otherMovable != null) otherMovable.Jump(jumpHeight);
        _animator.SetBool(Jump, true);
        StartCoroutine(WaitEndJumpAnim());
    }

    private IEnumerator WaitEndJumpAnim()
    {
        yield return new WaitForSeconds(0.4f);
        _animator.SetBool(Jump, false);
    }
}
