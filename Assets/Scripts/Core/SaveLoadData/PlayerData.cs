using System.Collections.Generic;



namespace Core.SaveLoadData
{
    [System.Serializable]
    public class PlayerData
    {
        public int maxHealth;
        public int attack;
        public int gems;
        public int teammateLetters;
        public int eldaanLetters;
        public string currentCharacter;
        public List<string> unlockedCharacters;
        public string lastSelectedDungeon;
        public List<string> unlockedDungeons;
        public Dictionary<string,Dictionary<string,int>> perks;    //<characterName,<perkName, perkLevel>>

        public PlayerData(string firstUnlockedCharacter)
        {
            currentCharacter = firstUnlockedCharacter;
            unlockedCharacters = new List<string> {firstUnlockedCharacter};
            lastSelectedDungeon = "RunupHills";
            unlockedDungeons = new List<string>{lastSelectedDungeon};
            perks = new Dictionary<string, Dictionary<string, int>>();
        }
    }
}
