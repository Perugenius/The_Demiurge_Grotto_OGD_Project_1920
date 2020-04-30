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
            _audioManager.ExampleSound();
            EventManager.StartListening("PlayerDies", PlayerDies);
        }
        
    }
}