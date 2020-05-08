/*
 * @Author: Elio Salvini
 */

using System.Collections.Generic;
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
    
    /// <summary>
    /// Class whose task is to build procedural dungeons
    /// </summary>
    public class DungeonBuilder : MonoBehaviour
    {
        /// <summary>
        /// List of all existing rooms to build a dungeon
        /// </summary>
        public List<GameObject> roomsList;

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

        /// <summary>
        /// This function builds a procedural dungeon by instantiating random rooms in the scene. Dungeons are created
        /// respecting the constraints specified in the parameters, however since the set of rooms is limited all
        /// constraints together can be unsatisfiable, in these cases the "maxNumOfUsages" is considered flexible.
        /// The algorithm tries to do the best to respect all constraints, if it can't the "maxNumOfUsages" for a single
        /// room is increased. The build dungeon is loop free.
        /// </summary>
        /// <param name="type"> dungeon type <see cref="Model.DungeonRoom"/> </param>
        /// <param name="difficulty"> dungeon level of difficulty </param>
        /// <param name="numOfRooms"> number of desired room to build a dungeon </param>
        /// <param name="maxNumOfUsages"> max number of time that a single prefab room can appear in the dungeon
        ///                               (flexible constraint) </param> 
        public void BuildDungeon(int type, int difficulty, int numOfRooms, int maxNumOfUsages) {
            _roomsPositions = new List<Vector3>();
            _frontier = new List<FrontierPointData>();
            _numOfRooms = numOfRooms;
            _maxNumOfUsages = maxNumOfUsages;
            
            //Init all needed lists of rooms
            InitLists(type);

            //Initialization: instantiation of first room 
            GameObject firstRoom = SelectFirstRoom();
            PhotonNetwork.Instantiate(AssetDatabase.GetAssetPath(firstRoom), Vector3.zero, Quaternion.identity);
            _roomsScriptsList[_roomsSpecificTypeList.IndexOf(firstRoom)].AddUsage();
            _roomsPositions.Add(Vector3.zero);
            _currentNumberOfRooms++;
            UpdateFrontier(firstRoom, Vector3.zero, RoomSides.None);
            
            for (int i = 0; i < numOfRooms - 2; i++)
            {
                GameObject room = SelectRoom(_frontier[0].EntranceSide);
                PhotonNetwork.Instantiate(AssetDatabase.GetAssetPath(room), _frontier[0].Position, Quaternion.identity);
                _roomsScriptsList[_roomsSpecificTypeList.IndexOf(room)].AddUsage();
                _roomsPositions.Add(_frontier[0].Position);
                _currentNumberOfRooms++;
                UpdateFrontier(room, _frontier[0].Position, _frontier[0].EntranceSide);
                _frontier.RemoveAt(0);
                /*Debug.Log("Frontier top: " +_frontier[0].Position.ToString());
                Debug.Log("Frontier length = " + _frontier.Count);*/
            }

            GameObject lastRoom = SelectLastRoom(_frontier[0].EntranceSide);
            PhotonNetwork.Instantiate(AssetDatabase.GetAssetPath(lastRoom), _frontier[0].Position, Quaternion.identity);
        }

        private void InitLists(int type)
        {
            //Initialization of room scripts list
            _roomsScriptsList = new List<DungeonRoom>();
            _roomsSpecificTypeList = new List<GameObject>();
            foreach (var room in roomsList)
            {
                DungeonRoom roomScript = room.GetComponent<DungeonRoom>();
                roomScript.ResetNumOfUsages();
                if (roomScript.dungeonType == type)
                {
                    _roomsSpecificTypeList.Add(room);
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

        private void UpdateFrontier(GameObject room, Vector3 position, RoomSides entranceSide)
        {
            DungeonRoom dungeonRoom = _roomsScriptsList[_roomsSpecificTypeList.IndexOf(room)];
            
            if(dungeonRoom.rightAccess == DungeonRoom.AccessType.Exit || 
               dungeonRoom.rightAccess == DungeonRoom.AccessType.EntranceExit && entranceSide!=RoomSides.Right) 
                _frontier.Add(new FrontierPointData(position + new Vector3(dungeonRoom.width,0f,0f), RoomSides.Left));
            if(dungeonRoom.leftAccess == DungeonRoom.AccessType.Exit || 
               dungeonRoom.leftAccess == DungeonRoom.AccessType.EntranceExit  && entranceSide!=RoomSides.Left) 
                _frontier.Add(new FrontierPointData(position - new Vector3(dungeonRoom.width,0f,0f),RoomSides.Right));
            if(dungeonRoom.topAccess == DungeonRoom.AccessType.Exit || 
               dungeonRoom.topAccess == DungeonRoom.AccessType.EntranceExit  && entranceSide!=RoomSides.Top) 
                _frontier.Add(new FrontierPointData(position + new Vector3(0f,dungeonRoom.height,0f),RoomSides.Down));
            if(dungeonRoom.downAccess == DungeonRoom.AccessType.Exit ||
               dungeonRoom.downAccess == DungeonRoom.AccessType.EntranceExit  && entranceSide!=RoomSides.Down) 
                _frontier.Add(new FrontierPointData(position - new Vector3(0f,dungeonRoom.height,0f),RoomSides.Top));
        }

        private GameObject SelectRoom(RoomSides entranceSide)
        {
            //all remaining rooms to build must be leaf rooms
            if(!_leafRoomNeeded) _leafRoomNeeded = _frontier.Count >= (_numOfRooms - _currentNumberOfRooms);
            
            /*Debug.Log("LeafRoomNeeded = " + _leafRoomNeeded);*/

            var rooms = (_leafRoomNeeded)?_leafRooms:_roomsScriptsList;

            List<GameObject> suitableRooms = FindSuitableRooms(entranceSide, rooms);
            return suitableRooms[Random.Range(0, suitableRooms.Count)];
            
        }

        private List<GameObject> FindSuitableRooms(RoomSides entranceSide, List<DungeonRoom> rooms)
        {
            //to avoid endless loops (used for debug)
            var numOfLoops = 0;
            
            var suitableRooms = new List<GameObject>();
            
            //if we can't find a suitable room the maxNumOfUsages is incremented
            var flexibleNumOfUsages = 0;
            
            //While is used for making maxNumOfUsages a flexible constraint
            while (suitableRooms.Count == 0 && numOfLoops < 10)
            {
                foreach (var room in rooms)
                {

                    //Each room is suitable to be the next to be instantiated if it satisfies all the following conditions 
                    
                    /*if(_leafRooms.Contains(room)) Debug.Log(room.name + " IsLeaf");
                    else Debug.Log("IsNOTLeaf");*/
                    
                    //Avoid to close the dungeon by bringing the frontier to zero before having instantiated all requested rooms
                    bool suitable = !(room.GetNumberOfEffectiveExits(entranceSide) == 0 && _frontier.Count <= 1 && _currentNumberOfRooms < (_numOfRooms-1));

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
        
        private static string GetGameObjectPath(GameObject obj)
        {
            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return path;
        }
    }
}
