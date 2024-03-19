
// WHEN: User has moved a unit and must select an action.
public class UnitActionState : State
{
    public UnitActionState(SelectionStateMachine stateMachine) : base(stateMachine)
    {
        StateMachine = stateMachine;
    }

    public override void OnEnterState()
    {
        // Display action menu
    }

    public override void OnExitState()
    {
        // Hide action menu
    }

    public override void UpdateState()
    {
        // OnMouseEnter: Nothing
        // OnMouseExit: Nothing.
        // OnClick: Nothing. Player must click on an action within action menu.
    }
}