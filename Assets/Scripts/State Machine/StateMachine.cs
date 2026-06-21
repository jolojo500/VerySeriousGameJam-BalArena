using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



/// <summary>
/// Low-level definiton for a state machine. Must be inherited from to produce anything worthwhile
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class StateMachine<T> : MonoBehaviour where T : State
{
    public T CurrentState;
    public T PreviousState;
    public T NextState;

    [SerializeReference]
    public List<T> AvailableStates = new List<T>();

    public virtual void Add(T state)
    {
        if (AvailableStates == null) AvailableStates = new();
        if (AvailableStates.Find(s => s.GetType() == state.GetType()) == null)
        {
            AvailableStates.Add(state);
        }
    }

    public void Initialize<G>()
    {
        CurrentState = AvailableStates.Find(s => s is G);
        PreviousState = CurrentState;
        CurrentState.OnEnter();
    }

    public bool IsCurrentState<G>(bool StrictRequest = false)
    {
        if (StrictRequest)
        {
            return CurrentState.GetType() == typeof(G);
        }
        return CurrentState is G;
    }

    public virtual void Set<G>(bool TriggerEnter = true) where G : T
    {
        NextState = AvailableStates.Find(s => s is G);
        CurrentState.OnExit();
        PreviousState = CurrentState;
        CurrentState = NextState;
        NextState = null;
        if (TriggerEnter) CurrentState.OnEnter();
    }


    public virtual void ForceSet(T forceState)
    {
        if (CurrentState != null)
            CurrentState.OnExit();

        PreviousState = CurrentState;
        CurrentState = (forceState);

        if (CurrentState != null)
            CurrentState.OnEnter();
    }


    public G Get<G>() where G : T
    {
        return AvailableStates.Find((s => s is G)) as G;
    }

}
