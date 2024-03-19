
// WHEN: User has selection action requiring weapon/item selection
using UnityEngine;

public class WeaponSelectState : State
{
    public WeaponSelectState(SelectionStateMachine stateMachine) : base(stateMachine)
    {
        StateMachine = stateMachine;
    }

    public override void OnEnterState()
    {
        // Display unit data and inventory
    }

    public override void OnExitState()
    {
        // Hide unit data and inventory
    }

    public override void UpdateState()
    {
        // OnMouseEnter: Only on inventory box, highlight inventory item.
        // OnMouseExit: Only on inventory box, reset item highlight.
        // OnClick: Activate item and go to target selection. Item determines effect.
    }
}