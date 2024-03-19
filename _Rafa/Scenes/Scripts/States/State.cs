public abstract class State : IState
{
    protected SelectionStateMachine StateMachine;

    public State(SelectionStateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }

    public virtual void OnEnterState()
    {
    }

    public virtual void OnExitState()
    {
    }

    public virtual void UpdateState()
    {
    }
}