using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class walterBrain : MonoBehaviour
{
    private Animator WaltAnimator;
    public RuntimeAnimatorController Walk;
    public RuntimeAnimatorController Idle;
    public bool Walking;
    private AudioSource Speaker;
    public AudioClip Main;
    public AudioClip Spotted;
    public AudioClip sayName;
    public Transform player;
    private float speed = 5f;
    private bool metPlayer;
    private int playerMeetPhase;
    public List<GameObject> Items;
    private bool lookPlayer = true;
    public bool Itempicked;


    // Start is called before the first frame update
    void Start()
    {
        WaltAnimator = gameObject.AddComponent<Animator>();
        Speaker = GetComponent<AudioSource>();
        Speaker.volume = mainMenuCtrl.entityV;
        Speaker.clip = Main;
        Speaker.loop = true;
        Speaker.Play();
    }

    // Update is called once per frame
    void Update()
    {
        //Play animations if walking, when walking move forwards
        if (!Walking)
        {
            WaltAnimator.runtimeAnimatorController = Idle;
        } else
        {
            WaltAnimator.runtimeAnimatorController = Walk;
            transform.Translate(-Vector3.forward * Time.deltaTime * speed);
        }
        //rotate at player
        if (lookPlayer)
        {
            transform.rotation = Quaternion.Euler(0, Mathf.Atan2(transform.position.x - player.position.x, transform.position.z - player.position.z) * Mathf.Rad2Deg, 0);
        }
        //try to raycast at player if the player is within 50 units
        if (Vector3.Distance(transform.position, player.position) <= 50 && !metPlayer)
        {
            if (Look())
            {
                metPlayer = true;
                playerMeetPhase = 1;
                Walking = true;
                Speaker.clip = Spotted;
                Speaker.Play();
            }
        }
        switch (playerMeetPhase)
        {
            case 0:
                break;
            case 1: //Walk towards player, untill the distance is very close
                if (Vector3.Distance(transform.position, player.position) <= 5)
                {
                    playerMeetPhase = 0;
                    Walking = false;
                    Speaker.loop = false;
                    Speaker.clip = sayName;
                    Speaker.Play();
                    StartCoroutine(Wait());
                }
                break;
            case 2: //Offer items
                if (Itempicked)
                {
                    playerMeetPhase = 3;
                    Speaker.clip = Spotted;
                    Speaker.Play();
                }
                break;
            case 3: // ACSEND
                transform.Translate(Vector3.up * speed * Time.deltaTime);
                break;
        }
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(7);
        playerMeetPhase = 2;
        lookPlayer = false;
        Itempicked = false;
        GameObject Item1 = Items[Mathf.RoundToInt(Random.Range(-0.49f,3.49f))];
        GameObject Item2 = Items[Mathf.RoundToInt(Random.Range(-0.49f, 3.49f))];
        Item1 = Instantiate(Item1, transform.position, transform.rotation, transform);
        Item2 = Instantiate(Item2, transform.position, transform.rotation, transform);
        Item1.transform.localPosition = new Vector3(2, 2.66f, -1);
        Item2.transform.localPosition = new Vector3(-2, 2.66f, -1);
        Item1.transform.localRotation = Quaternion.Euler(0,180,0);
        Item2.transform.localRotation = Quaternion.Euler(0,180,0);
        Item1.GetComponent<itemScript>().Spawner = GetComponent<walterBrain>();
        Item2.GetComponent<itemScript>().Spawner = GetComponent<walterBrain>();
    }
    bool Look()
    {
        //Raycast at player, if can detect the player, output true, otherwise output false
        RaycastHit hit;
        if (Physics.Raycast(transform.position, player.position - transform.position, out hit, 50f))
        {
            if (hit.collider.GetComponent<playerController>() != null)
            {
                return true;
            }
        }
        return false;
    }
}
