using System.Collections.Generic;



namespace Core.SaveLoadData
{
    [System.Serializable]
    public class PlayerData
    {
        public Dictionary<string,int> maxHealth;    //<characterName, characterMaxHealth>
        public Dictionary<string,float> attack;     //<characterName, characterAttack>
        public int gems;
        public int teammateLetters;
        public int eldaanLetters;
        public string currentCharacter;
        public List<string> unlockedCharacters;
        public string lastSelectedDungeon;
        public List<string> unlockedDungeons;
        public Dictionary<string,Dictionary<string,int>> perks;    //<characterName,<perkName, perkLevel>>

        public PlayerData(string firstUnlockedCharacter, List<string> charactersNames)
        {
            currentCharacter = firstUnlockedCharacter;
            unlockedCharacters = new List<string> {firstUnlockedCharacter};
            lastSelectedDungeon = "RunupHills";
            unlockedDungeons = new List<string>{lastSelectedDungeon};
            perks = new Dictionary<string, Dictionary<string, int>>();
            maxHealth = new Dictionary<string, int>();
            attack = new Dictionary<string, float>();
            gems = 0;
            teammateLetters = 0;
            eldaanLetters = 0;
            
            foreach (var character in charactersNames)
            {
                //initialize perks
                perks.Add(character,new Dictionary<string, int>());
                
                //TODO update this value if needed @Ice    
                maxHealth.Add(character,5);
                attack.Add(character,1);
            }
        }
    }
}
