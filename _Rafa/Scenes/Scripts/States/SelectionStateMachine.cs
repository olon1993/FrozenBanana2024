using System;
using UnityEngine;

public abstract class SelectionStateMachine : MonoBehaviour
{
    protected IState CurrentState;
    public event Action<IState> StateChangedEvent;

    protected virtual void Update()
    {
        if (CurrentState != null)
        CurrentState.UpdateState();
    }

    public virtual void ChangeState(IState newState)
    {
        if (CurrentState != null) CurrentState.OnExitState();
        CurrentState = newState;
        print("Newstate: " + CurrentState);
        CurrentState.OnEnterState();
        StateChangedEvent?.Invoke(newState);
    }
}
