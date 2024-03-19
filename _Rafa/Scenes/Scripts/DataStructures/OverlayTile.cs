using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Assets/Tiles/OverlayTile")]
public class OverlayTile : TileBase
{
    [SerializeField] Sprite baseColor;
    [SerializeField] Sprite blueHighlight;
    [SerializeField] Sprite redHighlight;
    [SerializeField] Sprite greenHighlight;
    Dictionary<Vector3Int, RangeType> HighlightState = new Dictionary<Vector3Int, RangeType>();
    
    public string GetName()
    {
        return "Overlay Tile";
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        if(HighlightState.TryGetValue(position, out RangeType type))
        {
            switch (type)
            {
                case RangeType.ATTACK:
                    tileData.sprite = redHighlight;
                    break;
                case RangeType.MOVE:
                    tileData.sprite = blueHighlight;
                    break;
                case RangeType.SUPPORT:
                    tileData.sprite = greenHighlight;
                    break;
                default:
                    tileData.sprite = baseColor;
                    break;
            }
        } else {
            tileData.sprite = baseColor;
        }
    }
    public void Highlight(HashSet<Vector3Int> positions, RangeType type)
    {
        foreach (Vector3Int position in positions)
        {
            //Debug.Log(position.ToString());
            HighlightState[position] = type;                                    
        }
    }

    public void ResetHighlight()
    {
        HighlightState = new Dictionary<Vector3Int, RangeType>();
    }
}
