using Core;
using UnityEngine;

namespace Examples
{

    public class PlayerEventsListener : MonoBehaviour
    {
        public AudioManager _audioManager;

        // Use this for initialization
        void OnEnable()
        {
            EventManager.StartListening("PlayerDies", PlayerDies);
        }

        void PlayerDies()
        {
            EventManager.StopListening("PlayerDies", PlayerDies);
            Debug.Log("Player is dead");
            EventManager.StartListening("PlayerDies", PlayerDies);
        }
        
    }
}