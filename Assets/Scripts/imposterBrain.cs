using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class imposterBrain : MonoBehaviour
{
    Grid grid;
    public Transform player;
    private Transform target;
    private List<Node> Path;
    private float speed;
    public float wanderSpeed;
    public float chaseSpeed;
    public int seekPhase;
    public AudioSource MainMusic;
    public AudioSource KillSound;
    public LayerMask playerMask;
    private float time;

    void Awake()
    {
        grid = GetComponent<Grid>();
    }
    private void Start()
    {
        MainMusic.volume = mainMenuCtrl.entityV;

    }
    // Update is called once per frame
    void Update()
    {
        Look(0);

        //different states that his movement can be in
        switch (seekPhase)
        {
            case 1: //1, chase

                speed = chaseSpeed;
                if (Look(80)) //if it sees the player it updates his position
                {
                    target = new GameObject("target").transform;
                    target = player;
                }
                if (target != null)
                {
                    //Run Pathfinding
                    FindPath(transform.position, target.position);
                }
                if (Physics.CheckBox(transform.position, new Vector3(1,10,1), transform.rotation, playerMask))
                {
                    Debug.Log("FOUND YOU(IMP)");
                    seekPhase = 0;
                    StartCoroutine("killAnimation");
                } else if (Path != null && target != null && Path[0].offsetFromMainParent != null)
                {
                    transform.rotation = Quaternion.Euler(0, Mathf.Atan2(Path[0].offsetFromMainParent.x, Path[0].offsetFromMainParent.z) * Mathf.Rad2Deg + 180, 0);
                    transform.Translate(Vector3.forward * speed * Time.deltaTime, transform);
                    
                }
                if (target == null) //if it looses track of the player, spin around for a bit, if nothing still; swap to wandering
                {
                    seekPhase = 3;
                }
                break;
            case 2: //2, wandering
                speed = wanderSpeed;
                if (Look(80)) //if it sees the player, run the SeesPlayer function
                {
                    StartCoroutine("SeesPlayer");
                }
                if (target != null)
                {
                    FindPath(transform.position, target.position);
                }
                if (Path != null && target != null && Path[0].offsetFromMainParent != null)
                {
                    transform.rotation = Quaternion.Euler(0, Mathf.Atan2(Path[0].offsetFromMainParent.x, Path[0].offsetFromMainParent.z) * Mathf.Rad2Deg + 180, 0);
                    transform.Translate(Vector3.forward * speed * Time.deltaTime, transform);
                }
                if (time >= 0 || target == null) // every 10 seconds or if the target is null, find a new path
                {
                    time = -10;
                    target = new GameObject("target").transform;
                    target.position = transform.position + new Vector3(Random.Range(-50, 50), 0, Random.Range(-50,50));
                }
                time = time +Time.deltaTime;
                break;
            case 3: //3, search(for when it looses track of the player)
                time = time + Time.deltaTime;
                transform.rotation = Quaternion.Euler(0, transform.rotation.y + 90 * Time.deltaTime, 0);
                if (time >= 4.5)
                {
                    seekPhase = 2;
                }
                if (Look(80))
                {
                    seekPhase = 1;
                }
                break;
        }

        

    }

    bool Look(float Angle)
    {
        //Raycast at player, if can detect the player within a certain angle, output true, otherwise output false
        RaycastHit hit;
        if (Physics.Raycast(transform.position, player.position - transform.position, out hit, 100f))
        {
            if (hit.collider.GetComponent<playerController>() != null)
            {
                float angle = Mathf.Atan2(transform.position.x - player.position.x, transform.position.z - player.position.z) * Mathf.Rad2Deg - transform.rotation.eulerAngles.y + 180;

                //correct the angle
                if (angle > 180)
                {
                    angle = angle-360;
                }
                Debug.Log(angle);
                //if (angle >= -Angle && angle <= Angle)
                {
                    return true;
                }

            }

        }
        return true;
    }
    IEnumerator SeesPlayer()
    {
        seekPhase = 0;
        transform.rotation = Quaternion.Euler(0, Mathf.Atan2(player.transform.position.x, player.position.z) * Mathf.Rad2Deg + 90, 0);
        yield return new WaitForSeconds(1);
        seekPhase = 1;
        time = 0;
    }

    IEnumerator killAnimation()
    {
        player.GetComponent<playerController>().DeathAnim(transform);
        MainMusic.volume = mainMenuCtrl.entityV/4;
        yield return new WaitForSeconds(1);
        KillSound.Play();
        MainMusic.Stop();
        player.GetComponent<playerController>().Death(0);
    }

    //funky pathfinding, most of this ain't mine lmfao
    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode == targetNode)
        {
            Destroy(target.gameObject);
            return;
        }
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);
        while(openSet.Count > 0)
        {

            Node currentNode = openSet[0];
            for (int i = 0; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbor in grid.GetNeighBours(currentNode))
            {
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
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        Path = path;
        grid.path = path;
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

