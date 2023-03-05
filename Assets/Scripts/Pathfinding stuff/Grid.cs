using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    //hahaha lmfao almost all of this code aint mine
    public Transform player;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRaduis;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    public List<Node> funky;

    //test thing to move grid
    public void Update()
    {
        foreach (Node n in grid)
        {
            n.worldPosition = transform.position - n.offsetFromMainParent;
            if (player != null)
            {
                if (Vector3.Distance(player.position, transform.position) <= 100)
                {
                    n.walkable = !Physics.CheckCapsule(n.worldPosition, n.worldPosition + Vector3.up * 0.01f, nodeRaduis, unwalkableMask);
                }
                else
                {
                    n.walkable = true;
                }
            }
        }
    }

    private void Start()
    {
        nodeDiameter = nodeRaduis * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
        CreateGrid();

    }
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - new Vector3(gridWorldSize.x/2, -3 , gridWorldSize.y/2);
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRaduis) + Vector3.forward * (y * nodeDiameter + nodeRaduis);
                grid[x,y] =  new Node(false, worldPoint, x, y);
                grid[x, y].mainParent = transform;
                grid[x, y].offsetFromMainParent = transform.position - grid[x, y].worldPosition;
            }
        }
    }

    public List<Node> GetNeighBours(Node node)
    {
        List<Node> neightbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >=0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neightbours.Add(grid[checkX, checkY]);
                }
            }
        }
        funky = neightbours;
        return neightbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPostition)
    {
        worldPostition = worldPostition - transform.position;
        float percentX = (worldPostition.x/ gridWorldSize.x) + 0.5f;
        float percentY = (worldPostition.z/ gridWorldSize.y) + 0.5f;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        //Debug.Log("X = " + percentX + " Y = " + percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public List<Node> path;
    void OnDrawGizmos()
    {
        if (true)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 10, gridWorldSize.y));

            if (grid != null)
            {
                Node playerNode = NodeFromWorldPoint(player.position);
                foreach (Node n in grid)
                {
                    Gizmos.color = (!n.walkable) ? Color.red : Color.clear;
                    if (playerNode == n)
                    {
                        Gizmos.color = Color.cyan;
                    }
                    if (path != null)
                    {

                        if (path.Contains(n))
                        {
                            Gizmos.color = Color.black;
                        }
                    }
                    Gizmos.DrawCube(n.worldPosition + Vector3.down * 2, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }
    }
}
