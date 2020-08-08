using System.Collections;
using System.Collections.Generic;
using Core.SaveLoadData;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject home;
    [SerializeField] private GameObject backgroundHills;
    [SerializeField] private GameObject dungeons;
    [SerializeField] private GameObject perksShop;
    [SerializeField] private GameObject characters;
    [SerializeField] private GameObject credits;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color selectedColor;

    private Menu _titleMenu;
    private Menu _hillsMenu;
    private Menu _homeMenu;
    private Menu _dungeonMenu;
    private Menu _perksMenu;
    private Menu _charactersMenu;
    private Menu _creditsMenu;

    private const int dungeon2LettersThreshold = 10;
    private const int dungeon3LettersThreshold = 20;
    private const int dungeon4LettersThreshold = 30;

    private const int characterPrice = 10;

    private List<string> charactersNames;
    private EventSystem _eventSystem;

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
        
        _eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();

        //characters names list
        charactersNames = new List<string>{"Vodoo","Kinja","Pinkie","Steve"};
        
        //Randomly choose first character
        if(SaveSystem.LoadPlayerData()==null)
        {
            SaveSystem.CreatePlayerData(charactersNames[Random.Range(0,charactersNames.Count-1)]);
        }

        /*PlayerData playerData = SaveSystem.LoadPlayerData();
        playerData.teammateLetters = 23;
        playerData.unlockedCharacters.Clear();
        SaveSystem.SavePlayerData(playerData);*/
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
        
        _homeMenu.gameObject.SetActive(true);
        _homeMenu.Focus(true);
    }

    public void ShowDungeonsMenu()
    {
        _homeMenu.Focus(false);
        _dungeonMenu.gameObject.SetActive(true);
        _dungeonMenu.Focus(true);
        PlayerData playerData = SaveSystem.LoadPlayerData();
        
        TryUnlockDungeon(playerData);
        
        foreach (var dungeon in playerData.unlockedDungeons)
        {
            GameObject button = _dungeonMenu.gameObject.transform.Find(dungeon).gameObject;
            if (button != null)
            {
                button.GetComponent<Button>().interactable = true;
                if (dungeon == playerData.lastSelectedDungeon)
                {
                    var colors = button.GetComponent<Button> ().colors;
                    colors.normalColor = selectedColor;
                    button.GetComponent<Button> ().colors = colors;
                }
            }
        }
    }

    private void TryUnlockDungeon(PlayerData playerData)
    {
        if(playerData.eldaanLetters >= dungeon2LettersThreshold && !playerData.unlockedDungeons.Contains("PanicTown"))
            playerData.unlockedDungeons.Add("PanicTown");
        if(playerData.eldaanLetters >= dungeon3LettersThreshold && !playerData.unlockedDungeons.Contains("MightyWoods")) 
            playerData.unlockedDungeons.Add("MightyWoods");
        if(playerData.eldaanLetters >= dungeon4LettersThreshold && !playerData.unlockedDungeons.Contains("StairwayToGrotto")) 
            playerData.unlockedDungeons.Add("StairwayToGrotto");
        SaveSystem.SavePlayerData(playerData);
    }

    public void SelectDungeon(string dungeon)
    {
        PlayerData playerData = SaveSystem.LoadPlayerData();
        playerData.lastSelectedDungeon = dungeon;
        SaveSystem.SavePlayerData(playerData);
        GameObject button = _dungeonMenu.gameObject.transform.Find(dungeon).gameObject;

        if (button != null)
        {
            var colors = button.GetComponent<Button> ().colors;
            colors.normalColor = selectedColor;
            button.GetComponent<Button> ().colors = colors;
        }
        
        foreach (var dun in playerData.unlockedDungeons)
        {
            GameObject dunButton = _dungeonMenu.gameObject.transform.Find(dun).gameObject;
            if (dunButton != null && dun!=dungeon)
            {
                var colors = dunButton.GetComponent<Button> ().colors;
                colors.normalColor = normalColor;
                dunButton.GetComponent<Button> ().colors = colors;
            }
        }
    }
    
    public void ShowPerksMenu()
    {
        _homeMenu.Focus(false);
        _perksMenu.gameObject.SetActive(true);
        _perksMenu.Focus(true);
    }
    
    public void ShowCharactersMenu()
    {
        _homeMenu.Focus(false);
        _charactersMenu.gameObject.SetActive(true);
        _charactersMenu.Focus(true);
        PlayerData playerData = SaveSystem.LoadPlayerData();
        
        foreach (var character in playerData.unlockedCharacters)
        {
            GameObject selectButton = _charactersMenu.gameObject.transform.Find(character).Find(character).gameObject;
            GameObject unlockButton = _charactersMenu.gameObject.transform.Find(character).Find("unlock"+character).gameObject;
            if (selectButton != null)
            {
                selectButton.GetComponent<Button>().interactable = true;
                unlockButton.GetComponent<Button>().interactable = false;
                if (character == playerData.currentCharacter)
                {
                    var colors = selectButton.GetComponent<Button> ().colors;
                    colors.normalColor = selectedColor;
                    selectButton.GetComponent<Button> ().colors = colors;
                }
            }
        }
    }
    
    public void SelectCharacter(string character)
    {
        PlayerData playerData = SaveSystem.LoadPlayerData();
        playerData.currentCharacter = character;
        SaveSystem.SavePlayerData(playerData);
        GameObject button = _charactersMenu.gameObject.transform.Find(character).Find(character).gameObject;

        if (button != null)
        {
            var colors = button.GetComponent<Button> ().colors;
            colors.normalColor = selectedColor;
            button.GetComponent<Button> ().colors = colors;
        }
        
        foreach (var otherChar in playerData.unlockedCharacters)
        {
            GameObject charButton = _charactersMenu.gameObject.transform.Find(otherChar).Find(otherChar).gameObject;
            if (charButton != null && otherChar!=character)
            {
                var colors = charButton.GetComponent<Button> ().colors;
                colors.normalColor = normalColor;
                charButton.GetComponent<Button> ().colors = colors;
            }
        }
    }

    public void UnlockCharacter(string character)
    {
        PlayerData playerData = SaveSystem.LoadPlayerData();
        if (playerData.teammateLetters >= characterPrice)
        {
            playerData.teammateLetters -= characterPrice;
            playerData.unlockedCharacters.Add(character);
            SaveSystem.SavePlayerData(playerData);
        }
        
        foreach (var otherChar in playerData.unlockedCharacters)
        {
            GameObject selectButton = _charactersMenu.gameObject.transform.Find(otherChar).Find(otherChar).gameObject;
            GameObject unlockButton = _charactersMenu.gameObject.transform.Find(otherChar).Find("unlock"+otherChar).gameObject;
            if (selectButton != null)
            {
                selectButton.GetComponent<Button>().interactable = true;
                unlockButton.GetComponent<Button>().interactable = false;
            }
        }

        _eventSystem.SetSelectedGameObject(_charactersMenu.transform.Find(character).Find(character).gameObject);
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
