using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Configurations/Battle Configuration")]
public class BattleConfig : ScriptableObject
{
    [Serializable]
    public struct Combatant 
    {
        public Vector3Int startingLocation;
        public CombatUnit unit;
    }
    public List<Combatant> playerUnits;
    public List<Combatant> allyUnits;
    public List<Combatant> enemyUnits;
    public List<Combatant> neutralUnits;

}