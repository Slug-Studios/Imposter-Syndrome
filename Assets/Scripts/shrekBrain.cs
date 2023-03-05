using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shrekBrain : MonoBehaviour
{
    Pathfinder pathfinder;
    private Animator ShrekAnimator;
    public RuntimeAnimatorController Walk;
    public RuntimeAnimatorController Run;
    public RuntimeAnimatorController Scream;
    private Rigidbody ShrekRigidbody;
    public int seekPhase;
    private float speed;
    private float time;
    private float time1;
    private float time2;
    public AudioSource KillSound;
    public LayerMask playerMask;
    public float walkSpeed;
    public float runSpeed;
    public Transform armature;
    public AudioSource Footstep;
    public AudioSource WhatDoingSwamp;
    private CapsuleCollider Collider;
    
    // Start is called before the first frame update
    void Start()
    {
        ShrekRigidbody = GetComponent<Rigidbody>();
        pathfinder = GetComponent<Pathfinder>();
        ShrekAnimator = GetComponent<Animator>();
        ShrekAnimator.runtimeAnimatorController = null;
        Collider = GetComponent<CapsuleCollider>();

        // set volumes
        WhatDoingSwamp.volume = mainMenuCtrl.entityV;
        Footstep.volume = mainMenuCtrl.entityV;
        KillSound.volume = mainMenuCtrl.entityV;
    }

    // Update is called once per frame
    void Update()
    {
        //disable collider if too far away
        if (Vector3.Distance(pathfinder.player.position, transform.position) <= 100f)
        {
            Collider.enabled = true;
        }
        else
        {
            Collider.enabled = false;
        }
        // if the player dies, stop playing sound
        if (pathfinder.player != null)
        {
            if (pathfinder.player.GetComponent<playerController>().dead)
            {
                Footstep.volume = 0;
                KillSound.volume = 0;
                WhatDoingSwamp.volume = 0;
            }
        }
         new Vector2();
        //play animations depending on velocity
        if (seekPhase != 0)
        {
            if (Mathf.Sqrt(Mathf.Pow(ShrekRigidbody.velocity.x, 2) + Mathf.Pow(ShrekRigidbody.velocity.z, 2)) >= runSpeed/10 - 6)
            {
                ShrekAnimator.runtimeAnimatorController = Run;
                if (time2 >= 0.32)
                {
                    Footstep.Play();
                    time2 = 0;
                }
            }
            else if (Mathf.Sqrt(Mathf.Pow(ShrekRigidbody.velocity.x, 2) + Mathf.Pow(ShrekRigidbody.velocity.z, 2)) >= walkSpeed/10 - 1)
            {
                ShrekAnimator.runtimeAnimatorController = Walk;
                if (time2 >= 0.66)
                {
                    Footstep.Play();
                    time2 = 0;
                }
            } else
            {
                ShrekAnimator.runtimeAnimatorController = null;
            }
            time2 += Time.deltaTime;
        }
        //AI, taken from imposter and modified
        switch (seekPhase)
        {
            case 1: //1, chase

                speed = runSpeed;
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
                if (Physics.CheckBox(transform.position, new Vector3(2.5f, 10, 2.5f), transform.rotation, playerMask))
                {
                    Debug.Log("GOT YOU(SHREK)");
                    seekPhase = 0;
                    StartCoroutine(killAnimation());
                }
                else if (pathfinder.Path != null && pathfinder.target != null && pathfinder.Path[0].offsetFromMainParent != null && Mathf.Sqrt(Mathf.Pow(ShrekRigidbody.velocity.x, 2) + Mathf.Pow(ShrekRigidbody.velocity.z, 2)) <= speed)
                {
                    float targetAngle = Mathf.Atan2(pathfinder.Path[0].offsetFromMainParent.x, pathfinder.Path[0].offsetFromMainParent.z) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, targetAngle + 180, 0);
                    ShrekRigidbody.AddRelativeForce(Vector3.forward * speed * Time.deltaTime);
                }
                if (pathfinder.target == null) //if it looses track of the player, spin around for a bit, if nothing still; swap to wandering
                {
                    seekPhase = 3;
                }
                //if velocity is close to 0 for 2 seconds, make target null
                if (ShrekRigidbody.velocity == Vector3.zero || Mathf.Abs(ShrekRigidbody.velocity.x) <= 0.1f && Mathf.Abs(ShrekRigidbody.velocity.z) <= 0.1f)
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
                    ShrekRigidbody.AddRelativeForce(Vector3.forward * speed * Time.deltaTime);
                }
                if (time >= 0 || pathfinder.target == null) // every 10 seconds or if the target is null, find a new path
                {
                    time = -10;
                    pathfinder.target = new GameObject("target").transform;
                    pathfinder.target.position = transform.position + new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));
                }
                time = time + Time.deltaTime;
                //if velocity is 0 for 1 second, make target null
                if (ShrekRigidbody.velocity == Vector3.zero || Mathf.Abs(ShrekRigidbody.velocity.x) <= 0.1f && Mathf.Abs(ShrekRigidbody.velocity.z) <= 0.1f)
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
        WhatDoingSwamp.Play();
        seekPhase = 0;
        transform.rotation = Quaternion.Euler(0, Mathf.Atan2(transform.position.x - pathfinder.player.position.x, transform.position.z - pathfinder.player.position.z) * Mathf.Rad2Deg + 180, 0);
        yield return new WaitForSeconds(2.5f);
        seekPhase = 1;
        time = 0;
    }

    IEnumerator killAnimation()
    {
        seekPhase = 0;
        ShrekRigidbody.velocity = Vector3.zero;
        ShrekRigidbody.angularVelocity = Vector3.zero;
        ShrekAnimator.runtimeAnimatorController = Scream;
        KillSound.Play();
        pathfinder.player.GetComponent<playerController>().DeathAnim(transform, new Vector3(0,5,4), Quaternion.Euler(-14,180,0));
        yield return new WaitForSeconds(2);
        pathfinder.player.GetComponent<playerController>().Death(1);
    }
}
