using Mechanics;
using UnityEngine;

namespace Model
{
    /// <summary>
    /// The main model containing needed data to implement the 
    /// game. This class should only contain data, and methods that operate 
    /// on the data. It is initialised with data in the GameController class.
    /// </summary>
    [System.Serializable]
    public class GameModel: Singleton<GameModel>
    {
        /// <summary>
        /// The main component which controls the player sprite, controlled 
        /// by the user.
        /// </summary>
        public PlayerController player;

        /// <summary>
        /// The spawn point in the scene.
        /// </summary>
        public Transform spawnPoint;
        
        /// <summary>
        /// The player speed.
        /// </summary>
        public float playerSpeed;
        //...
        
        
    }
}