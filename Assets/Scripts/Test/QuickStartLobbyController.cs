using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class QuickStartLobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField] private int roomSize;  //Manual set number of players per room
    // Start is called before the first frame update

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.JoinRandomRoom();    //Tries to join a random room
        Debug.Log("Starting...");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Existing room not found");
        CreateRoom();
    }

    void CreateRoom()
    {
        Debug.Log("Creating room...");
        int randomNumber = Random.Range(0, 1000);    //Random name for the room
        RoomOptions roomOps = new RoomOptions(){IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize};
        PhotonNetwork.CreateRoom("Room" + randomNumber, roomOps);    //Attempt to create a new room
        Debug.Log("Attempting to create room " + randomNumber);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room... Trying again");
        CreateRoom();    //Retrying to create the room with a new random name
    }
    
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
