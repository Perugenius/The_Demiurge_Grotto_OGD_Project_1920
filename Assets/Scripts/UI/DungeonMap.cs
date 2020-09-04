using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Model;
using UnityEngine;
using UnityEngine.UI;

public class DungeonMap : MonoBehaviour
{
    [SerializeField] private GameObject roomIcon;
    [SerializeField] private float iconsDistance = 2f;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;
    private Dictionary<GameObject, GameObject> _roomIcons;
    private GameObject _activeRoom;
    private DungeonBuilder _dungeonBuilder;
    private List<GameObject> _roomsList;
    private bool _mapIsCentered = false;

    public GameObject ActiveRoom => _activeRoom;

    private Menu _menu;
    private float _xOffset = 1f;
    private float _yOffset = 1f;

    // Start is called before the first frame update
    void Start()
    {
        _roomIcons = new Dictionary<GameObject, GameObject>();
        _menu = gameObject.GetComponent<Menu>();
        _dungeonBuilder = GameObject.Find("GameManager").GetComponent<DungeonBuilder>();
    }

    // Update is called once per frame
    void Update()
    {
        if((Input.GetKeyDown("m") || Input.GetKeyDown(KeyCode.Joystick1Button6)) && !_menu.isFocused() && !_menu.DuringTransition) _menu.Focus(true);
        if((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button1)) && _menu.isFocused() && !_menu.DuringTransition) _menu.Focus(false);
        if((Input.GetKeyDown("m") || Input.GetKeyDown(KeyCode.Joystick1Button6)) && _menu.isFocused() && !_menu.DuringTransition) _menu.Focus(false);

        if (!_mapIsCentered && _dungeonBuilder.DungeonReady) CenterMap();
    }

    private void CenterMap()
    {
        _roomsList = _dungeonBuilder.RoomInstancesList;
        _mapIsCentered = true;
        Vector3 firstRoomPosition = _roomsList[0].transform.position;
        float xUpperBound = firstRoomPosition.x, yUpperBound = firstRoomPosition.y, xLowerBound = firstRoomPosition.x, yLowerBound = firstRoomPosition.y;
        
        foreach (var room in _roomsList)
        {
            Vector3 roomPosition = room.transform.position;
            xUpperBound = roomPosition.x > xUpperBound ? roomPosition.x : xUpperBound;
            yUpperBound = roomPosition.y > yUpperBound ? roomPosition.y : yUpperBound;
            xLowerBound = roomPosition.x < xLowerBound ? roomPosition.x : xLowerBound;
            yLowerBound = roomPosition.y < yLowerBound ? roomPosition.y : yLowerBound;
        }

        DungeonRoom dungeonRoom = _roomsList[0].GetComponent<DungeonRoom>();
        _xOffset = -(xUpperBound + xLowerBound) / dungeonRoom.width;
        _yOffset = -(yUpperBound + yLowerBound) / dungeonRoom.height;
    }

    public void AddRoom(GameObject room)
    {
        if (room != null && !_roomIcons.Keys.Contains(room))
        {
            DungeonRoom dungeonRoom = room.GetComponent<DungeonRoom>();
            float iconX = room.transform.position.x / dungeonRoom.width * iconsDistance + _xOffset;
            float iconY = room.transform.position.y / dungeonRoom.height * iconsDistance + _yOffset;
            GameObject roomIconInstance = Instantiate(roomIcon);
            roomIconInstance.transform.SetParent(gameObject.transform, false);
            roomIconInstance.transform.position += new Vector3(iconX,iconY,0);
            SetIconTransitions(roomIconInstance, dungeonRoom);
            _roomIcons.Add(room,roomIconInstance);
        }
    }

    private void SetIconTransitions(GameObject roomIconInstance, DungeonRoom dungeonRoom)
    {
        List<RoomSides> entrances = dungeonRoom.GetEffectiveEntranceSides();
        List<RoomSides> exits = dungeonRoom.GetEffectiveExitSides();
        
        {
            if (!entrances.Contains(RoomSides.Down) && !exits.Contains(RoomSides.Down))
                roomIconInstance.transform.Find("transitionDown").gameObject.SetActive(false);
            if (!entrances.Contains(RoomSides.Top) && !exits.Contains(RoomSides.Top))
                roomIconInstance.transform.Find("transitionTop").gameObject.SetActive(false);
            if (!entrances.Contains(RoomSides.Right) && !exits.Contains(RoomSides.Right))
                roomIconInstance.transform.Find("transitionRight").gameObject.SetActive(false);
            if (!entrances.Contains(RoomSides.Left) && !exits.Contains(RoomSides.Left))
                roomIconInstance.transform.Find("transitionLeft").gameObject.SetActive(false);
        }
    }

    public void ActiveRoomIcon(GameObject room)
    {
        if(_activeRoom!=null) _roomIcons[_activeRoom].GetComponent<Image>().color = inactiveColor;
        if (_roomIcons.Keys.Contains(room))
        {
            _roomIcons[room].gameObject.GetComponent<Image>().color = activeColor;
            _activeRoom = room;
        }
    }
}
