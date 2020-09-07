using System.Collections;
using System.Collections.Generic;
using Mechanics;
using UnityEngine;

public class Credits : Movable
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 5f;
    private Vector3 _startingPosition;
    private MainMenu _mainMenu;
    private GameObject _credits;
    private bool _started;

    // Start is called before the first frame update
    void Start()
    {
        _startingPosition = Tr.position;
        _mainMenu = GameObject.Find("MainMenu").gameObject.GetComponent<MainMenu>();
        _credits = GameObject.Find("Credits").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            StopCredits();
        }
        
        if(!MoveFixedDistance && _started) StopCredits();
    }

    public void StartCredits()
    {
        _started = true;
        SetFixedDistance(target.position,speed);
    }
    
    public void StopCredits()
    {
        _started = false;
        if (MoveFixedDistance) MoveFixedDistance = false;
        Tr.position = _startingPosition;
        _mainMenu.ShowHome();
        _credits.SetActive(false);
    }
}
