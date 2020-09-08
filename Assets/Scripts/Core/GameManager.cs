using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Core.SaveLoadData;
using Mechanics.Camera;
using Mechanics.Collectibles;
using Mechanics.Players;
using Model;
using Photon.Pun;
using UI;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

namespace Core
{
    public class GameManager : Singleton<GameManager>
    {
        [FormerlySerializedAs("camera")] [SerializeField] private GameObject playerCamera;
        [SerializeField] private GameObject loading;
        [SerializeField] private GameObject victoryScreen;
        [SerializeField] private GameObject gameOverScreen;
        [SerializeField] private GameObject disconnectionScreen;
        [SerializeField] private GameObject messageBox;
        [SerializeField] private bool singlePlayerMode;
        [SerializeField] private GameObject gemsHUD;
        [SerializeField] private GameObject lettersHUD;
        [SerializeField] private GameObject attackBar;
        [SerializeField] private GameObject skillBar;
        private int _numOfPlayers = 2;
        private DungeonBuilder _dungeonBuilder;
        private CollectiblesManager _collectiblesManager;
        private bool _playerInstantiated;
        private int _maxBuildTentatives = 2;


        void Awake()
        {
            //List all object pools
        
            //ObjectPoolingManager.Instance.CreatePool (PoolingExample, 100, 200);
        
            //....
        }

        void Start()
        {
            _dungeonBuilder = GetComponent<DungeonBuilder>();
            _collectiblesManager = GameObject.Find("CollectiblesManager").GetComponent<CollectiblesManager>();
            OnJoinScene();
        }

        [PunRPC]
        private void BuildDungeon(string secondCharacter)
        {
            List<string> characters = new List<string>{SaveSystem.LoadPlayerData().currentCharacter,secondCharacter};
            List<DungeonRoom.PlatformingSkills> playersSkills = new List<DungeonRoom.PlatformingSkills>();
            foreach (var character in characters)
            {
                switch (character)
                {
                    case "Voodoo": playersSkills.Add(DungeonRoom.PlatformingSkills.Headstrong); break;
                    case "Kinja": playersSkills.Add(DungeonRoom.PlatformingSkills.DoubleJump); break;
                    case "Pinkie": playersSkills.Add(DungeonRoom.PlatformingSkills.Intangibility); break;
                    case "Steve": playersSkills.Add(DungeonRoom.PlatformingSkills.WallJump); break;
                }
            }

            int type = 1;
            switch (SaveSystem.LoadPlayerData().lastSelectedDungeon)
            {
                case "RunupHills": type = 1;
                    break;
                case "PanicTown": type = 2;
                    break;
                case "MightyWoods": type = 3;
                    break;
                case "StairwayToGrotto": type = 4;
                    break;
            }

            Debug.Log(_maxBuildTentatives);
            
            try
            {
                _dungeonBuilder.BuildDungeon(type,playersSkills,15,2);
            }
            catch (Exception e)
            {
                if (e is ArgumentOutOfRangeException || e is NullReferenceException)
                {
                    Debug.LogError("Error during dungeon creation");
                    if(_maxBuildTentatives <= 0)
                    {
                        ExitDungeon(false);
                        return;
                    }
                    _maxBuildTentatives--;
                    _dungeonBuilder.DestroyDungeon();
                    BuildDungeon(secondCharacter);
                }
                throw;
            }
        }

        public void OnJoinScene()
        {
            if (PhotonNetwork.PlayerList.Length == _numOfPlayers || singlePlayerMode) GetComponent<PhotonView>().RPC("BuildDungeon", RpcTarget.MasterClient, SaveSystem.LoadPlayerData().currentCharacter);
        }

        [PunRPC]
        public void InstantiatePlayer()
        {
            PlayerData playerData = SaveSystem.LoadPlayerData();
            GameObject player = PhotonNetwork.Instantiate(Path.Combine("Players",playerData.currentCharacter), new Vector3((PhotonNetwork.IsMasterClient)?-7f:-10f, 5f, 0f), Quaternion.identity);
            playerCamera.GetComponent<CameraFocusOnPlayer>().cameraPlayer = player;
            HealthBar healthBar = playerCamera.transform.Find("Canvas").Find("Health").GetComponent<HealthBar>();
            PlayableCharacter playerScript = player.GetComponent<PlayableCharacter>();
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
            loading.SetActive(false);
            
            //play music
            AudioManager.Instance.PlaySound(playerData.lastSelectedDungeon);
        }

        void Update()
        {
            if(!PhotonNetwork.IsMasterClient) return;
            if (_dungeonBuilder.DungeonReady && _playerInstantiated == false)
            {
                GetComponent<PhotonView>().RPC("InstantiatePlayer", RpcTarget.All);
                _playerInstantiated = true;
            }
            
            //debug
            if(Input.GetKeyDown(KeyCode.Escape)) ExitDungeon(false);
        }

        private void ExitDungeon(bool isGameOver)
        {
            //stop music
            AudioManager.Instance.StopSound(SaveSystem.LoadPlayerData().lastSelectedDungeon);
            
            if(!isGameOver) _collectiblesManager.SaveCollectibles();
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            //PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }

        public void ShowVictoryScreen()
        {
            victoryScreen.SetActive(true);
            StartCoroutine(WaitBeforeExit(false));
        }

        public void ShowGameOverScreen()
        {
            if(victoryScreen.activeSelf) return;
            gameOverScreen.SetActive(true);
            StartCoroutine(WaitBeforeExit(true));
        }
        
        public void ShowDisconnectionScreen()
        {
            if(victoryScreen.activeSelf) return;
            if(gameOverScreen.activeSelf) return;
            disconnectionScreen.SetActive(true);
            StartCoroutine(WaitBeforeExit(true));
        }

        private IEnumerator WaitBeforeExit(bool isGameOver)
        {
            yield return new WaitForSeconds(5);
            ExitDungeon(isGameOver);
        }

        public void ShowMessage(string message)
        {
            messageBox.SetActive(true);
            messageBox.GetComponent<MessageBox>().ShowMessage(message);
        }
    }
}
