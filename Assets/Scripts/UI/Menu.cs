using System.Collections;
using System.Collections.Generic;
using Mechanics;
using UnityEngine;
using UnityEngine.EventSystems;

public class Menu : Movable
{
    [SerializeField] private float speed;
    [SerializeField] private float distance;
    [SerializeField] private Vector2 unfocusedDirection;
    [SerializeField] private bool defaultFocused;
    [SerializeField] private GameObject firstButton;
    private bool _isFocused = false;
    private EventSystem _eventSystem;

    // Start is called before the first frame update
    void OnEnable()
    {
        _isFocused = defaultFocused;
        _eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Focus(bool isFocused)
    {
        if (_isFocused == isFocused)return;
        _isFocused = isFocused;
        if(isFocused)
        {
            SetFixedDistance(-unfocusedDirection, speed, distance);
            _eventSystem.SetSelectedGameObject(firstButton);
        }
        else
        {
            SetFixedDistance(unfocusedDirection, speed, distance);
            StartCoroutine(WaitBeforeDisabling());
        }
    }

    private IEnumerator WaitBeforeDisabling()
    {
        while (_isFocused || MoveFixedDistance)
        {
            yield return null;
        }
        gameObject.SetActive(false);
    }

    public bool isFocused()
    {
        return _isFocused;
    }
}
