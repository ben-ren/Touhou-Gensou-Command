using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//DISCLAIMER: Written with help from AI (I'm not doing all that myself again)
public class AStarPathway : MonoBehaviour
{
    public Tilemap displayTilemap;
    public Tilemap collisionTilemap;

    // Store last path for debugging
    private List<Vector3> debugPath;

    class Node
    {
        public Vector3Int position;
        public int gCost;
        public int hCost;
        public int fCost => gCost + hCost;
        public Node parent;

        public Node(Vector3Int pos)
        {
            position = pos;
        }
    }

    public List<Vector3> FindPath(Vector3 startWorld, Vector3 targetWorld)
    {
        Vector3Int startCell = displayTilemap.WorldToCell(startWorld);
        Vector3Int targetCell = displayTilemap.WorldToCell(targetWorld);

        List<Node> openSet = new();
        HashSet<Vector3Int> closedSet = new();

        Node startNode = new(startCell);
        Node targetNode = new(targetCell);

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                   openSet[i].fCost == currentNode.fCost &&
                   openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode.position);

            if (currentNode.position == targetNode.position)
            {
                debugPath = RetracePath(startNode, currentNode); // store path for debug
                return debugPath;
            }

            if (currentNode.position == targetNode.position)
                return RetracePath(startNode, currentNode);

            foreach (Vector3Int dir in directions)
            {
                Vector3Int neighbourPos = currentNode.position + dir;

                if (!displayTilemap.HasTile(neighbourPos))
                    continue;

                if (collisionTilemap.HasTile(neighbourPos))
                    continue;

                // Block diagonal corners
                if (dir.x != 0 && dir.y != 0)
                {
                    Vector3Int check1 = currentNode.position + new Vector3Int(dir.x, 0, 0);
                    Vector3Int check2 = currentNode.position + new Vector3Int(0, dir.y, 0);
                    if (collisionTilemap.HasTile(check1) || collisionTilemap.HasTile(check2))
                        continue;
                }

                if (closedSet.Contains(neighbourPos))
                    continue;

                int newCost = currentNode.gCost + ((dir.x != 0 && dir.y != 0) ? 14 : 10);

                Node neighbourNode = openSet.Find(n => n.position == neighbourPos);

                if (neighbourNode == null)
                {
                    neighbourNode = new Node(neighbourPos);
                    neighbourNode.gCost = newCost;
                    neighbourNode.hCost = GetDistance(neighbourPos, targetNode.position);
                    neighbourNode.parent = currentNode;
                    openSet.Add(neighbourNode);
                }
                else if (newCost < neighbourNode.gCost)
                {
                    neighbourNode.gCost = newCost;
                    neighbourNode.parent = currentNode;
                }
            }
        }

        debugPath = null;
        return null;
    }

    List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        List<Vector3> path = new();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(displayTilemap.GetCellCenterWorld(currentNode.position));
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    int GetDistance(Vector3Int a, Vector3Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        int straightCost = 10;
        int diagonalCost = 14;

        return straightCost * (dx + dy) + (diagonalCost - 2 * straightCost) * Mathf.Min(dx, dy);
    }

    static Vector3Int[] directions =
    {
        new(1,0,0),
        new(-1,0,0),
        new(0,1,0),
        new(0,-1,0),
        new(1,1,0),
        new(1,-1,0),
        new(-1,1,0),
        new(-1,-1,0)
    };

    // --- Debug: Draw path in Scene view ---
    private void OnDrawGizmos()
    {
        if (debugPath == null || debugPath.Count == 0) return;

        Gizmos.color = Color.red;
        for (int i = 0; i < debugPath.Count; i++)
        {
            Gizmos.DrawSphere(debugPath[i], 0.1f);
            if (i < debugPath.Count - 1)
            {
                Gizmos.DrawLine(debugPath[i], debugPath[i + 1]);
            }
        }
    }
}