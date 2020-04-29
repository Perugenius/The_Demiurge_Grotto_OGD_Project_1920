using UnityEngine;

namespace Events_Listeners
{
	public class EventListenerTemplate : MonoBehaviour
	{

		// Use this for initialization
		void OnEnable()
		{

			//List of all events that this class is listening
			EventManager.StartListening("EventName", EventFunction);
			//...
		}
		
		void OnDisable() {
			EventManager.StopListening ("EventName", EventFunction);
		}

		void EventFunction()
		{
			EventManager.StopListening("EventName", EventFunction);
			Debug.Log("Event is going to be executed");
			
			//event execution
			//...			
			
			EventManager.StartListening("EventName", EventFunction);
		}
	}
}