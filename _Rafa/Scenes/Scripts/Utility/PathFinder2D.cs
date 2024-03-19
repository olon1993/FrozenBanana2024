using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class PathFinder2D 
{
    class PathNode
    {
        float _dTraveled;
        float _dRemaining;
        List<Vector3Int> _parentNodes;
        public Vector3Int Position;
        public PathNode (Vector3Int current, Vector3Int end, List<Vector3Int> history, float moveCost = 0)
        {
            _dTraveled = moveCost;
            _dRemaining = GetDistance(current, end);
            _parentNodes = history;
            _parentNodes.Add(current);
            Position = current;
        }

        private float GetDistance(Vector3Int start, Vector3Int end)
        {
            return Vector3Int.Distance(start, end);
        }

        public float Traveled()
        {
            return _dTraveled;
        }

        public List<Vector3Int> History()
        {
            return _parentNodes;
        }

        public float Length()
        {
            return _dTraveled + _dRemaining;
        }
    }

    static List<Vector3Int> GetTileNeighborsAt(Vector3Int location, Tilemap grid)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        
        Vector3Int Up = new Vector3Int(location.x, location.y + 1, location.z);
        if (grid.HasTile(Up))
        {
            neighbors.Add(Up);
        }
        Vector3Int Right = new Vector3Int(location.x + 1, location.y, location.z);
        if (grid.HasTile(Right))
        {
            neighbors.Add(Right);
        }
        Vector3Int Down = new Vector3Int(location.x, location.y - 1, location.z);
        if (grid.HasTile(Down))
        {
            neighbors.Add(Down);
        }
        Vector3Int Left = new Vector3Int(location.x - 1, location.y, location.z);
        if (grid.HasTile(Left))
        {
            neighbors.Add(Left);
        }

        return neighbors;
    }

    // Returns path between two 2D locations, ignoring path cost, using Breadth-First.
    public static List<Vector3Int> FindPathBFS(Vector3Int start, Vector3Int end, Tilemap grid)
    {
        // Check self-reference
        if(start == end) return null;

        // Initialize revisit guard, route list, and search queue.
        Dictionary<Vector3Int, bool> checkedTile = new Dictionary<Vector3Int, bool>{{ start, true }};
        List<Vector3Int> pathHistory = new List<Vector3Int>(){ start };
        Queue<(List<Vector3Int> history, Vector3Int location)> SearchList = new Queue<(List<Vector3Int>, Vector3Int)>();
        
        // Add start location's neighbors as first layer to search
        foreach (Vector3Int neighbor in GetTileNeighborsAt(start, grid))
        {
            SearchList.Enqueue((pathHistory, neighbor));
        }
        
        // Main search loop
        while(SearchList.Count > 0)
        {
            // Get location to search in queue
            (List<Vector3Int> history, Vector3Int location) probe = SearchList.Dequeue();
 
            // Check if it has been visited
            if(checkedTile.TryGetValue(probe.location, out bool visited))
            {
                if(visited) continue;
            }

            // Check if it's the target location
            if(probe.location == end)
            {
                probe.history.Add(probe.location);
                return probe.history;
            }

            // If not target location, generate new potential route based on location neighbors
            List<Vector3Int> potentialRoute = new List<Vector3Int>(probe.history);
            potentialRoute.Add(probe.location);

            foreach (Vector3Int neighbor in GetTileNeighborsAt(probe.location, grid))
            {
                SearchList.Enqueue((potentialRoute, neighbor));
            }

            // Add location to list of searched locations and remove it from search list.
            checkedTile.Add(probe.location, true);
        }

        // If loop completes without finding target location, return null
        return null;
    }

    // Returns path between two 2D locations, ignoring path cost, using Depth-First.
    public static List<Vector3Int> FindPathDFS(Vector3Int start, Vector3Int end, Tilemap grid)
    {
         // Check self-reference
        if(start == end) return null;

        // Initialize revisit guard, route list, and search queue.
        Dictionary<Vector3Int, bool> checkedTile = new Dictionary<Vector3Int, bool>{{ start, true }};
        List<Vector3Int> pathHistory = new List<Vector3Int>(){ start };
        Stack<(List<Vector3Int> history, Vector3Int location)> SearchList = new Stack<(List<Vector3Int>, Vector3Int)>();
        
        // Add start location's neighbors as first layer to search
        foreach (Vector3Int neighbor in GetTileNeighborsAt(start, grid))
        {
            SearchList.Push((pathHistory, neighbor));
        }
        
        // Main search loop
        while(SearchList.Count > 0)
        {
            // Get location to search in queue
            (List<Vector3Int> history, Vector3Int location) probe = SearchList.Pop();
 
            // Check if it has been visited
            if(checkedTile.TryGetValue(probe.location, out bool visited))
            {
                if(visited) continue;
            }

            // Check if it's the target location
            if(probe.location == end)
            {
                probe.history.Add(probe.location);
                return probe.history;
            }

            // If not target location, generate new potential route based on location neighbors
            List<Vector3Int> potentialRoute = new List<Vector3Int>(probe.history)
            {
                probe.location
            };

            foreach (Vector3Int neighbor in GetTileNeighborsAt(probe.location, grid))
            {
                SearchList.Push((potentialRoute, neighbor));
            }

            // Add location to list of searched locations and remove it from search list.
            checkedTile.Add(probe.location, true);
        }

        // If loop completes without finding target location, return null
        return null;
    }

    // Returns shortest path between two 2D locations, accounting for movement cost.
    public static List<Vector3Int> FindPathAStar(Vector3Int start, Vector3Int end, Tilemap grid)
    {

        // Check self-reference
        if(start == end)
        {   
            return null;
        }

        // Initialize revisit guard, route list, and search variables.
        HashSet<Vector3Int> checkedTile = new HashSet<Vector3Int>{{ start }};
        PathNode path = new PathNode(start, end, new List<Vector3Int>());
        List<PathNode> SearchList = new List<PathNode>();   // Using List to maintain order and sort.
        
        // Add start location's neighbors as first layer to search
        foreach (Vector3Int neighbor in GetTileNeighborsAt(path.Position, grid))
        {
            CombatTile tile = (CombatTile)grid.GetTile(neighbor);
            PathNode newNode = new PathNode(neighbor, end, new List<Vector3Int>(path.History()), path.Traveled() + tile.GetMovementCost());
            SearchList.Add(newNode);
        }
        
        int limit = 1;
        // Main search loop
        while(SearchList.Count > 0 && limit < 1000)
        {
            limit += 1;
            if(limit >= 1000) Debug.Log("Limit Reached");

            // Get location to search in queue
            PathNode probe = SearchList[0];
            SearchList.RemoveAt(0);
            checkedTile.Add(probe.Position);

            // Check if it's the target location
            if(probe.Position == end) return probe.History();

            // If not target location, generate new potential route based on location neighbors
            foreach (Vector3Int neighbor in GetTileNeighborsAt(probe.Position, grid))
            {
                if(!checkedTile.Contains(neighbor)) 
                {
                    CombatTile tile = (CombatTile)grid.GetTile(neighbor);
                    PathNode newNode = new PathNode(neighbor, end, new List<Vector3Int>(probe.History()), probe.Traveled() + tile.GetMovementCost());
                    SearchList.Add(newNode);
                }
            }

            // Sort Search list.
            SearchList.Sort( (k, v) => k.Length().CompareTo(v.Length()) );
        }

        // If loop completes without finding target location, return null
        return null;
    }

    // Returns shortest path between two 2D locations, accounting for movement cost. Returned List<Vector2> is ordered.
    public static List<Vector3Int> FindConstrainedPath(Vector3Int start, Vector3Int end, Tilemap grid, HashSet<Vector3Int> constraint)
    {
        // Check self-reference
        if(start == end)
        {   
            return null;
        }

        // Initialize revisit guard, route list, and search variables.
        HashSet<Vector3Int> checkedTile = new HashSet<Vector3Int>{{ start }};
        PathNode path = new PathNode(start, end, new List<Vector3Int>());
        List<PathNode> SearchList = new List<PathNode>();
        
        // Add start location's neighbors as first layer to search
        foreach (Vector3Int neighbor in GetTileNeighborsAt(path.Position, grid))
        {
            if(constraint.Contains(neighbor))
            {
                CombatTile tile = (CombatTile)grid.GetTile(neighbor);
                PathNode newNode = new PathNode(neighbor, end, new List<Vector3Int>(path.History()), path.Traveled() + tile.GetMovementCost());
                SearchList.Add(newNode);
            }
        }
        
        int limit = 1;
        // Main search loop
        while(SearchList.Count > 0 && limit < 1000)
        {
            limit += 1;
            //if(limit >= 1000) Debug.Log("Limit Reached");

            // Get location to search in queue
            PathNode probe = SearchList[0];
            SearchList.RemoveAt(0);
            checkedTile.Add(probe.Position);

            // Check if it's the target location
            if(probe.Position == end) 
            {
                //Debug.Log(limit.ToString() + " iterations");
                return probe.History();
            }

            // If not target location, generate new potential route based on location neighbors
            foreach (Vector3Int neighbor in GetTileNeighborsAt(probe.Position, grid))
            {
                if(constraint.Contains(neighbor))
                {
                    if(!checkedTile.Contains(neighbor)) 
                    {
                        CombatTile tile = (CombatTile)grid.GetTile(neighbor);
                        PathNode newNode = new PathNode(neighbor, end, new List<Vector3Int>(probe.History()), probe.Traveled() + tile.GetMovementCost());
                        SearchList.Add(newNode);
                    }
                }
            }

            // Sort Search list.
            SearchList.Sort( (k, v) => k.Length().CompareTo(v.Length()) );
        }

        // If loop completes without finding target location, return null
        return null;
    }
    
    // Find move range accounting for tile movement cost. Returned Vector2's order doesn't matter.
    public static HashSet<Vector3Int> FindMoveRangeBFS(Vector3Int start, int allowance, Tilemap grid)
    {
        if(allowance <= 0) return new HashSet<Vector3Int>(){start};        

        // Initialize revisit guard, route list, and search queue.
        Queue<(int movement, Vector3Int location)> SearchList = new Queue<(int, Vector3Int)>();
        HashSet<Vector3Int> allowedTiles = new HashSet<Vector3Int>(){start};
        
        // Add start location's neighbors as first layer to search
        foreach (Vector3Int neighbor in GetTileNeighborsAt(start, grid))
        {
            CombatTile tile = (CombatTile)grid.GetTile(neighbor);
            SearchList.Enqueue((tile.GetMovementCost(), neighbor));
        }
        
        int limit = 0;
        // Main search loop
        while(SearchList.Count > 0)
        {
            limit += 1;
            // Get location to search in queue
            (int movement, Vector3Int location) probe = SearchList.Dequeue();
 
            // Check if it has been visited
            if(allowedTiles.Contains(probe.location)) continue;
            
            // Check if the target can be reached
            if(probe.movement <= allowance)
            {
                allowedTiles.Add(probe.location);
                // generate new potential tiles based on location neighbors
                foreach (Vector3Int neighbor in GetTileNeighborsAt(probe.location, grid))
                {
                    if(!allowedTiles.Contains(neighbor))
                    {
                        CombatTile tile = (CombatTile)grid.GetTile(neighbor);
                        SearchList.Enqueue((probe.movement + tile.GetMovementCost(), neighbor));
                    }
                }
            }
        }

        //Debug.Log(limit.ToString() + " iterations");
        return allowedTiles;
    }

    // Find attack range by counting number of tiles. Returned Vector3Int's order doesn't matter.
    public static HashSet<Vector3Int> FindAttackRangeBFS(Vector3Int start, int numTiles, Tilemap grid)
    {
        if(numTiles <= 0) return new HashSet<Vector3Int>();        

        // Initialize revisit guard, route list, and search queue.
        Queue<(int distance, Vector3Int location)> SearchList = new Queue<(int, Vector3Int)>();
        HashSet<Vector3Int> allowedTiles = new HashSet<Vector3Int>();
        
        // Add start location's neighbors as first layer to search
        foreach (Vector3Int neighbor in GetTileNeighborsAt(start, grid))
        {
            SearchList.Enqueue((1, neighbor));
        }
        
        int limit = 0;
        // Main search loop
        while(SearchList.Count > 0)
        {
            limit += 1;
            // Get location to search in queue
            (int distance, Vector3Int location) probe = SearchList.Dequeue();
 
            // Check if it has been visited
            if(allowedTiles.Contains(probe.location)) continue;
            
            // Check if the target can be reached
            if(probe.distance <= numTiles)
            {
                allowedTiles.Add(probe.location);
                // generate new potential tiles based on location neighbors
                foreach (Vector3Int neighbor in GetTileNeighborsAt(probe.location, grid))
                {
                    if(!allowedTiles.Contains(neighbor))
                    {
                        SearchList.Enqueue((probe.distance + 1, neighbor));
                    }
                    
                }
            }
        }

        //Debug.Log(limit.ToString() + " iterations");
        // Remove start tile
        allowedTiles.Remove(start);
        return allowedTiles;
    }

    // Find move and attack range accounting for tile movement cost.
    public static (HashSet<Vector3Int> move, HashSet<Vector3Int> attack) FindTotalRangeBFS(Vector3Int start, int moveAllowance, int attackAllowance, Tilemap grid)
    {
        if(moveAllowance <= 0) return (new HashSet<Vector3Int>(){start}, FindAttackRangeBFS(start, attackAllowance, grid));        

        // Initialize revisit guard, route list, and search queue.
        Queue<(int movement, Vector3Int location)> SearchList = new Queue<(int, Vector3Int)>();
        HashSet<Vector3Int> moveTiles = new HashSet<Vector3Int>(){start};
        HashSet<Vector3Int> attackTiles = new HashSet<Vector3Int>();
        
        // Add start location's neighbors as first layer to search
        foreach (Vector3Int neighbor in GetTileNeighborsAt(start, grid))
        {
            CombatTile tile = (CombatTile)grid.GetTile(neighbor);
            SearchList.Enqueue((tile.GetMovementCost(), neighbor));
        }
        
        int limit = 0;
        // Main search loop
        while(SearchList.Count > 0)
        {
            limit += 1;
            // Get location to search in queue
            (int movement, Vector3Int location) probe = SearchList.Dequeue();
 
            // Check if it has been visited
            if(moveTiles.Contains(probe.location)) continue;
            
            // Check if the target can be reached
            if(probe.movement <= moveAllowance)
            {
                moveTiles.Add(probe.location);
                attackTiles.UnionWith(FindAttackRangeBFS(probe.location, attackAllowance, grid));
                // generate new potential tiles based on location neighbors
                foreach (Vector3Int neighbor in GetTileNeighborsAt(probe.location, grid))
                {
                    if(!moveTiles.Contains(neighbor))
                    {
                        CombatTile tile = (CombatTile)grid.GetTile(neighbor);
                        SearchList.Enqueue((probe.movement + tile.GetMovementCost(), neighbor));
                    }
                }
            }
        }

        // Remove move tiles from attack tiles
        //attackTiles.ExceptWith(moveTiles);
        return (moveTiles, attackTiles);
    }
}
