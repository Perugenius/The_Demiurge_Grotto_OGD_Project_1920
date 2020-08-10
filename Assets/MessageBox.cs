using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    [SerializeField] private GameObject messageTextObj;
    private Text _messageText;
    private Menu _menu;
    
    // Start is called before the first frame update
    void Start()
    {
        _messageText = messageTextObj.GetComponent<Text>();
        _menu = GetComponent<Menu>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_menu.isFocused() && Input.anyKey && !_menu.DuringTransition) _menu.Focus(false);
    }

    public void ShowMessage(string message)
    {
        _messageText.text = message;
        _menu.Focus(true);
    }
}
