using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class playerController : MonoBehaviour
{
    private float speed = 20;
    public float rotSpeed = 100;
    private float Location = RoomSpawner.maxSize / 2;
    private Rigidbody Physics;
    public bool isProne;
    public Camera Camera;
    public GameObject Player;
    private bool menuUp = false;
    public Canvas menu;
    private bool canMove;
    public Canvas deathScreen;
    public Image ImpFoundYou;
    public GameObject DeathUI;
    private bool dead;
    private int killer;
    private float time;
    public AudioSource Ambience;
    public AudioSource footStep;
    private float stepDelay;
    public imposterBrain Imposter;

    // Start is called before the first frame update
    void Start()
    {
        
        Physics = GetComponent<Rigidbody>();
        canMove = true;
        menuUp = false;
    }

    // Update is called once per frame
    void Update()
    {

        //Set volume from values
        Ambience.volume = mainMenuCtrl.ambientV;

        
        if (canMove)
        {
            //Toggle menuUp if the esc key is pressed
            if (Input.GetKeyDown("escape"))
            {
                menuUp = !menuUp;
            }
            //if the menu is not up, allow movement and hide menu, if it is, show the menu and dissable movment
            if (!menuUp)
            {
                menu.enabled = false;
                //Simple Movement Script using force
                if (isProne)
                {
                    if (Mathf.Sqrt(Mathf.Pow(Physics.velocity.x, 2) + Mathf.Pow(Physics.velocity.z, 2)) >= 10f)
                    {
                        Physics.mass = 0.1f;
                        Physics.drag = 1;
                    } else
                    {
                        Physics.mass = 0.01f;
                        Physics.drag = 10;
                    }
                    Physics.AddRelativeForce(new Vector3(speed * Time.deltaTime * Input.GetAxis("Horizontal") * .25f, 0, speed * Time.deltaTime * .25f * Input.GetAxis("Vertical")));
                }
                else
                {
                    Physics.mass = 0.01f;
                    Physics.drag = 10;
                    Physics.AddRelativeForce(new Vector3(speed * Time.deltaTime * Input.GetAxis("Horizontal") * (Input.GetAxis("Sprint") + 1.5f), 0, speed * Time.deltaTime * Input.GetAxis("Vertical") * (Input.GetAxis("Sprint") + 2f)));
                }
                transform.Rotate(Vector3.up, rotSpeed * Time.deltaTime * Input.GetAxis("Mouse X"));

                if (Input.GetKeyDown("z"))
                {
                    isProne = !isProne;
                }
                // if you are prone, lower * rotate the player model
                if (isProne)
                {
                    Player.transform.rotation = Quaternion.Euler(90, transform.rotation.y, transform.rotation.z);
                    transform.position = new Vector3(transform.position.x, (float)0.5, transform.position.z);
                }
                else
                {
                    transform.position = new Vector3(transform.position.x, (float)3, transform.position.z);
                    Player.transform.rotation = Quaternion.Euler(0, transform.rotation.y, transform.rotation.z);
                }
            }
            else
            {
                menu.enabled = true;
            }
            float velocity = Mathf.Sqrt(Mathf.Pow(Physics.velocity.x, 2) + Mathf.Pow(Physics.velocity.z, 2));
            if (velocity > 0) //play footsteps dependant on speed, also notify imposter about them
            {
                velocity = Mathf.Clamp(velocity, 2, 10)/2;
                stepDelay = stepDelay + Time.deltaTime;
                if (stepDelay >= 2 / velocity)
                {
                    footStep.volume = velocity/5;
                    stepDelay = 0;
                    if (!isProne)
                    {
                        footStep.Play();
                    }
                    Imposter.HearPlayer(footStep.volume);
                }
            }
        }
        //if dead, fad in the death screen
        if (dead)
        {
            switch (killer)
            {
                case 0: //case 0, imposter
                    ImpFoundYou.color = new Color(255, 255, 255, time-1.5f);
                    break;
            }
            time = time + Time.deltaTime/2;
            if (time >= 3.5f)
            {
                DeathUI.SetActive(true);
                switch (killer)
                {
                    case 0:
                        DeathUI.GetComponent<Text>().text = "Ejected";
                        break;
                }
            }
        }
    }
    public void DeathAnim(Transform Killer)
    {
        menuUp = false;
        canMove = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(transform.rotation.x, Mathf.Atan2(transform.position.x - Killer.position.x, transform.position.z - Killer.position.z) * Mathf.Rad2Deg + 180, transform.rotation.z);
        transform.Translate(Vector3.back * 2, transform);
        
    }
    public void Death(int Killer)
    {
        deathScreen.enabled = true;
        dead = true;
        killer = Killer;
    }
    //Menu Button stuff
    public void quitMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
    public void Reload()
    {
        SceneManager.LoadScene("Level 0");
    }
}
