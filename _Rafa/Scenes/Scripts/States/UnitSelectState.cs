
// WHEN: User has not selected a unit and there are unactivated units left.
using UnityEngine;

public class UnitSelectState : State
{
    public UnitSelectState(SelectionStateMachine stateMachine) : base(stateMachine)
    {
        StateMachine = stateMachine;
    }

    public override void OnEnterState()
    {
        // Hide any overlay tile highlight
    }

    public override void OnExitState()
    {
        // Hide unit/terrain tile data
    }

    public override void UpdateState()
    {
        // OnMouseEnter: Display terrain tile data and unit data. If Ready unit, display unit move/attack range.
        // OnMouseExit: Hide unit data, hide overlay highlight
        // OnClick: If Ready unit, move to UnitMove, else display menu.
    }
}