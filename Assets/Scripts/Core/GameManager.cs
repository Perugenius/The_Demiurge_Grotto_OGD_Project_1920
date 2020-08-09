using System.Collections.Generic;
using System.IO;
using Core.SaveLoadData;
using Mechanics.Camera;
using Model;
using Photon.Pun;
using UnityEngine;

namespace Core
{
    public class GameManager : Singleton<GameManager>
    {
        public GameObject camera;
        public GameObject loading;
        private int _numOfPlayers = 2;
        private DungeonBuilder _dungeonBuilder;
        private GameObject _myCamera;
        private CollectiblesManager _collectiblesManager;

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
                    case "Vodoo": playersSkills.Add(DungeonRoom.PlatformingSkills.Headstrong); break;
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

            GetComponent<DungeonBuilder>().BuildDungeon(type,playersSkills,15,2);
        }

        public void OnJoinScene()
        {
            if (PhotonNetwork.PlayerList.Length == _numOfPlayers) GetComponent<PhotonView>().RPC("BuildDungeon", RpcTarget.MasterClient, SaveSystem.LoadPlayerData().currentCharacter);
            _myCamera = Instantiate(camera, new Vector3(0f, 0f, -10f), Quaternion.identity);
            int i = 0;
        }

        [PunRPC]
        public void InstantiatePlayer()
        {
            GameObject player = PhotonNetwork.Instantiate(Path.Combine("Players","Voodoo"), new Vector3((PhotonNetwork.IsMasterClient)?-7f:-10f, 5f, 0f), Quaternion.identity);
            _myCamera.GetComponent<CameraFocusOnPlayer>().cameraPlayer = player;
            loading.SetActive(false);
        }

        void Update()
        {
            if(!PhotonNetwork.IsMasterClient) return;
            if (_dungeonBuilder.dungeonReady)
            {
                _dungeonBuilder.dungeonReady = false;
                GetComponent<PhotonView>().RPC("InstantiatePlayer", RpcTarget.All);
            }
        }

        public void ExitDungeon()
        {
            _collectiblesManager.SaveCollectibles();
            
            //TODO log victory message
            
            //TODO leave photon room
        }
    }
}
