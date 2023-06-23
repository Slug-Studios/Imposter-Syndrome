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
    public Vector3 gridCenter;
    public int updateOffset;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    public int MaxSize
    {
        get {
            return gridSizeX * gridSizeY; 
        }
    }

    //move grid
    private void Updategrid()
    {
        gridCenter = transform.position;
        bool inDistance = false;
        if (Vector3.Distance(player.position, gridCenter) <= 200)
        {
            inDistance = true;
        }
        foreach (Node n in grid)
        {
            n.worldPosition = gridCenter - n.offsetFromMainParent;
            if (inDistance)
            {
                n.walkable = !Physics.CheckSphere(n.worldPosition, nodeRaduis, unwalkableMask);
            }
            else
            {
                n.walkable = true;
            }
        }
    }

    private void Start()
    {
        nodeDiameter = nodeRaduis * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
        CreateGrid();
        InvokeRepeating("Updategrid", updateOffset, 5);
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
    //optimizing away GetNeighbours memory allocation entirely (this adds 96 bytes to a grid's size)
    //feel free to give them method-specific names (like GetNeighBoursVarNeighbours)
    //PS looks like I may have lied and this may not be working
    //Oh I guess the compiler might be doing this anyway, it's probably just the node variable, let's fix that
    private Node[] neighbours = new Node[8];
    public int neighboursIndex;
    private int checkX;
    private int checkY;

    public Node[] GetNeighBours(in Node node)
    {
        neighbours = new Node[8];//can only have eight neighbours anyway, and list slow
        neighboursIndex = 0;// however we require another var to allow appending (still faster than list)
        checkX = 0;
        checkY = 0;
        for (int x = -1; x <= 1; x++)
        {
            checkX = node.gridX + x;
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                checkY = node.gridY + y;
                //int checkX = node.gridX + x;//don't think these have to be in the for loop
                //int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours[neighboursIndex] = (grid[checkX, checkY]);
                    neighboursIndex++;
                }
            }
        }
        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPostition)
    {
        worldPostition = worldPostition - gridCenter;
        float percentX = (worldPostition.x/ gridWorldSize.x) + 0.5f;
        float percentY = (worldPostition.z/ gridWorldSize.y) + 0.5f;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        //Debug.Log("X = " + percentX + " Y = " + percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    //public List<Node> path;
    void OnDrawGizmos()
    {
        /**
            Gizmos.DrawWireCube(gridCenter, new Vector3(gridWorldSize.x, 10, gridWorldSize.y));

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
        /**/
    }
}
