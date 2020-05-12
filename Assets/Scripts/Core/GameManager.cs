﻿using System.Collections;
using System.Collections.Generic;
using Core;
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
        //if(PhotonNetwork.IsMasterClient) GetComponent<DungeonBuilder>().BuildDungeon(0,0,20,2);
        if (gameObject.GetPhotonView().IsMine)
        {
            GameObject player = PhotonNetwork.Instantiate("VoodooTmp", new Vector3(-20f, 0f, 0f), Quaternion.identity);
            GameObject myCamera = Instantiate(camera, new Vector3(-20f,0f,-10f), Quaternion.identity);
            myCamera.GetComponent<CameraFocusOnPlayer>().cameraPlayer = player;
        }
    }
    
    
    void Update()
    {
        
    }
    
    public void FirstLevel()
    {
        //FirstLevel beginning code... 
    }

}
