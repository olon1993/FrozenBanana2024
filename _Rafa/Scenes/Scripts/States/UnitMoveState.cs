
// WHEN: User has selected a unit and must choose where to move them.
public class UnitMoveState : State
{
    public UnitMoveState(SelectionStateMachine stateMachine) : base(stateMachine)
    {
        StateMachine = stateMachine;
    }

    public override void OnEnterState()
    {
        // Display move/attack range
        // Set unit at origin tile
    }

    public override void OnExitState()
    {
        // Hide move/attack range
    }

    public override void UpdateState()
    {
        // OnMouseEnter: Determine if square is in move range, if so, display path arrow between unit and tile.
        // OnMouseExit: Hide path arrow.
        // OnClick: Execute move to new tile and move to 'ActionSelect' state.
    }
}