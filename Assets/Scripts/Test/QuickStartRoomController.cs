using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class QuickStartRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField] private int multiplayerSceneIndex;    //Number for the build index to the multiplay scene

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    
    public override void OnDisable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        StartGame();
    }

    void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Starting Game...");
            PhotonNetwork.LoadLevel(multiplayerSceneIndex);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
