using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;
using UnityEngine.UI;

public class DungeonMap : MonoBehaviour
{
    [SerializeField] private GameObject roomIcon;
    [SerializeField] private float iconsDistance = 2f;
    private Dictionary<GameObject, GameObject> _roomIcons;
    private GameObject _activeRoom;
    private Menu _menu;

    // Start is called before the first frame update
    void Start()
    {
        _roomIcons = new Dictionary<GameObject, GameObject>();
        _menu = gameObject.GetComponent<Menu>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("m") && !_menu.isFocused()) _menu.Focus(true);
        if(Input.anyKey && _menu.isFocused()) _menu.Focus(false);
    }

    public void AddRoom(GameObject room)
    {
        if (room != null && !_roomIcons.Keys.Contains(room))
        {
            DungeonRoom dungeonRoom = room.GetComponent<DungeonRoom>();
            float iconX = room.transform.position.x / dungeonRoom.width * iconsDistance;
            float iconY = room.transform.position.x / dungeonRoom.width * iconsDistance;
            GameObject roomIconInstance = Instantiate(roomIcon);
            roomIconInstance.transform.SetParent(gameObject.transform,false);
            roomIconInstance.transform.position = new Vector3(iconX, iconY, 0);
            _roomIcons.Add(room,roomIconInstance);
        }
    }

    public void ActiveRoomIcon(GameObject room)
    {
        if(_activeRoom!=null) _activeRoom.GetComponent<Image>().color = Color.black;
        if (_roomIcons.Keys.Contains(room))
        {
            _roomIcons[room].gameObject.GetComponent<Image>().color = Color.white;
            _activeRoom = room;
        }
    }
}
