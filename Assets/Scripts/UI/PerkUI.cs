using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerkUI : MonoBehaviour
{
    public MainMenu MainMenu; 
    
    [SerializeField] private GameObject name;
    [SerializeField] private GameObject description;
    [SerializeField] private GameObject price;

    private string _perkName;
    private int _perkLevel;
    private 
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPerkValues(Perk perk, int level)
    {
        name.GetComponent<Text>().text = perk.name;
        _perkName = perk.name;
        description.GetComponent<Text>().text = "Level " + level + ". " + perk.description;
        _perkLevel = level;
        price.GetComponent<Text>().text = perk.levelCost[level].ToString();
    }

    public void Buy()
    {
        MainMenu.BuyPerk(_perkName, _perkLevel, gameObject);
    }
}
