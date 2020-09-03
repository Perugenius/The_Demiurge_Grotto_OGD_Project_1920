using System.Collections.Generic;



namespace Core.SaveLoadData
{
    [System.Serializable]
    public class PlayerData
    {
        public Dictionary<string,int> maxHealth;    //<characterName, characterMaxHealth>
        public Dictionary<string,float> attack;     //<characterName, characterAttack>
        public Dictionary<string, float> attackRange;
        public Dictionary<string, int> secondarySkillLevel;
        public Dictionary<string, float> attackRate;
        public Dictionary<string, int> projectileNumber;
        public Dictionary<string, int> jumpHeight;
        public Dictionary<string, float> speed;
        public Dictionary<string, float> projectileSpeed;
        public Dictionary<string, float> attackDuration;
        public Dictionary<string, int> reanimationLife;
        public int gems;
        public int teammateLetters;
        public int eldaanLetters;
        public string currentCharacter;
        public List<string> unlockedCharacters;
        public string lastSelectedDungeon;
        public List<string> unlockedDungeons;
        public Dictionary<string,Dictionary<string,int>> perks;    //<characterName,<perkName, perkLevel>>
        public bool returningFromDungeon;

        public PlayerData(string firstUnlockedCharacter, List<string> charactersNames)
        {
            currentCharacter = firstUnlockedCharacter;
            unlockedCharacters = new List<string> {firstUnlockedCharacter};
            lastSelectedDungeon = "RunupHills";
            unlockedDungeons = new List<string>{lastSelectedDungeon};
            perks = new Dictionary<string, Dictionary<string, int>>();
            maxHealth = new Dictionary<string, int>();
            attack = new Dictionary<string, float>();
            jumpHeight = new Dictionary<string, int>();
            speed = new Dictionary<string, float>();
            secondarySkillLevel = new Dictionary<string, int>();
            attackRate = new Dictionary<string, float>();
            projectileNumber = new Dictionary<string, int>();
            projectileSpeed = new Dictionary<string, float>();
            attackRange = new Dictionary<string, float>();
            attackDuration = new Dictionary<string, float>();
            reanimationLife = new Dictionary<string, int>();
            returningFromDungeon = false;

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
                jumpHeight.Add(character, 62);
                speed.Add(character, 14);
                reanimationLife.Add(character, 2);
                if (character.Contains("Pinkie") || character.Contains("Kinja") || character.Contains("Steve"))
                {
                    secondarySkillLevel.Add(character, 1);
                    if (character.Contains("Kinja"))
                    {
                        attackRate.Add(character,1f);
                        projectileNumber.Add(character, 1);
                        projectileSpeed.Add(character,16);
                    }

                    if (character.Contains("Pinkie"))
                    {
                        attackRate.Add(character, 1.5f);
                        projectileNumber.Add(character,1);
                        projectileSpeed.Add(character,200);
                        attackDuration.Add(character, 3);
                    }

                    if (character.Contains("Steve"))
                    {
                        attackRate.Add(character,2);
                        attackRange.Add(character, 7);
                        projectileSpeed.Add(character, 10);
                    }
                }
                else if (character.Contains("Voodoo"))
                {
                    attackRate.Add(character,0.5f);
                }
            }
        }
    }
}
