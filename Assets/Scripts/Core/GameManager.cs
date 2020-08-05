using System.Collections;
using System.Collections.Generic;
using System.IO;
using Core;
using Mechanics.Camera;
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
        if(PhotonNetwork.IsMasterClient) GetComponent<DungeonBuilder>().BuildDungeon(1,0,15,2);
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
