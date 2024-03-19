using System;
using System.Collections.Generic;
using UnityEngine;

public class Selection
{
    public Vector3Int Origin {get; set;}
    public CombatTile Terrain {get; set;}
    public IUnits Unit {get; set;}
    public HashSet<Vector3Int> UnitMoveRange {get; set;}
    public HashSet<Vector3Int> UnitAttackRange {get; set;}

}