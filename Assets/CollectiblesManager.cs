using System.Collections;
using System.Collections.Generic;
using Core.SaveLoadData;
using UnityEngine;
using UnityEngine.AI;

public class CollectiblesManager : MonoBehaviour
{
    private int _gems;
    private int _teammateLetters;
    private int _eldaanLetters;

    public void CollectGem(int value)
    {
        _gems += value;
        Debug.Log("Gems: " + _gems);
    }
    
    public void CollectTeammateLetter()
    {
        _teammateLetters += 1;
        Debug.Log("Teammate letters: " + _gems);
    }
    
    public void CollectEldaanLetter()
    {
        _eldaanLetters += 1;
        Debug.Log("Eldaan letters: " + _gems);
    }

    public void SaveCollectibles()
    {
        PlayerData playerData = SaveSystem.LoadPlayerData();
        playerData.gems += _gems;
        playerData.teammateLetters += _teammateLetters;
        playerData.eldaanLetters += _eldaanLetters;
        SaveSystem.SavePlayerData(playerData);
    }
}
