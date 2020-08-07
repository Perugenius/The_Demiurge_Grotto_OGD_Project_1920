using System.Collections.Generic;
using System.IO;
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

        void Awake()
        {
            //List all object pools
        
            //ObjectPoolingManager.Instance.CreatePool (PoolingExample, 100, 200);
        
            //....
        }

        void Start()
        {
            _dungeonBuilder = GetComponent<DungeonBuilder>();
            OnJoinScene();
        }

        [PunRPC]
        private void BuildDungeon()
        {
            //TODO for testing reasons all skills are passed to dungeon builder
            List<DungeonRoom.PlatformingSkills> playersSkills = new List<DungeonRoom.PlatformingSkills>();
            playersSkills.Add(DungeonRoom.PlatformingSkills.Headstrong);
            playersSkills.Add(DungeonRoom.PlatformingSkills.DoubleJump);
            playersSkills.Add(DungeonRoom.PlatformingSkills.Intangibility);
            playersSkills.Add(DungeonRoom.PlatformingSkills.WallJump);
        
            GetComponent<DungeonBuilder>().BuildDungeon(1,playersSkills,15,2);
        }

        public void OnJoinScene()
        {
            if (PhotonNetwork.PlayerList.Length == _numOfPlayers) GetComponent<PhotonView>().RPC("BuildDungeon", RpcTarget.MasterClient);
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
    }
}
