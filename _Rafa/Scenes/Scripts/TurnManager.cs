using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public int TurnCounter;
    public HashSet<IUnits> Combatants;
    public Team currentActor;
    HashSet<IUnits> currentUnits;
    GridUI GridDrawer;

    public TurnManager(HashSet<IUnits> combatants, GridUI drawer)
    {
        Combatants = combatants;
        GridDrawer = drawer;
    }
    public void TurnStart(Team actor)
    {
        GridDrawer.UpdateTurnTracker(TurnCounter + 1, actor);
        currentActor = actor;
        currentUnits = GetActorCombatants(actor);
        
        if(actor == Team.NONE) 
        {
            TurnCounter += 1;
            TurnEnd();
        }

        foreach (IUnits unit in currentUnits)
        {
            unit.ActivateUnit();
        }

        if(currentUnits.Count == 0) TurnEnd();
    }

    public void ActivatedUnit(IUnits unit)
    {
        currentUnits.Remove(unit);
        if(currentUnits.Count == 0) TurnEnd();
    }

    public void TurnEnd()
    {
        Debug.Log("Ending turn for " + currentActor.ToString());
        int next = ((int) currentActor + 1) % 4;
        Team nextActor = (Team) next;
        TurnStart(nextActor);
    }

    public HashSet<IUnits> GetActorCombatants(Team actor)
    {
        HashSet<IUnits> actorUnits = new HashSet<IUnits>();
        foreach (CombatUnit unit in Combatants)
        {   
            if(unit.UnitTeam == actor)
            {
                actorUnits.Add(unit);
            }
        }
        return actorUnits;
    }
}
