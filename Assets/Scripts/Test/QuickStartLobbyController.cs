using System.Collections;
using System.Collections.Generic;
using Core.SaveLoadData;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class QuickStartLobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField] private int roomSize;  //Manual set number of players per room
    // Start is called before the first frame update
    private string _characterName;
    private string _dungeonChosen;
    private List<string> _characters = new List<string>()
    {
        "Pinkie",
        "Voodoo",
        "Kinja",
        "Steve"
    };

    private List<string> _charactersRemaining = new List<string>();
    
    void Start()
    {
        _characterName = SaveSystem.LoadPlayerData().currentCharacter;
        _dungeonChosen = SaveSystem.LoadPlayerData().lastSelectedDungeon;
        foreach (string character in _characters)
        {
            if(character!= _characterName) _charactersRemaining.Add(_characterName);
        }
        
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Hashtable roomProperties = new Hashtable {{"grotto", _dungeonChosen},{"grotto", _characterName}};
        PhotonNetwork.JoinRandomRoom(roomProperties,(byte)roomSize);    //Tries to join a random room
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
        Hashtable roomProperties = new Hashtable {{"grotto", _dungeonChosen}, {"grotto", _charactersRemaining[0]}, {"grotto", _charactersRemaining[1]}, {"grotto", _charactersRemaining[2]}};
        RoomOptions roomOps = new RoomOptions(){IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize, CustomRoomProperties = roomProperties};
        PhotonNetwork.CreateRoom("Room" + randomNumber, roomOps);    //Attempt to create a new room
        Debug.Log("Attempting to create room " + randomNumber);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room... Trying again");
        CreateRoom();    //Retrying to create the room with a new random name
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
