using System;
using UnityEngine;

public class EventListener
{
    public delegate void GridSelect(Vector3Int position);
    public static event GridSelect GridTileClick;
    public static void CallTileClick(Vector3Int position)
    {
        GridTileClick?.Invoke(position);
    }

    public static event GridSelect GridTileEnter;
    public static void CallTileEnter(Vector3Int position)
    {
        GridTileEnter?.Invoke(position);
    }

    public static event GridSelect GridTileExit;
    public static void CallTileExit(Vector3Int position)
    {
        GridTileExit?.Invoke(position);
    }

    public static event GridSelect TerrainSelected;
    public static void CallTerrainSelect(Vector3Int position)
    {
        GridTileExit?.Invoke(position);
    }

    public static event GridSelect UnitSelected;
    public static void CallUnitSelect(Vector3Int position)
    {
        GridTileExit?.Invoke(position);
    }

}
