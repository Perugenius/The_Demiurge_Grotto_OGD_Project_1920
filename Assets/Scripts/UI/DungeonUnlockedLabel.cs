using System.Collections;
using System.Collections.Generic;
using Core.SaveLoadData;
using UnityEngine;

public class DungeonUnlockedLabel : MonoBehaviour
{
    [SerializeField] private Transform dungeon1Position;
    [SerializeField] private Transform dungeon2Position;
    [SerializeField] private Transform dungeon3Position;
    [SerializeField] private Transform dungeon4Position;
    // Start is called before the first frame update
    void Start()
    {
        switch (SaveSystem.LoadPlayerData().eldaanLetters)
        {
            case MainMenu.dungeon2LettersThreshold:
                gameObject.transform.position = dungeon2Position.position; break;
            case MainMenu.dungeon3LettersThreshold:
                gameObject.transform.position = dungeon3Position.position; break;
            case MainMenu.dungeon4LettersThreshold:
                gameObject.transform.position = dungeon4Position.position; break;
            default: gameObject.SetActive(false); break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
