using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMap : MonoBehaviour
{
    private DungeonMap _dungeonMap;

    // Start is called before the first frame update
    void Start()
    {
        _dungeonMap = GameObject.Find("Main Camera").transform.Find("Canvas").transform.Find("Map")
            .GetComponent<DungeonMap>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        GameObject parentRoom = gameObject.transform.parent.gameObject;
        if(_dungeonMap.ActiveRoom != parentRoom)
        {
            _dungeonMap.AddRoom(parentRoom);
            _dungeonMap.ActiveRoomIcon(parentRoom);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
