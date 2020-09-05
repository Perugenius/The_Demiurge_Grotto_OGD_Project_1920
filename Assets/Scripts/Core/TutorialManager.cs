using System.Collections.Generic;
using Core.SaveLoadData;
using UnityEngine;

namespace Core
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> mainCharacters;
        
        // Start is called before the first frame update
        void Start()
        {
            ActiveCurrentPlayer();
        }

        private void ActiveCurrentPlayer()
        {
            foreach (var character in mainCharacters)
            {
                if(character.name == SaveSystem.LoadPlayerData().currentCharacter)
                {
                    character.SetActive(true);
                    //TODO call spawn animation
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
