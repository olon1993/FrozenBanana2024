using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridSystem : MonoBehaviour
{
    [SerializeField] BattleConfig _configuration;
    [SerializeField] GridUI GridDrawer;
    [SerializeField] BattleSystem battleController;
    public Tilemap MapGrid;
    public Tilemap OverlayGrid;
    public Dictionary<Vector3Int, IUnits> UnitMap;
    public Transform unitContainer;
    public Selection ActiveSelection;
    public bool SelectionIsActive;
    TurnManager turnManager;
    
    void Start()
    {
        // Initialize map grid
        if(MapGrid is null || OverlayGrid is null) {
            Debug.LogError("Error: Map or Overlay is missing."); 
            return;
        }
        MapGrid.CompressBounds();
        GridDrawer.SetUpOverlayTiles(OverlayGrid, MapGrid.cellBounds);
        UnitMap = new Dictionary<Vector3Int, IUnits>();

        // Spawn combatants
        // PLAYER UNITS
        foreach (BattleConfig.Combatant combatant in _configuration.playerUnits)
        {
            SpawnUnit(combatant.startingLocation, combatant.unit);
        }
        // ALLY UNITS
        foreach (BattleConfig.Combatant combatant in _configuration.allyUnits)
        {
            SpawnUnit(combatant.startingLocation, combatant.unit);
        }
        // ENEMY UNITS
        foreach (BattleConfig.Combatant combatant in _configuration.enemyUnits)
        {
            SpawnUnit(combatant.startingLocation, combatant.unit);
        }
        // NEUTRAL UNITS
        foreach (BattleConfig.Combatant combatant in _configuration.neutralUnits)
        {
            SpawnUnit(combatant.startingLocation, combatant.unit);
        }

        // Initialize turn tracker
        turnManager = new TurnManager(new HashSet<IUnits>(UnitMap.Values), GridDrawer);
        turnManager.TurnStart(Team.PLAYER);
    }
    public void SpawnUnit(Vector3Int location, IUnits unit)
    {
        CombatUnit spawnedUnit = Instantiate((CombatUnit)unit, unitContainer);
        UnitMap[location] = spawnedUnit;
        spawnedUnit.InitializeUnit();

        spawnedUnit.transform.position = MapGrid.GetCellCenterLocal(location);
    }

    #region UNIT ACTIVATION
    public void SelectUnit(Vector3Int position)
    {
        // Determine if unactivated unit present at location.
        if(UnitMap.TryGetValue(position, out IUnits unit) && unit.GetReady())
        {
            // If ready unit present, set UnitSelect active and populate selection data. 
            SelectionIsActive = true;
            (int move, int attack) Ranges = unit.GetRanges();
            (HashSet<Vector3Int> move, HashSet<Vector3Int> attack) range =   PathFinder2D.FindTotalRangeBFS(position, Ranges.move, Ranges.attack, MapGrid);
            ActiveSelection = new Selection() 
            {
                Origin = position,
                Terrain = GetTileAt(position),
                Unit = unit,
                UnitAttackRange = range.attack,
                UnitMoveRange = range.move
            };
        }
    }
    public void CancelUnitSelect()
    {
        SelectionIsActive = false;
        ActiveSelection = null;
        GridDrawer.ResetHighlight();
    }
    public void MoveUnit(Selection selection, Vector3Int newLocation)
    {
        // Check unit has not moved
        if(selection.Unit.GetMoved()) return;
      
        // Check destination does not contain unit
        if(selection.Origin != newLocation && UnitMap.TryGetValue(newLocation, out IUnits _))
        {
            Debug.Log("Error: Space occupied by another unit.");
            return;
        }
   
        // Check if new location is in move range and execute move or cancel selection.
        if(selection.UnitMoveRange.Contains(newLocation))
        {
            IUnits unit = selection.Unit;
            UnitMap[newLocation] = unit;
            UnitMap.Remove(selection.Origin);
        
            //((CombatUnit)unit).transform.position = MapGrid.GetCellCenterLocal(newLocation);; 
            StartCoroutine(MoveAnimation(newLocation));
            unit.SetMoved();
            
            SelectAction(selection, newLocation);
            GridDrawer.HighlightAttackRange(unit, newLocation);
        } else {
            CancelUnitSelect();
        }
    }
    IEnumerator MoveAnimation(Vector3Int endPoint)
    {
        float moveSpeed = 10f;
        List<Vector3Int> path = PathFinder2D.FindConstrainedPath
            (
                ActiveSelection.Origin, 
                endPoint, 
                MapGrid,
                ActiveSelection.UnitMoveRange
            );

       for (int i = 1; i < path.Count; i++)
        {
            Vector3 currentPos = ((CombatUnit)ActiveSelection.Unit).transform.localPosition;
            Vector3 direction = path[i] - currentPos;
            int facing = Mathf.RoundToInt(direction.x + direction.y * 10);

            if(facing != 0)
            {
                ((CombatUnit)ActiveSelection.Unit).transform.GetComponent<Animator>().SetInteger("Direction",  facing);
                ((CombatUnit)ActiveSelection.Unit).transform.GetComponent<SpriteRenderer>().flipX = Mathf.RoundToInt(direction.x) == 1;
            }
            
            while(currentPos != path[i])
            {
                ((CombatUnit)ActiveSelection.Unit).transform.localPosition += direction * 1 / moveSpeed;
                currentPos = ((CombatUnit)ActiveSelection.Unit).transform.localPosition;
                yield return new WaitForSeconds(0.02f);
            }
        }
        
        ((CombatUnit)ActiveSelection.Unit).transform.GetComponent<Animator>().SetInteger("Direction", 0);
        ((CombatUnit)ActiveSelection.Unit).transform.GetComponent<SpriteRenderer>().flipX = false;
    }
     public void SelectAction(Selection selection, Vector3Int newLocation)
    {
        // Determine available actions

        // Display Action UI
        GridDrawer.DisplayActionMenu(newLocation, selection.Unit);
        // Handle action select
    }
    public void AttackAction(IUnits attacker, IUnits defender, Vector3Int defendTile)
    {
        CombatStatistic attackerStats = attacker.GetStatistics();
        CombatStatistic defenderStats = defender.GetStatistics();
        if(attackerStats is null || defenderStats is null) return;

        (int tileAvoid, int tileDefense) = GetTileAt(defendTile).GetTileBonus();
        int damage = Math.Max(attackerStats.Attack - (defenderStats.Defense + tileDefense), 0);
        defender.ChangeHP(-damage); 
        Debug.Log("You have dealt " + damage + " points of damage.");
        Debug.Log("Defender's HP: " + defender.GetCurrentHealth().HP);

        CompleteUnitAction(attacker);
    }
    public void WaitAction(IUnits unit)
    {
        CompleteUnitAction(unit);
    }
    void CompleteUnitAction(IUnits unit)
    {
        unit.DeactivateUnit();
        GridDrawer.CloseMenu();
        turnManager.ActivatedUnit(unit);
        SelectionIsActive = false;
        ActiveSelection = null;
    }
    #endregion
    public CombatTile GetTileAt(Vector3Int position)
    {
        return (CombatTile)MapGrid.GetTile(position);
    }
}
