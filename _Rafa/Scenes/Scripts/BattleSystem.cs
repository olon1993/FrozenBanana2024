using System;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{

    [SerializeField] GridSystem gridController;

    public void AttackAction(IUnits attacker, IUnits defender, Vector3Int defendTile)
    {
        CombatStatistic attackerStats = attacker.GetStatistics();
        CombatStatistic defenderStats = defender.GetStatistics();
        if(attackerStats is null || defenderStats is null) return;

        (int tileAvoid, int tileDefense) = gridController.GetTileAt(defendTile).GetTileBonus();
        int damage = Math.Max(attackerStats.Attack - (defenderStats.Defense + tileDefense), 0);
        defender.ChangeHP(-damage); 
        Debug.Log("You have dealt " + damage + " points of damage.");
        Debug.Log("Defender's HP: " + defender.GetCurrentHealth().HP);

        //CompleteUnitAction(attacker);
    }
}