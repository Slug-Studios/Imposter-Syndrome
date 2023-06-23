using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public Grid grid;
    public Transform player;
    public Transform target;
    public List<Node> Path;
    public List<Vector3> simplePath;
    private void Start()
    {
        grid = GetComponent<Grid>();
    }
    //most of this script ain't mine
    public void FindPath(Vector3 startPos, Vector3 targetPos)//main lag cause
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode == targetNode)
        {
            Destroy(target.gameObject);
            return;
        }
        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);
        while (openSet.Count > 0)
        {

            Node currentNode = openSet.RemoveFirst();
            
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }
            //foreach is slightly slower than for, which is slightly slower than for with a cached local var
            Node[] neighbours = grid.GetNeighBours(currentNode);//could even just read these without allocation
            int n = grid.neighboursIndex-1;
            for (;n >= 0; n--)
            {
                //if (neighbours[n] == null) { return; }can be easily optimised out now (without passing around references)
                Node neighbor = neighbours[n];
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }

        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        //we don't need a local variable here
        Path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            Path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Path.Reverse();
        //grid.path = path; unused
    }

    int GetDistance(Node NodeA, Node NodeB)
    {
        int dstX = Mathf.Abs(NodeA.gridX - NodeB.gridX);
        int dstY = Mathf.Abs(NodeA.gridY - NodeB.gridY);
        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        else
        {
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }
}
