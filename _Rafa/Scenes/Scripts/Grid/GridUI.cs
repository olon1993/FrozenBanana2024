using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


public class GridUI : MonoBehaviour
{
    [SerializeField] GameObject gridLines;
    [SerializeField] LineRenderer pathLine;
    [SerializeField] OverlayTile overlayTiles;
    [SerializeField] GridSystem GridController;
    [SerializeField] GameObject SelectionIndicator;
    [SerializeField] AudioSource SelectionBeep;
    [SerializeField] Transform _camera;
    [SerializeField] Button _TargetPrefab;
    [SerializeField] Button AttackButton;
    [SerializeField] Button WaitButton;
    [SerializeField] GameObject ActionMenu;
    [SerializeField] GameObject TargetMenu;
    [SerializeField] GameObject TurnTracker;
    [SerializeField] float gridOffset;

    [SerializeField] BattleUIController battleUI;


    void Start()
    {
        CenterCamera();;
    }

    void OnEnable()
    {
        EventListener.GridTileClick += OnTileClick;
        EventListener.GridTileEnter += OnTileEnter;
        EventListener.GridTileExit += OnTileExit;
    }

    void OnDisable()
    {
        EventListener.GridTileClick -= OnTileClick;
        EventListener.GridTileEnter -= OnTileEnter;
        EventListener.GridTileExit -= OnTileExit;
    }

    void CenterCamera()
    {
       
    }

    public void SetUpOverlayTiles(Tilemap overlayMap, BoundsInt size)
    {
        TileBase[] tile = Enumerable.Repeat(overlayTiles, size.size.x * size.size.y).ToArray();
        overlayMap.SetTilesBlock(size, tile);
        overlayMap.CompressBounds();
    }

    #region AREA HIGHLIGHT

    public void HighlightUnitRange(IUnits unit, Vector3Int location)
    {
        (int move, int attack) Ranges = unit.GetRanges();
        (HashSet<Vector3Int> moveArea, HashSet<Vector3Int> attackArea) = PathFinder2D.FindTotalRangeBFS
            (
                location, 
                Ranges.move, 
                Ranges.attack, 
                GridController.MapGrid
            );
        HighlightArea(attackArea, RangeType.ATTACK);
        HighlightArea(moveArea, RangeType.MOVE);
    }

    public void HighlightAttackRange(IUnits unit, Vector3Int location)
    {
        ResetHighlight();
        HashSet<Vector3Int> attackArea = PathFinder2D.FindAttackRangeBFS(location, unit.GetRanges().attack, GridController.MapGrid);
        HighlightArea(attackArea, RangeType.ATTACK);
    }

    void HighlightArea(HashSet<Vector3Int> area, RangeType type)
    {
        overlayTiles.Highlight(area, type);
        GridController.OverlayGrid.RefreshAllTiles();
    }

    public void ResetHighlight()
    {
        overlayTiles.ResetHighlight();
        GridController.OverlayGrid.RefreshAllTiles();
    }

    #endregion
    public void MoveSelector(Vector3Int newLocation)
    {
        SelectionIndicator.transform.position = GridController.MapGrid.GetCellCenterLocal(newLocation);
        SelectionBeep.Play();
    }

    public void DisplayActionMenu(Vector3Int newLocation, IUnits selection)
    {
        ActionMenu.SetActive(true);
        AttackButton.onClick.AddListener(() => DisplayAttackTarget(newLocation, selection));
        WaitButton.onClick.AddListener(() => GridController.WaitAction(selection));
    }

    public void DisplayAttackTarget(Vector3Int position, IUnits attacker)
    {
        HashSet<Vector3Int> attackRange = PathFinder2D.FindAttackRangeBFS(position, attacker.GetRanges().attack, GridController.MapGrid);
        HighlightArea(attackRange, RangeType.ATTACK);
        HashSet<IUnits> units = new HashSet<IUnits>();
        ClearContainer(TargetMenu);

        foreach (Vector3Int location in attackRange)
        {
            if(GridController.UnitMap.TryGetValue(location, out IUnits neighborUnit))
            {
                units.Add(neighborUnit);
                Button button = Instantiate(_TargetPrefab, TargetMenu.transform);
                button.gameObject.GetComponentInChildren<TMP_Text>().text = neighborUnit.GetName();
                button.onClick.AddListener(() => GridController.AttackAction(attacker, neighborUnit, location));
                
            }
        }

        TargetMenu.SetActive(true);
    }

    void ClearContainer(GameObject container)
    {
        for(int x = 0; x < container.transform.childCount; x++)
        {
            Destroy(container.transform.GetChild(x).gameObject);
        }
    }

    public void CloseMenu()
    {
        AttackButton.onClick.RemoveAllListeners();
        WaitButton.onClick.RemoveAllListeners();
        ActionMenu.SetActive(false);
        TargetMenu.SetActive(false);
        ResetHighlight();
    }

    void DrawPathLine(Vector3Int position)
    {
        List<Vector3Int> path = PathFinder2D.FindConstrainedPath
                (
                    GridController.ActiveSelection.Origin, 
                    position, 
                    GridController.MapGrid,
                    GridController.ActiveSelection.UnitMoveRange
                );
        pathLine.gameObject.SetActive(true);
        if(path is not null)
        {
            pathLine.positionCount = path.Count;
            for(int index = 0; index < path.Count; index++)
            {
                pathLine.SetPosition(index, new Vector3(path[index].x + gridOffset, path[index].y + gridOffset, 0));
            }
        }
    }

    public void UpdateTurnTracker(int turnCounter, Team team)
    {
        TMP_Text activeTeam = TurnTracker.transform.GetChild(0).GetComponent<TMP_Text>();
        TMP_Text counter = TurnTracker.transform.GetChild(1).GetComponent<TMP_Text>();
        
        activeTeam.text = "Turn: " + team.ToString();
        counter.text = "Round: " + turnCounter;
    }

    #region EVENT HANDLERS
    public void OnTileClick(Vector3Int position)
    {
        // If unit is selected and move
        if(GridController.SelectionIsActive)
        {   
            GridController.MoveUnit(GridController.ActiveSelection, position);
            // if(GridController.ActiveSelection.Unit.GetMoved()) 
            // {
                pathLine.gameObject.SetActive(false);
                
            //}
        // Activate unit selection
        } else {
            GridController.SelectUnit(position);
        }
    }

    public void OnTileEnter(Vector3Int position)
    {
        // Move Selector to tile position
        MoveSelector(position);

        if(GridController.SelectionIsActive && GridController.ActiveSelection.Unit.GetMoved()) return;

        // If selected unit has not moved, display path arrow.
        if(GridController.SelectionIsActive && !GridController.ActiveSelection.Unit.GetMoved())
        {
            DrawPathLine(position);
        // While no unit is selected, display range for units on hover.
        } else { 
            if(GridController.UnitMap.TryGetValue(position, out IUnits unit) && !unit.GetMoved())
            {
                HighlightUnitRange(unit, position);
                battleUI.DisplayUnitData(unit);
            }
        }

        CombatTile tile = GridController.GetTileAt(position);
        if(tile is not null)
        {
            battleUI.DisplayTileData(tile);
        }
    }

    public void OnTileExit(Vector3Int position)
    {
         // While no unit is selected, reset range for units on hover.
        if(!GridController.SelectionIsActive)
        {
            ResetHighlight();
        }
        battleUI.HideTileData();
        battleUI.HideUnitData();
    }
    #endregion
}
