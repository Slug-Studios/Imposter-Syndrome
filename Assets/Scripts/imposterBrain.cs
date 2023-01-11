using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class imposterBrain : MonoBehaviour
{
    Grid grid;
    Pathfinder pathfinder;
    private float speed;
    public float wanderSpeed;
    public float chaseSpeed;
    public int seekPhase;
    public AudioSource MainMusic;
    public AudioSource KillSound;
    public LayerMask playerMask;
    private float time;
    private float time1;
    private Rigidbody ImpRigigbody;

    void Awake()
    {
        grid = GetComponent<Grid>();
        pathfinder = GetComponent<Pathfinder>();
    }
    private void Start()
    {
        MainMusic.volume = mainMenuCtrl.entityV;
        ImpRigigbody = gameObject.GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {

        //different states that his movement can be in
        switch (seekPhase)
        {
            case 1: //1, chase

                speed = chaseSpeed;
                if (Look(80)) //if it sees the player it updates his position
                {
                    if (pathfinder.target == null)
                    {
                        pathfinder.target = new GameObject("target").transform;
                    }
                    pathfinder.target.position = pathfinder.player.position;
                }
                if (pathfinder.target != null)
                {
                    //Run Pathfinding
                    pathfinder.FindPath(transform.position, pathfinder.target.position);
                }
                if (Physics.CheckBox(transform.position, new Vector3(1.5f,10,1.5f), transform.rotation, playerMask))
                {
                    Debug.Log("GOT YOU(IMP)");
                    seekPhase = 0;
                    StartCoroutine("killAnimation");
                } else if (pathfinder.Path != null && pathfinder.target != null && pathfinder.Path[0].offsetFromMainParent != null && Mathf.Sqrt(Mathf.Pow(ImpRigigbody.velocity.x, 2) + Mathf.Pow(ImpRigigbody.velocity.z, 2)) <= speed)
                {
                    transform.rotation = Quaternion.Euler(0, Mathf.Atan2(pathfinder.Path[0].offsetFromMainParent.x, pathfinder.Path[0].offsetFromMainParent.z) * Mathf.Rad2Deg + 180, 0);
                    ImpRigigbody.AddRelativeForce(Vector3.forward * speed * Time.deltaTime);
                    
                }
                if (pathfinder.target == null) //if it looses track of the player, spin around for a bit, if nothing still; swap to wandering
                {
                    seekPhase = 3;
                }
                //if velocity is close to 0 for 2 seconds, make target null
                if (ImpRigigbody.velocity == Vector3.zero || Mathf.Abs(ImpRigigbody.velocity.x) <=0.1f && Mathf.Abs(ImpRigigbody.velocity.z) <= 0.1f)
                {
                    time1 = time1 + Time.deltaTime;
                    if (time1 >= 2)
                    {
                        Destroy(pathfinder.target.gameObject);
                    }
                }
                else
                {
                    time1 = 0;
                }
                break;
            case 2: //2, wandering
                speed = wanderSpeed;
                if (Look(80)) //if it sees the player, run the SeesPlayer function
                {
                    StartCoroutine("SeesPlayer");
                }
                if (pathfinder.target != null)
                {
                    pathfinder.FindPath(transform.position, pathfinder.target.position);
                }
                if (pathfinder.Path != null && pathfinder.target != null && pathfinder.Path[0].offsetFromMainParent != null && Mathf.Sqrt(Mathf.Pow(ImpRigigbody.velocity.x, 2) + Mathf.Pow(ImpRigigbody.velocity.z, 2)) <= speed)
                {
                    transform.rotation = Quaternion.Euler(0, Mathf.Atan2(pathfinder.Path[0].offsetFromMainParent.x, pathfinder.Path[0].offsetFromMainParent.z) * Mathf.Rad2Deg + 180, 0);
                    ImpRigigbody.AddRelativeForce(Vector3.forward * speed * Time.deltaTime);
                }
                if (time >= 0 || pathfinder.target == null) // every 10 seconds or if the target is null, find a new path
                {
                    time = -10;
                    pathfinder.target = new GameObject("target").transform;
                    pathfinder.target.position = transform.position + new Vector3(Random.Range(-50, 50), 0, Random.Range(-50,50));
                }
                time = time +Time.deltaTime;
                //if velocity is 0 for 1 second, make target null
                if (ImpRigigbody.velocity == Vector3.zero || Mathf.Abs(ImpRigigbody.velocity.x) <= 0.1f && Mathf.Abs(ImpRigigbody.velocity.z) <= 0.1f)
                {
                    time1 = time1 + Time.deltaTime;
                    if (time1 >= 1)
                    {
                        Destroy(pathfinder.target.gameObject);
                    }
                }
                else
                {
                    time1 = 0;
                }
                
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
    public void HearPlayer(float playerNoise)
    {
        float distance = (Mathf.Abs(Mathf.Sqrt(Mathf.Pow(pathfinder.player.position.x - transform.position.x, 2) + Mathf.Pow(pathfinder.player.position.z - transform.position.z, 2))));
        float hearchance = Mathf.Clamp01(playerNoise / distance -0.01f)*100f;

        if (hearchance <= (float)Random.Range(100, 0))
        {
            float RandX = Random.Range(0, Mathf.Clamp(distance - 10, 0, 100));
            float RandY = Random.Range(0, Mathf.Clamp(distance - 10, 0, 100));
            Vector3 potentialTarget = pathfinder.player.position + new Vector3(RandX - RandX/2,0, RandY - RandY / 2);
            if (pathfinder.target == null)
            {
                pathfinder.target = new GameObject().transform;
                pathfinder.target.position = potentialTarget;
            } else
            {
                pathfinder.target.position = potentialTarget;
            }
        }
    }

    bool Look(float Angle)
    {
        //Raycast at player, if can detect the player within a certain angle, output true, otherwise output false
        RaycastHit hit;
        if (Physics.Raycast(transform.position, pathfinder.player.position - transform.position, out hit, 100f))
        {
            if (hit.collider.GetComponent<playerController>() != null)
            {
                float angle = Mathf.Atan2(transform.position.x - pathfinder.player.position.x, transform.position.z - pathfinder.player.position.z) * Mathf.Rad2Deg - transform.rotation.eulerAngles.y + 180;

                //correct the angle
                if (angle > 180)
                {
                    angle = angle-360;
                }
                if (angle >= -Angle && angle <= Angle)
                {
                    return true;
                }

            }

        }
        return false;
    }
    IEnumerator SeesPlayer()
    {
        seekPhase = 0;
        transform.rotation = Quaternion.Euler(0, Mathf.Atan2(pathfinder.player.transform.position.x, pathfinder.player.position.z) * Mathf.Rad2Deg + 90, 0);
        yield return new WaitForSeconds(1);
        seekPhase = 1;
        time = 0;
    }

    IEnumerator killAnimation()
    {
        ImpRigigbody.velocity = Vector3.zero;
        ImpRigigbody.angularVelocity = Vector3.zero;
        pathfinder.player.GetComponent<playerController>().DeathAnim(transform, Vector3.forward * 2, Quaternion.Euler(0,0,0));
        MainMusic.volume = mainMenuCtrl.entityV/4;
        yield return new WaitForSeconds(1);
        KillSound.Play();
        MainMusic.Stop();
        pathfinder.player.GetComponent<playerController>().Death(0);
    }
}

