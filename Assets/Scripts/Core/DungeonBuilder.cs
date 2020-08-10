﻿/*
 * @Author: Elio Salvini
 */

using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Model;
using Photon.Pun;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core
{
    public enum RoomSides
    {
        Right, Left, Top, Down, None
    }
    public class FrontierPointData
    {
        public Vector3 Position;
        public RoomSides EntranceSide;

        public FrontierPointData(Vector3 position, RoomSides entranceSide)
        {
            Position = position;
            EntranceSide = entranceSide;
        }
    }

    public class RoomNode
    {
        public RoomNode Parent;
        public GameObject RoomInstance;
        public GameObject PrefabRoom;
        public List<string> Blacklist;
        public List<FrontierPointData> RoomFrontier;

        public RoomNode(RoomNode parent, GameObject roomInstance, GameObject prefabRoom)
        {
            Parent = parent;
            RoomInstance = roomInstance;
            PrefabRoom = prefabRoom;
            Blacklist = new List<string>();
            RoomFrontier = new List<FrontierPointData>();
        }
    }
    
    /// <summary>
    /// Class whose task is to build procedural dungeons
    /// </summary>
    public class DungeonBuilder : MonoBehaviour
    {
        /// <summary>
        /// True if dungeon has been builded
        /// </summary>
        public bool dungeonReady = false;
        
        /// <summary>
        /// List of all existing rooms to build a dungeon
        /// </summary>
        private Object[] _roomsList;

        private List<GameObject> _roomInstancesList;

        public List<GameObject> RoomInstancesList => _roomInstancesList;

        private List<GameObject> _roomsSpecificTypeList;
        private List<DungeonRoom> _roomsScriptsList;
        private List<DungeonRoom> _leafRooms;
        private int _numOfRooms;
        private int _maxNumOfUsages;
        private int _currentNumberOfRooms = 0;
        private List<Vector3> _roomsPositions;
        private List<FrontierPointData> _frontier;
        private List<int> _randomIndexesList;
        private bool _leafRoomNeeded = false;
        private int _currentDifficulty = 0;

        /// <summary>
        /// This function builds a procedural dungeon by instantiating random rooms in the scene. Dungeons are created
        /// respecting the constraints specified in the parameters, however since the set of rooms is limited all
        /// constraints together can be unsatisfiable, in these cases the "maxNumOfUsages" is considered flexible.
        /// The algorithm tries to do the best to respect all constraints, if it can't the "maxNumOfUsages" for a single
        /// room is increased. The build dungeon is loop free.
        /// </summary>
        /// <param name="type"> dungeon type <see cref="Model.DungeonRoom"/> </param>
        /// <param name="requiredSkills"> skills required inside the dungeon </param>
        /// <param name="numOfRooms"> number of desired room to build a dungeon </param>
        /// <param name="maxNumOfUsages"> max number of time that a single prefab room can appear in the dungeon
        ///                               (flexible constraint) </param> 
        public void BuildDungeon(int type, List<DungeonRoom.PlatformingSkills> requiredSkills, int numOfRooms, int maxNumOfUsages) {
            _roomsPositions = new List<Vector3>();
            _frontier = new List<FrontierPointData>();
            _numOfRooms = numOfRooms;
            _maxNumOfUsages = maxNumOfUsages;
            _roomInstancesList = new List<GameObject>();
            
            //Init all needed lists of rooms
            InitLists(type, requiredSkills);

            //Initialization: instantiation of first room 
            GameObject firstRoom = SelectFirstRoom();
            GameObject firstRoomInstance = PhotonNetwork.Instantiate(GetGameObjectPath(firstRoom, type), Vector3.zero, Quaternion.identity);
            _roomInstancesList.Add(firstRoomInstance);
            RoomNode root = new RoomNode(null,firstRoomInstance, firstRoom);
            _roomsScriptsList[_roomsSpecificTypeList.IndexOf(firstRoom)].AddUsage();
            _roomsPositions.Add(Vector3.zero);
            _currentNumberOfRooms++;
            UpdateFrontier(firstRoom, Vector3.zero, RoomSides.None, root);

            RoomNode parent = root;
            
            for (int i = 0; i < numOfRooms - 2; i++)
            {
                GameObject room = SelectRoom(_frontier[0].EntranceSide, parent);

                if (room != null)
                {
                    _roomsScriptsList[_roomsSpecificTypeList.IndexOf(room)].AddUsage();
                    GameObject roomInstance = PhotonNetwork.Instantiate(GetGameObjectPath(room, type), _frontier[0].Position, Quaternion.identity);
                    _roomInstancesList.Add(roomInstance);
                    RoomNode child = new RoomNode(parent,roomInstance, room);
                    parent = child;
                    _roomsPositions.Add(_frontier[0].Position);
                    _currentNumberOfRooms++;
                    UpdateFrontier(room, _frontier[0].Position, _frontier[0].EntranceSide,child);
                    _frontier.RemoveAt(0);
                    _currentDifficulty = Mathf.RoundToInt((i+2f) / numOfRooms * 5f);
                    roomInstance.GetComponent<DungeonRoom>().SetDifficulty(_currentDifficulty);
                    /*Debug.Log("current room difficulty: " + _currentDifficulty);
                    Debug.Log("Frontier top: " +_frontier[0].Position.ToString());
                    Debug.Log("Frontier length = " + _frontier.Count);*/
                }
                else //Last room placed removed for tree backtracking
                {
                    i--;
                }
            }

            GameObject lastRoom = SelectLastRoom(_frontier[0].EntranceSide);
            GameObject lastRoomInstance = PhotonNetwork.Instantiate(GetGameObjectPath(lastRoom, type), _frontier[0].Position, Quaternion.identity);
            _roomInstancesList.Add(lastRoomInstance);
            
            dungeonReady = true;
        }

        private void InitLists(int type, List<DungeonRoom.PlatformingSkills> skills)
        {
            //Initialization of room scripts list
            _roomsList = Resources.LoadAll(Path.Combine("DungeonRooms","Type" + type), typeof(GameObject));
            _roomsScriptsList = new List<DungeonRoom>();
            _roomsSpecificTypeList = new List<GameObject>();
            foreach (var room in _roomsList)
            {
                GameObject roomGameObject = (GameObject) room;
                DungeonRoom roomScript = roomGameObject.GetComponent<DungeonRoom>();
                bool skipRoom = false;

                //Rooms are filtered on the basis of the required skills (rooms that require skills not contained in parameter
                //"requiredSkills" are not considered)
                foreach (var reqSkill in roomScript.requiredSkills)
                {
                    if (!skills.Contains(reqSkill)) skipRoom = true;
                }
                if (!skipRoom)
                {
                    _roomsSpecificTypeList.Add(roomGameObject);
                    roomScript.ResetNumOfUsages();
                    _roomsScriptsList.Add(roomScript);
                }
            }
            
            //Find all leaf rooms (rooms that doesn't increase the frontier)
            _leafRooms = GetLeafRooms(_roomsScriptsList);
        }

        //-------------------DEBUG CODE--------------------------------
        //Update is used to progressively build the dungeon during debug 
        //private bool ready;
        private void Update()
        {
            /*if(!ready || _currentNumberOfRooms == _numOfRooms) return;
            ready = false;
            
            if (_currentNumberOfRooms == _numOfRooms - 1)
            {
                GameObject lastRoom = SelectLastRoom(_frontier[0].EntranceSide);
                Instantiate(lastRoom, _frontier[0].Position, Quaternion.identity);
                _currentNumberOfRooms++;
                return;
            }

            GameObject room = SelectRoom(_frontier[0].EntranceSide);
            Debug.Log("Instantiating: " + room.name + " IsLeaf: " + _leafRooms.Contains(_roomsScriptsList[roomsList.IndexOf(room)]));
            Instantiate(room, _frontier[0].Position, Quaternion.identity);
            _roomsScriptsList[roomsList.IndexOf(room)].AddUsage();
            _roomsPositions.Add(_frontier[0].Position);
            _currentNumberOfRooms++;
            UpdateFrontier(room, _frontier[0].Position, _frontier[0].EntranceSide);
            _frontier.RemoveAt(0);
            Debug.Log("Frontier top: " +_frontier[0].Position.ToString());
            Debug.Log("Frontier length = " + _frontier.Count);
            
            foreach (var nextRoom in _frontier)
            {
                Debug.Log("\nFrontier:\n");
                Debug.Log(nextRoom.Position);
                Debug.Log(nextRoom.EntranceSide);
            }

            StartCoroutine(Wait());*/
            //ready = true;
        }

        /*private IEnumerator Wait()
        {
            yield return new WaitForSeconds(0.2f);
            ready = true;
        }*/
        //--------------------------------------------------------------

        private void UpdateFrontier(GameObject room, Vector3 position, RoomSides entranceSide, RoomNode node)
        {
            DungeonRoom dungeonRoom = _roomsScriptsList[_roomsSpecificTypeList.IndexOf(room)];

            if (dungeonRoom.rightAccess == DungeonRoom.AccessType.Exit ||
                dungeonRoom.rightAccess == DungeonRoom.AccessType.EntranceExit && entranceSide != RoomSides.Right)
            {
                FrontierPointData roomFrontierPoint = 
                    new FrontierPointData(position + new Vector3(dungeonRoom.width, 0f, 0f), RoomSides.Left);
                _frontier.Add(roomFrontierPoint);
                node.RoomFrontier.Add(roomFrontierPoint);
            }

            if (dungeonRoom.leftAccess == DungeonRoom.AccessType.Exit ||
                dungeonRoom.leftAccess == DungeonRoom.AccessType.EntranceExit && entranceSide != RoomSides.Left)
            {
                FrontierPointData roomFrontierPoint =
                    new FrontierPointData(position - new Vector3(dungeonRoom.width, 0f, 0f), RoomSides.Right);
                _frontier.Add(roomFrontierPoint);
                node.RoomFrontier.Add(roomFrontierPoint);
            }

            if (dungeonRoom.topAccess == DungeonRoom.AccessType.Exit ||
                dungeonRoom.topAccess == DungeonRoom.AccessType.EntranceExit && entranceSide != RoomSides.Top)
            {
                FrontierPointData roomFrontierPoint =
                    new FrontierPointData(position + new Vector3(0f, dungeonRoom.height, 0f), RoomSides.Down);
                _frontier.Add(roomFrontierPoint);
                node.RoomFrontier.Add(roomFrontierPoint);
            }

            if (dungeonRoom.downAccess == DungeonRoom.AccessType.Exit ||
                dungeonRoom.downAccess == DungeonRoom.AccessType.EntranceExit && entranceSide != RoomSides.Down)
            {
                FrontierPointData roomFrontierPoint =
                    new FrontierPointData(position - new Vector3(0f, dungeonRoom.height, 0f), RoomSides.Top);
                _frontier.Add(roomFrontierPoint);
                node.RoomFrontier.Add(roomFrontierPoint);
            }
        }

        private GameObject SelectRoom(RoomSides entranceSide, RoomNode parent)
        {
            //all remaining rooms to build must be leaf rooms
            if(!_leafRoomNeeded) _leafRoomNeeded = _frontier.Count >= (_numOfRooms - _currentNumberOfRooms);
            
            /*Debug.Log("LeafRoomNeeded = " + _leafRoomNeeded);*/

            var rooms = (_leafRoomNeeded)?_leafRooms:_roomsScriptsList;

            List<GameObject> suitableRooms = FindSuitableRooms(entranceSide, rooms, parent);
            return suitableRooms?[Random.Range(0, suitableRooms.Count)];
        }

        private List<GameObject> FindSuitableRooms(RoomSides entranceSide, List<DungeonRoom> rooms, RoomNode parent)
        {
            //to avoid endless loops
            var numOfLoops = 0;
            
            var suitableRooms = new List<GameObject>();
            
            //if we can't find a suitable room the maxNumOfUsages is incremented
            var flexibleNumOfUsages = 0;
            
            //While is used for making maxNumOfUsages a flexible constraint; max flexibility is set to 5
            while (suitableRooms.Count == 0 && numOfLoops < 5)
            {
                foreach (var room in rooms)
                {

                    //Each room is suitable to be the next to be instantiated if it satisfies all the following conditions 
                    
                    /*if(_leafRooms.Contains(room)) Debug.Log(room.name + " IsLeaf");
                    else Debug.Log("IsNOTLeaf");*/

                    //Avoid to close the dungeon by bringing the frontier to zero before having instantiated all requested rooms
                    bool suitable = !(room.GetNumberOfEffectiveExits(entranceSide) == 0 && _frontier.Count <= 1 && _currentNumberOfRooms < (_numOfRooms-1));

                    //Avoid to visit again wrong nodes (for tree backtracking) 
                    if (suitable && parent.Blacklist.Contains(room.gameObject.name)) suitable = false;

                    //Don't instantiate lastRoom before others 
                    if (suitable && room.isLastRoom && _currentNumberOfRooms != _numOfRooms - 1) suitable = false; 
                    
                    //Next room entrance side is compatible with the one specified by the frontier
                    if (suitable && !room.GetEffectiveEntranceSides().Contains(entranceSide)) suitable = false;
                    
                    //The room has not been used more times than "maxNumOfUsages" (flexible constraint)
                    if (suitable && room.GetNumberOfUsages() > _maxNumOfUsages + flexibleNumOfUsages) suitable = false;
                    
                    //Check that by instantiating the next room we don't increase the frontier too much; if the frontier
                    //is bigger than the number of remaining rooms to build we cannot complete the dungeon respecting
                    //the "numOfRooms" constraint
                    if (suitable && !_leafRoomNeeded && _frontier.Count + room.GetNumberOfEffectiveExits(entranceSide) > (_numOfRooms - _currentNumberOfRooms))
                        suitable = false;

                    //Check that rooms don't overlap with existing rooms, or with rooms that are in the frontier (but 
                    // they aren't' instantiated yet). NB the dungeon is loop free.
                    if(suitable)
                    {
                        List<Vector3> nextRoomsPositions = new List<Vector3>();
                        foreach (var exit in room.GetEffectiveExitSides())
                        {
                            if (exit == RoomSides.Right && entranceSide!=RoomSides.Right)
                                nextRoomsPositions.Add(_frontier[0].Position + new Vector3(room.width, 0f, 0f));
                            if (exit == RoomSides.Left && entranceSide!=RoomSides.Left)
                                nextRoomsPositions.Add(_frontier[0].Position - new Vector3(room.width, 0f, 0f));
                            if (exit == RoomSides.Top && entranceSide!=RoomSides.Top)
                                nextRoomsPositions.Add(_frontier[0].Position + new Vector3(0f, room.height, 0f));
                            if (exit == RoomSides.Down && entranceSide!=RoomSides.Down)
                                nextRoomsPositions.Add(_frontier[0].Position - new Vector3(0f, room.height, 0f));
                        }

                        foreach (var nextRoomPosition in nextRoomsPositions)
                        {
                            //overlap with existing rooms
                            foreach (var frontierPosition in _frontier)
                            {
                                if (frontierPosition.Position == nextRoomPosition)
                                {
                                    suitable = false;
                                    break;
                                }
                            }
                            
                            if(!suitable) break;
                            
                            //overlap with frontier rooms (that wil be instantiated)
                            if (_roomsPositions.Contains(nextRoomPosition))
                            {
                                /*Debug.Log("Condition 5");*/
                                suitable = false;
                                break;
                            }
                        }
                    }
                
                    if(suitable) suitableRooms.Add(room.gameObject);
                }
                numOfLoops++;
                flexibleNumOfUsages++; //"maxNumOfUsages"flexibility increase
            }

            if (suitableRooms.Count == 0) RemoveLastRoom(parent); //None room available, go back and remove last room 

            return suitableRooms;
        }

        private GameObject SelectFirstRoom()
        {
            List<GameObject> suitableRooms = new List<GameObject>();
            foreach (var room in _roomsScriptsList)
            {
                if(room.isFirstRoom) suitableRooms.Add(_roomsSpecificTypeList[_roomsScriptsList.IndexOf(room)]);
            }
            
            return suitableRooms[Random.Range(0, suitableRooms.Count)];
        }
        
        private GameObject SelectLastRoom(RoomSides entranceSide)
        {
            List<GameObject> suitableRooms = new List<GameObject>();
            foreach (var room in _roomsScriptsList)
            {
                if(room.isLastRoom && room.GetEffectiveEntranceSides().Contains(entranceSide)) suitableRooms.Add(_roomsSpecificTypeList[_roomsScriptsList.IndexOf(room)]);
            }
            return suitableRooms[Random.Range(0, suitableRooms.Count)];
        }

        private List<DungeonRoom> GetLeafRooms(List<DungeonRoom> rooms)
        {
            List<DungeonRoom> leafRooms = new List<DungeonRoom>();
            foreach (var room in rooms)
            {
                if(room.GetNumberOfEntranceExits() + room.GetNumberOfEntrance() <= 1 && room.GetNumberOfExits()==0)
                {
                    /*Debug.Log("Leaf: " + room.name);*/
                    leafRooms.Add(room);
                }
            }

            return leafRooms;
        }
        
        /// <summary>
        /// Remove last node/room created, and mark it to avoid to visit it again (tree backtracking)
        /// </summary>
        /// <param name="node">node to be removed</param>
        private void RemoveLastRoom(RoomNode node)
        {
            Debug.LogWarning("One room removed - tree backtracking");
            Destroy(node.RoomInstance);
            node.Parent.Blacklist.Add(node.PrefabRoom.name);
            _currentNumberOfRooms--;
            _roomsPositions.RemoveAt(_roomsPositions.Count-1);
            foreach (var point in node.RoomFrontier)
            {
                _frontier.Remove(point);
            }
        }

        /// <summary>
        /// This function returns a random number between 0 and maxNum. Only once the same number is returned.
        /// </summary>
        private int RandomNotRepeated(int maxNum)
        {
            if(_randomIndexesList == null) _randomIndexesList = new List<int>(maxNum);
            for (int i = 0; i < _randomIndexesList.Count; i++)
            {
                _randomIndexesList[i] = i;
            }
            var randomNumber = Random.Range(0, _randomIndexesList.Count);
            var result = _randomIndexesList[randomNumber];
            _randomIndexesList.Remove(result);
            return result;
        }

        private RoomSides GetOppositeSide(RoomSides roomSide)
        {
            if (roomSide == RoomSides.Right) return RoomSides.Left;
            if (roomSide == RoomSides.Left) return RoomSides.Right;
            if (roomSide == RoomSides.Top) return RoomSides.Down;
            return RoomSides.Top;
        }
        
        private static string GetGameObjectPath(GameObject obj, int type)
        {
            return Path.Combine("DungeonRooms","Type" + type, obj.name);
            /*string path = AssetDatabase.GetAssetPath(obj);
            string resourcePath = path.Substring(17);
            return resourcePath;*/
        }
    }
}
