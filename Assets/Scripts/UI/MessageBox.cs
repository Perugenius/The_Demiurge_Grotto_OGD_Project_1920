using System.Collections;
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
        if(_menu.DuringTransition)
        {
            StartCoroutine(WaitDuringTransition(message));
            return;
        }
        if (_menu.isFocused())
        {
            _menu.Focus(false);
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

    private IEnumerator WaitDuringTransition(string message)
    {
        while (_menu.DuringTransition)
        {
            yield return new WaitForEndOfFrame();
        }
        ShowMessage(message);
    }

    private IEnumerator MessageBoxNotClosable()
    {
        _closable = false;
        yield return new WaitForSeconds(messageBoxMinDuration);
        _closable = true;
    }
}
