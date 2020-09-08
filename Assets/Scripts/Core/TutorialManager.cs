using System.Collections;
using System.Collections.Generic;
using Core.SaveLoadData;
using Mechanics.Camera;
using Mechanics.Collectibles;
using Mechanics.Players;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> mainCharacters;
        [SerializeField] private GameObject playerCamera;
        [SerializeField] private GameObject victoryScreen;
        [SerializeField] private GameObject messageBox;
        [SerializeField] private GameObject gemsHUD;
        [SerializeField] private GameObject lettersHUD;
        [SerializeField] private GameObject attackBar;
        [SerializeField] private GameObject voodooRoom;
        [SerializeField] private GameObject kinjaRoom;
        [SerializeField] private GameObject steveRoom;
        [SerializeField] private GameObject pinkieRoom;
        [SerializeField] private GameObject skillBar;
        private CollectiblesManager _collectiblesManager;
        private GameObject _currentPlayer;

        
        // Start is called before the first frame update
        void Start()
        {
            _collectiblesManager = GameObject.Find("CollectiblesManager").GetComponent<CollectiblesManager>();

            ActiveCurrentPlayer();

            ActiveSkillRoom();

            playerCamera.GetComponent<CameraFocusOnPlayer>().cameraPlayer = _currentPlayer;
            
            //setup HUD
            HealthBar healthBar = playerCamera.transform.Find("Canvas").Find("Health").GetComponent<HealthBar>();
            PlayableCharacter playerScript = _currentPlayer.GetComponent<PlayableCharacter>();
            healthBar.Character = playerScript;
            playerScript.HealthBar1 = healthBar;
            playerScript.AttackBar1 = attackBar.GetComponent<Bar>();
            if (playerScript is Pinkie pinkie)
            {
                pinkie.SkillBar = skillBar.GetComponent<Bar>();
            }
            else
            {
                skillBar.SetActive(false);
            }
            gemsHUD.SetActive(true);
            lettersHUD.SetActive(true);
            
            //play music
            AudioManager.Instance.PlaySound("RunupHills");
        }

        private void ActiveSkillRoom()
        {
            switch (_currentPlayer.name)
            {
                case "Voodoo": voodooRoom.SetActive(true); break;
                case "Kinja": kinjaRoom.SetActive(true); break;
                case "Steve": steveRoom.SetActive(true); break;
                case "Pinkie": pinkieRoom.SetActive(true); break;
            }
        }

        private void ActiveCurrentPlayer()
        {
            foreach (var character in mainCharacters)
            {
                if(character.name == SaveSystem.LoadPlayerData().currentCharacter)
                {
                    character.SetActive(true);
                    character.GetComponent<PlayableCharacter>().localTesting = true;
                    _currentPlayer = character;
                    //TODO call spawn animation
                }
            }
        }

        public GameObject GetCurrentCharacter()
        {
            return _currentPlayer;
        }
        
        public List<GameObject> GetAllCharacters()
        {
            return mainCharacters;
        }
        
        public void ShowMessage(string message)
        {
            messageBox.SetActive(true);
            messageBox.GetComponent<MessageBox>().ShowMessage(message);
        }
        
        public void ShowVictoryScreen()
        {
            victoryScreen.SetActive(true);
            StartCoroutine(WaitBeforeExit());
        }
        
        private IEnumerator WaitBeforeExit()
        {
            yield return new WaitForSeconds(5);
            
            //stop music
            AudioManager.Instance.StopSound("RunupHills");
            
            PlayerData playerData = SaveSystem.LoadPlayerData();
            playerData.returningFromDungeon = true;
            SaveSystem.SavePlayerData(playerData);
            SceneManager.LoadScene("MainMenu");
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
