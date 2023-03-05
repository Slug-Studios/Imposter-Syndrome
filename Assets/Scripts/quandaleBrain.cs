using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class quandaleBrain : MonoBehaviour
{
    Pathfinder pathfinder;
    private Animator QuandaleAnimator;
    public RuntimeAnimatorController Walk;
    public RuntimeAnimatorController Scream;
    private Rigidbody QuandaleRigidbody;
    public int seekPhase;
    private float speed;
    private float time;
    private float time1;
    private float time2;
    public AudioSource KillSound;
    public LayerMask playerMask;
    public float walkSpeed;
    public float runSpeed;
    public AudioSource QuandaleSound;
    public List<AudioClip> QuandaleLines;
    public quandaleBrain Quandale1;
    public quandaleBrain Quandale2;
    public Vector3 KillOffset;
    public int type;
    public LayerMask Unwalkable;
    private Grid grid;

    // Start is called before the first frame update
    void Start()
    {
        QuandaleRigidbody = GetComponent<Rigidbody>();
        pathfinder = GetComponent<Pathfinder>();
        grid = GetComponent<Grid>();
        QuandaleAnimator = GetComponent<Animator>();
        QuandaleSound.volume = mainMenuCtrl.entityV;
        KillSound.volume = mainMenuCtrl.entityV;
        QuandaleAnimator.runtimeAnimatorController = null;

        // set volumes
        KillSound.volume = mainMenuCtrl.entityV;

    }

    // Update is called once per frame
    void Update()
    {
        // if the player dies, stop playing sound
        if (pathfinder.player != null)
        {
            if (pathfinder.player.GetComponent<playerController>().dead)
            {
                QuandaleSound.volume = 0;
                KillSound.volume = 0;
            }
        }

        //play animations depending on velocity
        if (seekPhase != 0)
        {
            if (Mathf.Sqrt(Mathf.Pow(QuandaleRigidbody.velocity.x, 2) + Mathf.Pow(QuandaleRigidbody.velocity.z, 2)) >= 0.1f && Walk != null)
            {
                QuandaleAnimator.runtimeAnimatorController = Walk;
            }
            else
            {
                QuandaleAnimator.runtimeAnimatorController = null;
            }
            time2 += Time.deltaTime;
        }
        //AI, taken from imposter and modified
        switch (seekPhase)
        {
            case 1: //1, chase
                if (QuandaleSound.isPlaying == false)
                {
                    QuandaleSound.clip = QuandaleLines[Random.Range(0,QuandaleLines.Count-1)];
                    QuandaleSound.Play();
                }
                speed = runSpeed;
                //if far away from target, speed up
                if (pathfinder.target != null)
                {
                    if (Mathf.Sqrt(Mathf.Pow(transform.position.x - pathfinder.target.position.x, 2) + Mathf.Pow(transform.position.z - pathfinder.target.position.z, 2)) >= 50)
                    {
                        speed = speed * 2;
                    }
                }
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
                if (pathfinder.target == null) //if it looses track of the player, spin around for a bit, if nothing still; swap to wandering
                {
                    seekPhase = 3;
                }
                //if velocity is close to 0 for 2 seconds, make target null
                if (pathfinder.target != null)
                {
                    if (QuandaleRigidbody.velocity == Vector3.zero || Mathf.Abs(QuandaleRigidbody.velocity.x) <= 0.1f && Mathf.Abs(QuandaleRigidbody.velocity.z) <= 0.1f)
                    {
                        time1 = time1 + Time.deltaTime;
                        if (time1 >= 2)
                        {
                            Destroy(pathfinder.target.gameObject);
                        }
                    }
                }
                else
                {
                    time1 = 0;
                }
                //notify the other quandales
                if (Quandale1 != null && pathfinder.target != null && pathfinder != null)
                {
                    Quandale1.pathfinder.target = pathfinder.target;
                    Quandale1.seekPhase = 1;
                }
                if (Quandale2 != null && pathfinder.target != null && Quandale2.pathfinder.target)
                {
                    Quandale2.pathfinder.target = pathfinder.target;
                    Quandale2.seekPhase = 1;
                }
                if (Physics.CheckBox(transform.position, new Vector3(2.5f, 10, 2.5f), transform.rotation, playerMask))
                {
                    Debug.Log("GOT YOU(QUANDALE)");
                    seekPhase = 0;
                    StartCoroutine(killAnimation());
                } else if (pathfinder.Path != null && pathfinder.target != null && pathfinder.Path[0].offsetFromMainParent != null && Mathf.Sqrt(Mathf.Pow(QuandaleRigidbody.velocity.x, 2) + Mathf.Pow(QuandaleRigidbody.velocity.z, 2)) <= speed)
                {
                    float targetAngle = Mathf.Atan2(pathfinder.Path[0].offsetFromMainParent.x, pathfinder.Path[0].offsetFromMainParent.z) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, targetAngle + 180, 0);
                    QuandaleRigidbody.AddRelativeForce(Vector3.forward * speed * Time.deltaTime);
                }
                break;
            case 2: //2, wandering
                if (QuandaleSound.isPlaying == true)
                {
                    QuandaleSound.Stop();
                }
                speed = walkSpeed;
                if (Look(80)) //if it sees the player, run the SeesPlayer function
                {
                    StartCoroutine("SeesPlayer");
                }
                if (pathfinder.target != null)
                {
                    pathfinder.FindPath(transform.position, pathfinder.target.position);
                }
                if (pathfinder.Path != null && pathfinder.target != null && pathfinder.Path[0].offsetFromMainParent != null)
                {
                    float targetAngle = Mathf.Atan2(pathfinder.Path[0].offsetFromMainParent.x, pathfinder.Path[0].offsetFromMainParent.z) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, targetAngle + 180, 0);
                    QuandaleRigidbody.AddRelativeForce(Vector3.forward * speed * Time.deltaTime);
                }
                if (time >= 0 || pathfinder.target == null) // every 10 seconds or if the target is null, find a new path
                {
                    time = -10;
                    pathfinder.target = new GameObject("target").transform;
                    pathfinder.target.position = transform.position + new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));
                }
                time = time + Time.deltaTime;
                //if velocity is 0 for 1 second, make target null
                if (QuandaleRigidbody.velocity == Vector3.zero || Mathf.Abs(QuandaleRigidbody.velocity.x) <= 0.1f && Mathf.Abs(QuandaleRigidbody.velocity.z) <= 0.1f)
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
                transform.Rotate(Vector3.up, 90 * Time.deltaTime);
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
        if (Physics.Raycast(transform.position, pathfinder.player.position - transform.position, out hit, 100f))
        {
            if (hit.collider.GetComponent<playerController>() != null)
            {
                float angle = Mathf.Atan2(transform.position.x - pathfinder.player.position.x, transform.position.z - pathfinder.player.position.z) * Mathf.Rad2Deg - transform.rotation.eulerAngles.y + 180;

                //correct the angle
                if (angle > 180)
                {
                    angle = angle - 360;
                }
                if (angle < -180)
                {
                    angle = angle + 360;
                }
                //Debug.Log(angle);
                if (angle >= -Angle && angle <= Angle)
                {
                    return true;
                }

            }

        }
        return false;
    }

    public void HearPlayer(float playerNoise)
    {
        float distance = (Mathf.Abs(Mathf.Sqrt(Mathf.Pow(pathfinder.player.position.x - transform.position.x, 2) + Mathf.Pow(pathfinder.player.position.z - transform.position.z, 2))));
        float hearchance = Mathf.Clamp01(playerNoise / distance - 0.01f) * 100f;

        if (hearchance <= (float)Random.Range(100, 0))
        {
            float RandX = Random.Range(0, Mathf.Clamp(distance - 10, 0, 100));
            float RandY = Random.Range(0, Mathf.Clamp(distance - 10, 0, 100));
            Vector3 potentialTarget = pathfinder.player.position + new Vector3(RandX - RandX / 2, 0, RandY - RandY / 2);
            if (pathfinder.target == null)
            {
                pathfinder.target = new GameObject().transform;
                pathfinder.target.position = potentialTarget;
            }
            else
            {
                pathfinder.target.position = potentialTarget;
            }
        }
    }

    IEnumerator SeesPlayer()
    {
        QuandaleSound.clip = QuandaleLines[Random.Range(0,QuandaleLines.Count-1)];
        QuandaleSound.Play();
        seekPhase = 0;
        transform.rotation = Quaternion.Euler(0, Mathf.Atan2(transform.position.x - pathfinder.player.position.x, transform.position.z - pathfinder.player.position.z) * Mathf.Rad2Deg + 180, 0);
        yield return new WaitForSeconds(1);
        seekPhase = 1;
        time = 0;
    }

    IEnumerator killAnimation()
    {
        QuandaleSound.Stop();
        seekPhase = 0;
        Quandale1.seekPhase = 0;
        Quandale2.seekPhase = 0;
        QuandaleRigidbody.velocity = Vector3.zero;
        QuandaleRigidbody.angularVelocity = Vector3.zero;
        QuandaleAnimator.runtimeAnimatorController = Scream;
        KillSound.Play();
        pathfinder.player.GetComponent<playerController>().DeathAnim(transform, KillOffset, Quaternion.Euler(0, 180, 0));
        yield return new WaitForSeconds(2);
        pathfinder.player.GetComponent<playerController>().Death(1+type);
    }
}
