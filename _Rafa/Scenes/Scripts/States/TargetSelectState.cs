
// WHEN: User has selected a weapon and must select target unit.
public class TargetSelectState : State
{
    public TargetSelectState(SelectionStateMachine stateMachine) : base(stateMachine)
    {
        StateMachine = stateMachine;
    }

    public override void OnEnterState()
    {
        // Display attack range
    }

    public override void OnExitState()
    {
        // Hide attack range
    }

    public override void UpdateState()
    {
        // OnMouseEnter: Determine if square is in range, if so, if there's valid unit, if so, move selector and display combat preview.
        // OnMouseExit: Hide selector and combat preview.
        // OnClick: Execute combat, move to unit deactivation step.
    }
}