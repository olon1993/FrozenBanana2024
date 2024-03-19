using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
[RequireComponent(typeof(EventListener))]
public class MouseTracker : MonoBehaviour
{
    Vector3Int currentCell;
    Tilemap mapGrid;
    [SerializeField] Camera worldCamera;


    void Start()
    {
        mapGrid = gameObject.GetComponent<Tilemap>();
        if(mapGrid is null)
        {
            Debug.LogError("Tilemap component not found.");
        }
    }

    void Update()
    {
        // Get Mouse world coordinates
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Obtain tilemap cell
        Vector3Int cellPosition = mapGrid.WorldToCell(mousePosition);

        if(Input.GetMouseButtonDown(0))
        {
            EventListener.CallTileClick(cellPosition);
        } else {
             if(cellPosition != currentCell)
            {
                // Trigger Mouse Enter/Exit Events
                EventListener.CallTileExit(currentCell);
                EventListener.CallTileEnter(cellPosition);
                currentCell = cellPosition;
            }
        }
    }
}
