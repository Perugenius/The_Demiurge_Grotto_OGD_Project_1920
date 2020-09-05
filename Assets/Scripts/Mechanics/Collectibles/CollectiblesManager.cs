﻿using Core.SaveLoadData;
using UnityEngine;
using UnityEngine.UI;

namespace Mechanics.Collectibles
{
    public class CollectiblesManager : MonoBehaviour
    {
        public int Gems => _gems;

        public int TeammateLetters => _teammateLetters;

        public int EldaanLetters => _eldaanLetters;

        public Text gemsHUD;
        public Text teammateLettersHUD;

        private int _gems;
        private int _teammateLetters;
        private int _eldaanLetters;

        public void CollectGem(int value)
        {
            _gems += value;
            gemsHUD.text = _gems.ToString();
            Debug.Log("Gems: " + _gems);
        }
    
        public void CollectTeammateLetter()
        {
            _teammateLetters += 1;
            teammateLettersHUD.text = _teammateLetters.ToString();
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
            playerData.returningFromDungeon = true;
            SaveSystem.SavePlayerData(playerData);
        }
    }
}
