using System;
using UnityEngine;
using UnityEngine.Events;

public class StateMachine<T> where T : struct, IConvertible, IComparable {

    public string StateMachineName = "";
    public bool debug = false;

    T state;
    public T CurrentState { get { return state; } }
    public static bool operator ==(StateMachine<T> stMachine, T state)
    {
        return stMachine.CurrentState.CompareTo(state) == 0;
    }
    public static bool operator !=(StateMachine<T> stMachine, T state)
    {
        return stMachine.CurrentState.CompareTo(state) != 0;
    }


    public void SetState(T newState, bool forceEvent = false)
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("T must be an enumerated type");
        }

        T oldState = state;
        if (debug)
            Debug.Log(StateMachineName + "STATE from: " + state.ToString() + " to: " + newState.ToString());
        state = newState;      
        if (!oldState.Equals(state) || forceEvent)
        {
            onStateChanged.Invoke(state);
            onStateChanged2.Invoke(oldState, state);
        }
    }


    public class StateChangedEvent : UnityEvent<T> { };
    public StateChangedEvent onStateChanged = new StateChangedEvent();

    public class StateChangedEvent2 : UnityEvent<T,T> { };
    public StateChangedEvent2 onStateChanged2 = new StateChangedEvent2();
}
