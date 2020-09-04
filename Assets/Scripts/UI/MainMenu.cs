using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.SaveLoadData;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public delegate void PerkCback(string character, int level); // callback for executing perks;

public class Perk
{
    public string name;
    public string character;
    public string description;
    public Dictionary<int, int> levelCost; //<perkLevel, cost>
    public PerkCback runPerk; //function to execute perk and persistence it

    public Perk(string name,string character)
    {
        this.name = name;
        this.character = character;
    }
}

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject home;
    [SerializeField] private GameObject backgroundHills;
    [SerializeField] private GameObject dungeons;
    [SerializeField] private GameObject perksShop;
    [SerializeField] private GameObject characters;
    [SerializeField] private GameObject credits;
    [SerializeField] private GameObject perkUI;
    [SerializeField] private RectTransform firstPerkPosition;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color selectedColor;
    [SerializeField] private float verticalPerksOffset;

    private Menu _titleMenu;
    private Menu _hillsMenu;
    private Menu _homeMenu;
    private Menu _dungeonMenu;
    private Menu _perksMenu;
    private Menu _charactersMenu;
    private Menu _creditsMenu;

    public const int dungeon2LettersThreshold = 10;
    public const int dungeon3LettersThreshold = 20;
    public const int dungeon4LettersThreshold = 30;

    private const int characterPrice = 10;

    private List<Perk> perks;

    private List<string> charactersNames;
    private EventSystem _eventSystem;
    private bool _returnFromDungeon;

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
        charactersNames = new List<string>{"Voodoo","Kinja","Pinkie","Steve"};
        
        //Randomly choose first character
        if(SaveSystem.LoadPlayerData()==null)
        {
            SaveSystem.CreatePlayerData(charactersNames[Random.Range(0,charactersNames.Count)], charactersNames);
        }

        LoadPerks();

        PlayerData playerData = SaveSystem.LoadPlayerData();
        playerData.gems = 1200;
        playerData.teammateLetters = 40;
        //playerData.eldaanLetters = 10;
        
        try
        {
            _returnFromDungeon = playerData.returningFromDungeon;
            playerData.returningFromDungeon = false;
        }
        catch (NullReferenceException e)
        {
            _returnFromDungeon = false;
        }
        
        SaveSystem.SavePlayerData(playerData);
    }

    // Update is called once per frame
    void Update()
    {
        //enter to main menu
        if (_titleMenu.isFocused() && (Input.anyKey || _returnFromDungeon)) ShowHome();

        //Debug values on console
        if (Input.GetKeyDown("l")) 
        {
            PlayerData playerData = SaveSystem.LoadPlayerData();
            Debug.Log(playerData.currentCharacter + " maxHealth = " + playerData.maxHealth[playerData.currentCharacter]);
            Debug.Log(playerData.currentCharacter + " attack = " +  playerData.attack[playerData.currentCharacter]);
        }
        
        //Close menu with "B" from joystick
        if(_charactersMenu.isFocused() && !_charactersMenu.DuringTransition && Input.GetKeyDown(KeyCode.Joystick1Button1)) ShowHome();
        if(_dungeonMenu.isFocused() && !_dungeonMenu.DuringTransition && Input.GetKeyDown(KeyCode.Joystick1Button1)) ShowHome();
        if(_perksMenu.isFocused() && !_perksMenu.DuringTransition && Input.GetKeyDown(KeyCode.Joystick1Button1)) ShowHome();
    }

    public void ShowHome()
    {
        if(UITransition()) return;
        _titleMenu.Focus(false);
        _hillsMenu.Focus(false);
        
        _dungeonMenu.Focus(false);
        _charactersMenu.Focus(false);
        _perksMenu.Focus(false);
        _creditsMenu.Focus(false);
        
        _homeMenu.gameObject.SetActive(true);
        _homeMenu.Focus(true);
    }

    /// <summary>
    /// It returns true if a UI transition is being performed.
    /// This method used to disable input during UI transitions
    /// </summary>
    /// <returns></returns>
    private bool UITransition()
    {
        return (_homeMenu.DuringTransition || _charactersMenu.DuringTransition || _perksMenu.DuringTransition
                ||_dungeonMenu.DuringTransition);
    }

    public void ShowDungeonsMenu()
    {
        if(UITransition()) return;
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
        if(UITransition()) return;
        _homeMenu.Focus(false);
        _perksMenu.gameObject.SetActive(true);
        _perksMenu.Focus(true);

        PlayerData playerData = SaveSystem.LoadPlayerData();
        int i = 0;
        List<GameObject> dynamicContent = new List<GameObject>();
        
        foreach (var perk in perks)
        {
            if (perk.character == playerData.currentCharacter)
            {
                Vector2 firstPosition = firstPerkPosition.position;
                RectTransform rectTransform = firstPerkPosition;
                rectTransform.position = firstPosition;
                GameObject perkUiInstance = Instantiate(perkUI, rectTransform);
                perkUiInstance.transform.SetParent(perksShop.transform,false);
                perkUiInstance.transform.position -= new Vector3(0,i*verticalPerksOffset,0);
                perkUiInstance.GetComponent<PerkUI>().MainMenu = this;
                dynamicContent.Add(perkUiInstance);
                
                UpdatePerks(playerData, perk, perkUiInstance);
                
                i++;
            }
        }
        
        _perksMenu.SetContent(dynamicContent);
    }

    public void UpdatePerks(PlayerData playerData, Perk perk, GameObject perkUiInstance)
    {
        List<string> perkList = playerData.perks[playerData.currentCharacter].Keys.ToList();
        int perkLevel = perkList.Contains(perk.name)?playerData.perks[playerData.currentCharacter][perk.name]:0;
        int levelLable = 0;
        if (perkLevel == 0) levelLable = 1;
        else
        {
            List<int> levelsList = perk.levelCost.Keys.ToList();
            if (levelsList.IndexOf(playerData.perks[playerData.currentCharacter][perk.name]) !=
                levelsList.Count - 1) levelLable = perkLevel + 1;
            else
            {
                levelLable = perkLevel;
                perkUiInstance.transform.Find("Buy").GetComponent<Button>().interactable = false;
            }
        }
        perkUiInstance.GetComponent<PerkUI>().SetPerkValues(perk,levelLable);
    }

    public void BuyPerk(string perkName, int level, GameObject perkUi)
    {
        PlayerData playerData = SaveSystem.LoadPlayerData();
        foreach (var perk in perks)
        {
            if (perk.name == perkName && perk.character == playerData.currentCharacter && perk.levelCost[level] <= playerData.gems)
            {
                playerData.gems -= perk.levelCost[level];
                List<string> alreadyBought = playerData.perks[playerData.currentCharacter].Keys.ToList();
                if (alreadyBought.Contains(perkName)) playerData.perks[playerData.currentCharacter][perkName] = level;
                else playerData.perks[playerData.currentCharacter].Add(perkName,level);
                
                SaveSystem.SavePlayerData(playerData);
                
                perk.runPerk(playerData.currentCharacter,level); //execute perk

                UpdatePerks(playerData,perk,perkUi);
            }
        }
    }
    
    public void ShowCharactersMenu()
    {
        if(UITransition()) return;
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
    
    private void LoadPerks()
    {
        perks = new List<Perk>();
        foreach (var character in charactersNames)
        {
            //Max Health perk
            Perk healthPerk = new Perk("Maximum Health",character);
            Dictionary<int,int> healthLevelCost = new Dictionary<int, int>
            {
                {1,200},
                {2,300},
                {3,500},
                {4,600},
                {5,800},
            };
            healthPerk.levelCost = healthLevelCost;
            healthPerk.description = "Increase " + character + " maximum health.";
            healthPerk.runPerk = (characterName, level) =>
            {
                PlayerData playerData = SaveSystem.LoadPlayerData();
                playerData.maxHealth[characterName] += 1;
                SaveSystem.SavePlayerData(playerData);
            };
            
            //Attack perk
            Perk attackPerk = new Perk("Attack",character);
            Dictionary<int,int> attackLevelCost = new Dictionary<int, int>
            {
                {1,200},
                {2,300},
                {3,500},
                {4,600},
                {5,800},
            };
            attackPerk.levelCost = attackLevelCost;
            attackPerk.description = "Increase " + character + " attack damage.";
            attackPerk.runPerk = (characterName, level) =>
            {
                PlayerData playerData = SaveSystem.LoadPlayerData();
                playerData.attack[characterName] += 0.5f;
                SaveSystem.SavePlayerData(playerData);
            };

            perks.Add(healthPerk);
            perks.Add(attackPerk);
            
            Perk attackRate = new Perk("Rate of fire", character);
            Dictionary<int, int> attackRateCost = new Dictionary<int, int>
            {
                {1, 200},
                {2, 300},
                {3, 500},
                {4, 600},
                {5, 800}
            };

            attackRate.levelCost = attackRateCost;
            attackRate.description = "Increase " + character + " rate of fire";
            attackRate.runPerk = (s, level) =>
            {
                PlayerData playerData = SaveSystem.LoadPlayerData();
                playerData.attackRate[s] -= 0.3f;
                SaveSystem.SavePlayerData(playerData);
            };

            if (character.Contains("Voodoo"))
            {
                perks.Add(attackRate);
                
                Perk reanimationLifePerk = new Perk("Reanimation Life", character);
                Dictionary<int, int> reanimationLifeCost = new Dictionary<int, int>
                {
                    {1, 200},
                    {2, 300},
                    {3, 500},
                    {4, 600},
                    {5, 800}
                };
                reanimationLifePerk.levelCost = reanimationLifeCost;
                reanimationLifePerk.description = "Increase reanimation life";
                reanimationLifePerk.runPerk = (s, level) =>
                {
                    PlayerData playerData = SaveSystem.LoadPlayerData();
                    playerData.reanimationLife[s] += 1;
                    SaveSystem.SavePlayerData(playerData);
                };
                perks.Add(reanimationLifePerk);
            }

            else
            {

                Perk secondarySkillPerk = new Perk("Skill level",character);
                Dictionary<int, int> secondarySkillCost = new Dictionary<int, int>
                {
                    {1, 200},
                    {2, 600},
                    {3, 1000},
                };
                secondarySkillPerk.levelCost = secondarySkillCost;
                secondarySkillPerk.runPerk = (s, level) =>
                {
                    PlayerData playerData = SaveSystem.LoadPlayerData();
                    playerData.secondarySkillLevel[s] += 1;
                    SaveSystem.SavePlayerData(playerData);
                };

                if (character.Contains("Pinkie") || character.Contains("Kinja"))
                {
                    Perk numberOfProjectiles = new Perk("Projectiles number", character);
                    Dictionary<int, int> numberOfProjectileCost;
                    numberOfProjectiles.description = "Increase " + character + " number of projectiles";
                    numberOfProjectiles.runPerk = (characterName, level) =>
                    {
                        PlayerData playerData = SaveSystem.LoadPlayerData();
                        playerData.projectileNumber[characterName] += 1;
                        SaveSystem.SavePlayerData(playerData);
                    };
                    if (character.Contains("Pinkie"))
                    {
                        numberOfProjectileCost = new Dictionary<int, int>
                        {
                            {1, 300},
                            {2, 600},
                            {3, 900}
                        };
                        
                        secondarySkillPerk.description = "Increase invulnerability duration";
                        
                        Perk attackDurationPerk = new Perk("Attack duration",character);
                        Dictionary<int, int> attackDurationCost = new Dictionary<int, int>
                        {
                            {1, 200},
                            {2, 300},
                            {3, 600},
                            {4, 800},
                        };
                        attackDurationPerk.levelCost = attackDurationCost;
                        attackDurationPerk.description = "Increase the duration of the projectiles";
                        attackDurationPerk.runPerk = (s, level) =>
                        {
                            PlayerData playerData = SaveSystem.LoadPlayerData();
                            playerData.attackDuration[s] += 0.4f;
                            SaveSystem.SavePlayerData(playerData);
                        };
                        perks.Add(attackDurationPerk);
                    }

                    else
                    {
                        
                        numberOfProjectileCost = new Dictionary<int, int>
                        {
                            {1, 200},
                            {2, 300},
                            {3, 600},
                            {4, 800},
                        };
                        secondarySkillPerk.description = "Raise number of consecutive jumps";
                        perks.Add(attackRate);
                    }
                    
                    numberOfProjectiles.levelCost = numberOfProjectileCost;
                    perks.Add(numberOfProjectiles);
                }

                if (character.Contains("Steve"))
                {
                    perks.Add(attackRate);
                    secondarySkillPerk.description = "Raise the friction on the wall";
                    
                    Perk attackRange = new Perk("Attack Range", character);
                    Dictionary<int, int> attackRangeCost = new Dictionary<int, int>
                    {
                        {1, 200},
                        {2, 300},
                        {3, 500},
                        {4, 600},
                        {5, 800}
                    };
                    attackRange.levelCost = attackRangeCost;
                    attackRange.description = "Increase laser range";
                    attackRange.runPerk = (s, level) =>
                    {
                        PlayerData playerData = SaveSystem.LoadPlayerData();
                        playerData.attackRange[s] += 0.3f;
                        SaveSystem.SavePlayerData(playerData);
                    };
                }
                perks.Add(secondarySkillPerk);
            }
            
        }
        
    }
}
