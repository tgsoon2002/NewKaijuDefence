using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    #region Data Members

    private Dictionary <string, UnityEvent> eventDictionary;
    private static EventManager _instance;

    public static EventManager instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!_instance)
                {
                    Debug.LogError("No EventManager GameObject detected!");
                }
                else
                {
                    //Initialize EventManager
                    _instance.Init();
                }
            }

            return _instance;
        }
    }

    #endregion

    #region Setters & Getters (Empty)

    #endregion

    #region Built-In Unity Methods (Empty)

    #endregion

    #region Main Methods

    public static void StartListening(string eventName, UnityAction listener)
    {
        //Declaring local variables
        UnityEvent thisEvent = null;

        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction listener)
    {
        //Just double checking that there is an EventManager object still
        //present in the game. If there isn't then don't execute the rest
        //of this method and return to the last memory address the call was
        //made.
        if (_instance == null)
            return;

        //Declaring local variables
        UnityEvent thisEvent = null;

        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }

    }

    public static void TriggerEvent(string eventName)
    {
        //Declaring local variables
        UnityEvent thisEvent = null;

        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }

    #endregion

    #region Helper Methods

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string,UnityEvent>();
        }
    }

    #endregion
}

