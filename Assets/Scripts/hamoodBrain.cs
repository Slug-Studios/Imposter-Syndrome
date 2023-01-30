using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hamoodBrain : MonoBehaviour
{
    private bool isKill;
    public Animator animator;
    public RuntimeAnimatorController Walk; 
    public RuntimeAnimatorController Scream;
    public AudioSource mainSound;
    public AudioClip mainClip;
    public AudioClip screamClip;
    public float speed;
    public float startSpeed;
    public float endSpeed;
    public float timeToAcc;
    private float AccSpeed;
    public Transform Player;
    private Rigidbody HRigidbody;
    // Start is called before the first frame update
    void Start()
    {
        HRigidbody = GetComponent<Rigidbody>();
        mainSound.volume = mainMenuCtrl.entityV;
        mainSound.clip = mainClip;
        mainSound.Play();
        AccSpeed = (endSpeed-startSpeed)/timeToAcc;
        speed = startSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isKill)//make him clap, rotate at the player, and move
        {
            animator.runtimeAnimatorController = Walk;
            transform.rotation = Quaternion.Euler(0,Mathf.Atan2(transform.position.x - Player.position.x, transform.position.z - Player.position.z) * Mathf.Rad2Deg + 180,0);
            HRigidbody.AddRelativeForce(Vector3.forward * speed * Time.deltaTime);
            if (speed < endSpeed)
            {
                speed += AccSpeed * Time.deltaTime;
            }
            //detect how far away the player is, if the player is too close, start kill animation thing
            float distance = (Mathf.Sqrt(Mathf.Pow(transform.position.x - Player.position.x, 2) + Mathf.Pow(transform.position.z - Player.position.z, 2)));
            if (distance <= 1)
            {
                isKill = true;
                StartCoroutine(killAnimation());
            }
        }
        IEnumerator killAnimation()
        {
            HRigidbody.velocity = Vector3.zero;
            HRigidbody.angularVelocity = Vector3.zero;
            animator.runtimeAnimatorController = Scream;
            mainSound.clip = screamClip;
            mainSound.loop = false; 
            mainSound.Play();
            Player.GetComponent<playerController>().DeathAnim(transform, new Vector3(0, 2.5f, 1), Quaternion.Euler(-20, 180, 0));
            yield return new WaitForSeconds(2);
            Player.GetComponent<playerController>().Death(5);
        }
    }
}
