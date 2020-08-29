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

    // Start is called before the first frame update
    void Start()
    {
        _messageText = messageTextObj.GetComponent<Text>();
        _menu = GetComponent<Menu>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_menu.isFocused() && Input.anyKey && !_menu.DuringTransition && _closable) _menu.Focus(false);
    }

    public void ShowMessage(string message)
    {
        _messageText.text = message;
        _menu.Focus(true);
        StartCoroutine(MessageBoxNotClosable());
    }

    private IEnumerator MessageBoxNotClosable()
    {
        _closable = false;
        yield return new WaitForSeconds(messageBoxMinDuration);
        _closable = true;
    }
}
