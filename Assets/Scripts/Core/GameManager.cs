using System.Collections;
using System.Collections.Generic;
using System.IO;
using Core;
using Mechanics.Camera;
using Model;
using Photon.Pun;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameObject Player;
    public GameObject PoolingExample;
    public Screen screen;
    public GameObject camera;


    void Awake()
    {
        //List all object pools
        
        //ObjectPoolingManager.Instance.CreatePool (PoolingExample, 100, 200);
        
        //....
    }

    void Start()
    {
        //TODO for testing reasons all skills are passed to dungeon builder
        List<DungeonRoom.PlatformingSkills> playersSkills = new List<DungeonRoom.PlatformingSkills>();
        playersSkills.Add(DungeonRoom.PlatformingSkills.Headstrong);
        playersSkills.Add(DungeonRoom.PlatformingSkills.DoubleJump);
        playersSkills.Add(DungeonRoom.PlatformingSkills.Intangibility);
        playersSkills.Add(DungeonRoom.PlatformingSkills.WallJump);
        
        if(PhotonNetwork.IsMasterClient) GetComponent<DungeonBuilder>().BuildDungeon(1,playersSkills,15,2);
        OnJoinScene();
    }

    public void OnJoinScene()
    {
        GameObject player = PhotonNetwork.Instantiate(Path.Combine("Players","Voodoo"), new Vector3((PhotonNetwork.IsMasterClient)?-7f:-10f, 5f, 0f), Quaternion.identity);
        GameObject myCamera = Instantiate(camera, new Vector3(0f, 0f, -10f), Quaternion.identity);
        myCamera.GetComponent<CameraFocusOnPlayer>().cameraPlayer = player;
    }


    void Update()
    {
        
    }
    
    public void FirstLevel()
    {
        //FirstLevel beginning code... 
    }

}
