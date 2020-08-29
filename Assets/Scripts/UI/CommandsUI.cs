using UnityEngine;

namespace UI
{
    public class CommandsUI : MonoBehaviour
    {
        private Menu _menu;
        // Start is called before the first frame update
        void Start()
        {
            _menu = GetComponent<Menu>();
        }

        // Update is called once per frame
        void Update()
        {
            if((Input.GetKeyDown("c") || Input.GetKeyDown(KeyCode.Joystick1Button7)) && _menu.isFocused() && !_menu.DuringTransition) _menu.Focus(false);
            if((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button1)) && _menu.isFocused() && !_menu.DuringTransition) _menu.Focus(false);
            if((Input.GetKeyDown("c") || Input.GetKeyDown(KeyCode.Joystick1Button7)) && !_menu.isFocused() && !_menu.DuringTransition) _menu.Focus(true);
        }
    }
}
