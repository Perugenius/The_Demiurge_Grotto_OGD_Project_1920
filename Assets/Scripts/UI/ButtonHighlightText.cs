using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHighlightText : MonoBehaviour
{
    [SerializeField] private Color highlightedColor;
    [SerializeField] private Color normalColor;
    private EventSystem _eventSystem;
    private Text _buttonText;
    private Image _buttonImage;
    private bool _buttonHasText;
    private bool _isHighlighted;
    private bool _buttonHasImage;

    // Start is called before the first frame update
    void Start()
    {
        _buttonText = transform.GetComponentInChildren<Text>();
        Transform childTransform = transform.Find("icon");
        if(childTransform!=null) _buttonImage = childTransform.GetComponent<Image>();
        _buttonHasText = (_buttonText != null);
        _buttonHasImage = (childTransform != null);
        _eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_eventSystem.currentSelectedGameObject == gameObject && !_isHighlighted)
        {
            if (_buttonHasText)
            {
                _isHighlighted = true;
                _buttonText.color = highlightedColor;
            }

            if (_buttonHasImage)
            {
                _isHighlighted = true;
                _buttonImage.color = highlightedColor;
            }
        }
        if (_eventSystem.currentSelectedGameObject != gameObject && _isHighlighted)
        {
            if (_buttonHasText)
            {
                _isHighlighted = false;
                _buttonText.color = normalColor;
            }
            
            if (_buttonHasImage)
            {
                _isHighlighted = false;
                _buttonImage.color = normalColor;
            }
        }
    }
}
