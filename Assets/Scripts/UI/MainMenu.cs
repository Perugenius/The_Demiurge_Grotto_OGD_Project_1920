using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject home;
    [SerializeField] private GameObject backgroundHills;
    [SerializeField] private GameObject dungeons;
    [SerializeField] private GameObject perksShop;
    [SerializeField] private GameObject characters;
    [SerializeField] private GameObject credits;

    private Menu _titleMenu;
    private Menu _hillsMenu;
    private Menu _homeMenu;
    private Menu _dungeonMenu;
    private Menu _perksMenu;
    private Menu _charactersMenu;
    private Menu _creditsMenu;

    // Start is called before the first frame update
    void Start()
    {
        _titleMenu = title.GetComponent<Menu>();
        _hillsMenu = backgroundHills.GetComponent<Menu>();
        _homeMenu = home.GetComponent<Menu>();
        _dungeonMenu = dungeons.GetComponent<Menu>();
        _perksMenu = perksShop.GetComponent<Menu>();
        _charactersMenu = characters.GetComponent<Menu>();
        _creditsMenu = credits.GetComponent<Menu>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_titleMenu.isFocused() && Input.anyKey) ShowHome();
    }

    public void ShowHome()
    {
        _titleMenu.Focus(false);
        _hillsMenu.Focus(false);
        
        _dungeonMenu.Focus(false);
        _charactersMenu.Focus(false);
        _perksMenu.Focus(false);
        _creditsMenu.Focus(false);
        
        _homeMenu.Focus(true);
    }

    public void ShowDungeonsMenu()
    {
        _homeMenu.Focus(false);
        _dungeonMenu.Focus(true);
    }
    
    public void ShowPerksMenu()
    {
        _homeMenu.Focus(false);
        _perksMenu.Focus(true);
    }
    
    public void ShowCharactersMenu()
    {
        _homeMenu.Focus(false);
        _charactersMenu.Focus(true);
    }

    public void Play()
    {
        SceneManager.LoadScene("NetworkSetup");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
