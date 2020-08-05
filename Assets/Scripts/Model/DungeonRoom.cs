/*
 * @Author: Elio Salvini
 */

using System;
using System.Collections.Generic;
using Core;
using Mechanics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Model
{
    public class DungeonRoom: MonoBehaviour
    {
        /// <summary>
        /// Room available types of accesses:
        /// - "Entrance" if it is possible to enter the room that side;
        /// - "Exit" if it is possible to exit the room that side;
        /// - "EntranceExit if it is possible to both enter/exit from the same side; 
        /// - "Closed" if there is no way to enter/exit the room from that side.
        /// </summary>
        public enum AccessType
        {
            Entrance, Exit, EntranceExit, Closed
        }

        public enum PlatformingSkills
        {
            DoubleJump, WallJump, Headstrong, Intangibility
        }
        
        /// <summary>
        /// Room right side access type 
        /// </summary>
        public AccessType rightAccess = AccessType.Closed;
        
        /// <summary>
        /// Room top side access type
        /// </summary>
        public AccessType topAccess = AccessType.Closed;
        
        /// <summary>
        /// Room left side access type
        /// </summary>
        public AccessType leftAccess = AccessType.Closed;
        
        /// <summary>
        /// Room down side access type
        /// </summary>
        public AccessType downAccess = AccessType.Closed;

        /// <summary>
        /// Room width in the scene
        /// </summary>
        public float width;

        /// <summary>
        /// Room height in the scene
        /// </summary>
        public float height;

        /// <summary>
        /// Dungeon type (it influences enemy, platform and reward types)
        /// </summary>
        public int dungeonType = 0;
        
        /// <summary>
        /// Room minimum level of difficulty 
        /// </summary>
        public int minDifficulty = 0;

        /// <summary>
        /// List of required skills to complete(or that can be simply used) the room
        /// </summary>
        public List<PlatformingSkills> requiredSkills;

        /// <summary>
        /// true if the room is the starting point of a dungeon
        /// </summary>
        public bool isFirstRoom = false;

        /// <summary>
        /// true if the room is the dungeon end
        /// </summary>
        public bool isLastRoom = false;

        /// <summary>
        /// Number of times that a room is been used in the last dungeon build.
        /// </summary>
        private int _numberOfUsages = 0;

        /// <summary>
        /// Room level of difficulty
        /// </summary>
        public int _difficulty = 0;

        /// <summary>
        /// Getter for the total number of exits and entrance/exits of a room given an entrance side. If the room has
        /// an EntranceExit AccessType at the side of the specified entrance, it isn't counted as an exit.'
        /// </summary>
        /// <returns>total number of exits of a room</returns>
        public int GetNumberOfEffectiveExits(RoomSides entranceSide)
        {
            /*Debug.Log(gameObject.name+": "+((rightAccess == AccessType.Exit || 
                        (rightAccess == AccessType.EntranceExit && entranceSide!=RoomSides.Right)) ? 1 : 0)
                      + ((leftAccess == AccessType.Exit || 
                          (leftAccess == AccessType.EntranceExit && entranceSide!=RoomSides.Left)) ? 1 : 0)
                      + ((topAccess == AccessType.Exit || 
                          (topAccess == AccessType.EntranceExit && entranceSide!=RoomSides.Top)) ? 1 : 0)
                      + ((downAccess == AccessType.Exit || 
                          (downAccess == AccessType.EntranceExit && entranceSide!=RoomSides.Down)) ? 1 : 0));*/
            
            return   ((rightAccess == AccessType.Exit || 
                       (rightAccess == AccessType.EntranceExit && entranceSide!=RoomSides.Right)) ? 1 : 0)
                   + ((leftAccess == AccessType.Exit || 
                       (leftAccess == AccessType.EntranceExit && entranceSide!=RoomSides.Left)) ? 1 : 0)
                   + ((topAccess == AccessType.Exit || 
                       (topAccess == AccessType.EntranceExit && entranceSide!=RoomSides.Top)) ? 1 : 0)
                   + ((downAccess == AccessType.Exit || 
                       (downAccess == AccessType.EntranceExit && entranceSide!=RoomSides.Down)) ? 1 : 0);
        }
        
        public int GetNumberOfExits()
        {
            return   ((rightAccess == AccessType.Exit) ? 1 : 0)
                     + ((leftAccess == AccessType.Exit) ? 1 : 0)
                     + ((topAccess == AccessType.Exit) ? 1 : 0)
                     + ((downAccess == AccessType.Exit) ? 1 : 0);
        }
        
        public int GetNumberOfEntrance()
        {
            return   ((rightAccess == AccessType.Entrance) ? 1 : 0)
                     + ((leftAccess == AccessType.Entrance) ? 1 : 0)
                     + ((topAccess == AccessType.Entrance) ? 1 : 0)
                     + ((downAccess == AccessType.Entrance) ? 1 : 0);
        }
        
        public int GetNumberOfEntranceExits()
        {
            return   ((rightAccess == AccessType.EntranceExit) ? 1 : 0)
                     + ((leftAccess == AccessType.EntranceExit) ? 1 : 0)
                     + ((topAccess == AccessType.EntranceExit) ? 1 : 0)
                     + ((downAccess == AccessType.EntranceExit) ? 1 : 0);
        }

        /// <summary>
        /// Getter for entrance and entrance/exits sides of a room. If the room has
        /// </summary>
        /// <returns>list of entrance and entrance/exits sides of a room</returns>
        public List<RoomSides> GetEffectiveEntranceSides()
        {
            List<RoomSides> entranceSides = new List<RoomSides>();
            if(rightAccess == AccessType.Entrance || rightAccess == AccessType.EntranceExit) entranceSides.Add(RoomSides.Right);
            if(leftAccess == AccessType.Entrance || leftAccess == AccessType.EntranceExit) entranceSides.Add(RoomSides.Left);
            if(topAccess == AccessType.Entrance || topAccess == AccessType.EntranceExit) entranceSides.Add(RoomSides.Top);
            if(downAccess == AccessType.Entrance || downAccess == AccessType.EntranceExit) entranceSides.Add(RoomSides.Down);
            return entranceSides;
        }
        
        /// <summary>
        /// Getter for exits and entrance/exits sides of a room. If the room has
        /// </summary>
        /// <returns>list of exits and entrance/exits sides of a room</returns>
        public List<RoomSides> GetEffectiveExitSides()
        {
            List<RoomSides> entranceSides = new List<RoomSides>();
            if(rightAccess == AccessType.Exit || rightAccess == AccessType.EntranceExit) entranceSides.Add(RoomSides.Right);
            if(leftAccess == AccessType.Exit || leftAccess == AccessType.EntranceExit) entranceSides.Add(RoomSides.Left);
            if(topAccess == AccessType.Exit || topAccess == AccessType.EntranceExit) entranceSides.Add(RoomSides.Top);
            if(downAccess == AccessType.Exit || downAccess == AccessType.EntranceExit) entranceSides.Add(RoomSides.Down);
            return entranceSides;
        }

        /// <summary>
        /// Function that resets the number of usages of a single room. It needs to be called before each dungeon build.
        /// </summary>
        public void ResetNumOfUsages()
        {
            _numberOfUsages = 0;
        }

        /// <summary>
        /// Function that has to be called every time that a room is instantiated. 
        /// </summary>
        public void AddUsage()
        {
            _numberOfUsages++;
            //Debug.Log(gameObject.name +" NumOfUsages: " + _numberOfUsages);
        }

        public int GetNumberOfUsages()
        {
            return _numberOfUsages;
        }

        /// <summary>
        /// Function used to set difficulty of a room.
        /// It also disables enemiesSpawner according to specified difficulty.
        /// Difficulty 1 -> 20% of enemies
        /// Difficulty 2 -> 40% of enemies
        /// Difficulty 3 -> 60% of enemies
        /// Difficulty 4 -> 80% of enemies
        /// Difficulty 5 -> 100% of enemies 
        /// </summary>
        /// <param name="difficultyLevel">room level of challenge</param>
        public void SetDifficulty(int difficultyLevel)
        {
            _difficulty = (difficultyLevel <= 5) ? (difficultyLevel>=0 ? difficultyLevel : 0) : 5;
            //Debug.Log(gameObject.name + " " + difficultyLevel);
            
            //enable/disable enemies according to difficulty
            GameObject roomArea = gameObject.transform.Find("RoomArea").gameObject;
            List<GameObject> enemies = new List<GameObject>();
            foreach (Transform child in roomArea.transform)
            {
                if(child.gameObject.GetComponent<EnemySpawner>()!=null) enemies.Add(child.gameObject);
            }
            
            int totEnemies = enemies.Count;
            for (int i = 0; i < Mathf.RoundToInt(totEnemies - _difficulty/5f*totEnemies); i++)
            {
                GameObject enemy = enemies[Random.Range(0, enemies.Count-1)];
                enemies.Remove(enemy);
                Destroy(enemy);
            }
        }
        
        public int GetDifficulty()
        {
            return _difficulty;
        }
    }
}
