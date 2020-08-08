using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.Events;

namespace Core
{
    
    public delegate void Cback(); // declare delegate type

    /// <summary>
    /// Class used to collect, mark events with names, and call callback functions when the event is triggered.
    /// </summary>
    public class EventsCollector
    {
        private static EventsCollector _instance=null;
        private Dictionary<string,Cback> _events;

        protected EventsCollector()
        {
            _events = new Dictionary<string, Cback>();
        }
    
        public static EventsCollector Instance
        {
            get
            {
                if(_instance==null) _instance=new EventsCollector();
                return _instance;
            }
        }

        public void CreateEvent(string eventName, Cback cback)
        {
            _events.Add(eventName,cback);
        }

        public string GetEvent(string eventName)
        {
            return _events.Keys.Contains(eventName) ? eventName : null;
        }

        public void TriggerEvent(string name)
        {
            _events[name]();
        }
    }
}
