using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private Dictionary<Type, List<EventPair>> eventTable;

    private void Awake()
    {
        eventTable = new Dictionary<Type, List<EventPair>>();
    }

    public void AddListener<T>(GameObject go, Action<T> listener) where T : GameEvent
    {
        Type t = typeof(T);
        List<EventPair> listeners;

        if (!eventTable.TryGetValue(t, out listeners))
        {
            listeners = new List<EventPair>();
            eventTable.Add(t, listeners);
        }

        Action<GameEvent> action = (obj) =>
        {
            var cast = (T)Convert.ChangeType(obj, t);
            listener(cast);
        };
        var eventPair = new EventPair { go = go, eventAction = action };
        listeners.Add(eventPair);
    }

    //public void RemoveListener<T>(Action<T> listener) where T : GameEvent
    //{
    //    Type t = typeof(T);
    //    List<Action<GameEvent>> listeners;

    //    if (!eventTable.TryGetValue(t, out listeners))
    //    {
    //        return;
    //    }

    //    Action<GameEvent> action = (obj) =>
    //    {
    //        var cast = (T)Convert.ChangeType(obj, t);
    //        listener(cast);
    //    };
    //    listeners.Remove(action);
    //}

    public void FireEvent<T>(T gameEvent) where T : GameEvent
    {
        Type t = typeof(T);
        List<EventPair> listeners;

        if (!eventTable.TryGetValue(t, out listeners))
        {
            return;
        }

        for(int i = listeners.Count - 1; i >= 0; i--)
        {
            var listener = listeners[i];
            if(listener.go == null)
            {
                listeners.RemoveAt(i);
                continue;
            }
            listener.eventAction(gameEvent);
        }
    }
}

public class EventPair
{
    public GameObject go;
    public Action<GameEvent> eventAction;
}

public abstract class GameEvent
{
} 

// For whenever the health changes (damage, or something else)
public class PlayerHealthEvent : GameEvent
{
   public int Health { get; set; } 
}

public class PlayerSpawn : GameEvent
{
    public GameObject Player { get; set; }
}

// Only when the player takes damage from an enemy
public class PlayerDamage : GameEvent { }

public class PlayerStartMoving : GameEvent { }

public class PlayerStopMoving : GameEvent { }

public class WeaponFired : GameEvent { }

public class Reload : GameEvent
{
    public float Duration { get; set; }
}

public class WeaponClipChange : GameEvent
{
    public int BulletsInClip { get; set; }
}

public class WeaponMagChange : GameEvent
{
    public int Bullets { get; set; }
}

// Waves

public class WaveStarted : GameEvent
{
    public int WaveNum { get; set; }
}

public class WaveEnded : GameEvent { }

public class WaveEnemyChange : GameEvent
{
    public int EnemiesLeft { get; set; }
}

// Level

public class LevelBranchEnter : GameEvent
{
    public LevelBranch LevelBranch { get; set; }
}

public class LevelBranchExit : GameEvent
{
    public LevelBranch LevelBranch { get; set; }

    // The level being exited to
    public LevelScript Level { get; set; }
}

public class LevelChange : GameEvent
{
}
