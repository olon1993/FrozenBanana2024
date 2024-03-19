
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Assets/Tiles/TerrainTile")]
public class CombatTile : TileBase
{
    // Set up data
    public string TileName;
    public Sprite TileSprite;
    public int MovementCost;
    public int DefenseBonus;
    public int AvoidBonus;

    public string GetName()
    {
        return TileName;
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = TileSprite;   
    }
    public void SetMovementCost(int cost)
    {
        MovementCost = cost;
    }

    public int GetMovementCost()
    {
        return MovementCost;
    }

    public (int avoidBonus, int defenseBonus) GetTileBonus()
    {
        return (AvoidBonus, DefenseBonus);
    }
}
