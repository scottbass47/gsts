using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private Dictionary<Type, List<Action<GameEvent>>> eventTable;

    private void Awake()
    {
        eventTable = new Dictionary<Type, List<Action<GameEvent>>>();
    }

    public void AddListener<T>(Action<T> listener) where T : GameEvent
    {
        Type t = typeof(T);
        List<Action<GameEvent>> listeners;

        if (!eventTable.TryGetValue(t, out listeners))
        {
            listeners = new List<Action<GameEvent>>();
            eventTable.Add(t, listeners);
        }

        Action<GameEvent> action = (obj) =>
        {
            var cast = (T)Convert.ChangeType(obj, t);
            listener(cast);
        };
        listeners.Add(action);
    }

    public void RemoveListener<T>(Action<T> listener) where T : GameEvent
    {
        Type t = typeof(T);
        List<Action<GameEvent>> listeners;

        if (!eventTable.TryGetValue(t, out listeners))
        {
            return;
        }

        Action<GameEvent> action = (obj) =>
        {
            var cast = (T)Convert.ChangeType(obj, t);
            listener(cast);
        };
        listeners.Remove(action);
    }

    public void FireEvent<T>(T gameEvent) where T : GameEvent
    {
        Type t = typeof(T);
        List<Action<GameEvent>> listeners;

        if (!eventTable.TryGetValue(t, out listeners))
        {
            return;
        }

        foreach(var listener in listeners)
        {
            listener(gameEvent);
        }
    }
}

public abstract class GameEvent
{
} 

// For whenever the health changes (damage, or something else)
public class PlayerHealthEvent : GameEvent
{
   public int Health { get; set; } 
}

// Only when the player takes damage from an enemy
public class PlayerDamage : GameEvent { }
