﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    [SerializeField] private GameObject messageTextObj;
    [SerializeField] private float messageBoxMinDuration = 1.2f;
    private Text _messageText;
    private Menu _menu;
    private bool _closable = false;
    private List<string> _que;

    // Start is called before the first frame update
    void Start()
    {
        _messageText = messageTextObj.GetComponent<Text>();
        _menu = GetComponent<Menu>();
        _que = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_menu.isFocused() && Input.anyKey && !_menu.DuringTransition && _closable) _menu.Focus(false);
    }

    private IEnumerator CloseMessage()
    {
        _menu.Focus(false);
        while (_menu.DuringTransition)
        {
            yield return new WaitForEndOfFrame();
        }
        string message = _que[0];
        _que.Remove(message);
        ShowMessage(message);
    }

    public void ShowMessage(string message)
    {
        Debug.Log("Show message");
        if (_menu.isFocused())
        {
            _que.Add(message);
            StartCoroutine(CloseMessage());
        }
        else
        {
            _messageText.text = message;
            _menu.Focus(true);
            StartCoroutine(MessageBoxNotClosable());
        }
    }

    private IEnumerator MessageBoxNotClosable()
    {
        _closable = false;
        yield return new WaitForSeconds(messageBoxMinDuration);
        _closable = true;
    }
}
