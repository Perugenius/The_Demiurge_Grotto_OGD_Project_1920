using System;
using Core;
using UnityEngine;

public class MessageTrigger : MonoBehaviour
{
    [SerializeField] private string messageText;
    private TutorialManager _tutorialManager;
    private bool _alreadyTriggered = false;

    // Start is called before the first frame update
    void Start()
    {
        _tutorialManager = GameObject.Find("TutorialManager").GetComponent<TutorialManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_alreadyTriggered)
        {
            _alreadyTriggered = true;
            _tutorialManager.ShowMessage(messageText);
            gameObject.SetActive(false);
        }
    }
}
