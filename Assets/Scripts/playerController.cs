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
    public Image ShrekFoundYou;
    public Image QuandaleFoundYouA;
    public Image QuandaleFoundYouB;
    public Image QuandaleFoundYouC;
    public Image HamoodFoundYou;
    public GameObject DeathUI;
    public Text DeathText;
    public bool dead;
    private int killer;
    private float time;
    public AudioSource Ambience;
    public AudioSource footStep;
    private float stepDelay;
    public imposterBrain Imposter;
    public shrekBrain Shrek;
    public quandaleBrain QuandaleA;
    public quandaleBrain QuandaleB;
    public quandaleBrain QuandaleC;
    public Slider staminaBar;
    public float stamina;
    private float sprintKey;
    public Canvas UICanvas;
    public int upgradeEquiped;
    private float speedBoost = 1;
    private float BoostTime;
    public Slider itemRemainingBar;
    public GameObject itemRemainBarActive;
    private Collider Pcollider;
    public GameObject equipedItem;


    // Start is called before the first frame update
    void Start()
    {
        Pcollider = GetComponent<Collider>();
        Physics = GetComponent<Rigidbody>();
        canMove = true;
        menuUp = false;
        //Set volume from values
        Ambience.volume = mainMenuCtrl.ambientV;
    }

    // Update is called once per frame
    void Update()
    {
        if (menuUp || dead)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        } else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (canMove)
        {
            //Upgrades, in a switch statement
            switch (upgradeEquiped)
            {
                case 0:
                    break;
                case 1: //Drip
                    stamina = 20;
                    speedBoost = 1.2f;
                    break;
                case 2: //"Milk"
                    if (Input.GetKey(KeyCode.Mouse1))
                    {
                        Pcollider.enabled = false;
                        speedBoost = 0.70f;
                        BoostTime += Time.deltaTime;
                        Debug.Log(BoostTime);
                        if (BoostTime > 10)
                        {
                            BoostTime = 0;
                            upgradeEquiped = 0;
                            speedBoost = 1f;
                            Pcollider.enabled = true;
                            Destroy(equipedItem);
                        }
                    }
                    else
                    {
                        Pcollider.enabled = true;
                        speedBoost = 1f;
                    }
                    break;
                case 3: //Blue Substance
                    if (Input.GetKey(KeyCode.Mouse1))
                    {
                        speedBoost = 2f;
                        BoostTime += Time.deltaTime;
                        Debug.Log(BoostTime);
                        if (BoostTime > 30)
                        {
                            BoostTime = 0;
                            upgradeEquiped = 0;
                            speedBoost = 1;
                            Destroy(equipedItem);
                        }
                    } else
                    {
                        speedBoost = 1;
                    }
                    break;
                case 4: //Radar, the script is found elsewhere lmfao

                    break;
            }
            //show upgrade remaining meter when boostTime is over 0
            if (BoostTime > 0)
            {
                itemRemainBarActive.SetActive(true);
                int max = 0;
                switch (upgradeEquiped)
                {
                    case 2:
                        max = 10;
                        break;
                    case 3:
                        max = 30;
                        break;
                }
                itemRemainingBar.maxValue = max;
                itemRemainingBar.value = max-BoostTime;
            } else
            {
                itemRemainBarActive.SetActive(false);
            }
            //drain stamina when sprinting, if stamina is 0 don't allow sprint
            sprintKey = Input.GetAxis("Sprint");
            if (staminaBar != null)
            {
                if (sprintKey > 0 && !isProne)
                {
                    if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                    {
                        if (staminaBar.value <= 0)
                        {
                            sprintKey = 0;
                        }
                        staminaBar.value -= Time.deltaTime / stamina;
                    }
                }
                else
                {
                    staminaBar.value += Time.deltaTime / (stamina * 1.5f);
                }
            }

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
                    Physics.AddRelativeForce(new Vector3(speed * Time.deltaTime * Input.GetAxis("Horizontal") * (sprintKey + 1.5f) * speedBoost, 0, speed * Time.deltaTime * Input.GetAxis("Vertical") * (sprintKey + 2f) * speedBoost));
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
            if (velocity > 0) //play footsteps dependant on speed, also notify all entities about them
            {
                velocity = Mathf.Clamp(velocity, 2, 10)/2;
                stepDelay = stepDelay + Time.deltaTime;
                if (stepDelay >= 2 / velocity)
                {
                    footStep.volume = velocity/5*mainMenuCtrl.footStepV;
                    stepDelay = 0;
                    if (!isProne)
                    {
                        footStep.Play();
                    }
                    if (Imposter != null)
                    {
                        Imposter.HearPlayer(velocity / 5);
                    }
                    if (Shrek != null)
                    {
                        Shrek.HearPlayer(velocity / 5);
                    }
                    if (QuandaleA != null)
                    {
                        QuandaleA.HearPlayer(velocity / 5);
                    }
                    if (QuandaleB != null)
                    {
                        QuandaleB.HearPlayer(velocity / 5);
                    }
                    if (QuandaleC != null)
                    {
                        QuandaleC.HearPlayer(velocity / 5);
                    }
                }
            }
        }
        //if dead, fade in the death screen
        if (dead)
        {
            switch (killer)
            {
                case 0: //case 0, imposter
                    ImpFoundYou.color = new Color(255, 255, 255, time-1.5f);
                    break;
                case 1:// case 1, shrek
                    ShrekFoundYou.color = new Color(255, 255, 255, time - 1.5f);
                    break;
                case 2:// case 2, quandaleA
                    QuandaleFoundYouA.color = new Color(255, 255, 255, time - 1.5f);
                    break;
                case 3:// case 3, quandaleB
                    QuandaleFoundYouB.color = new Color(255, 255, 255, time - 1.5f);
                    break;
                case 4:// case 4, quandaleC
                    QuandaleFoundYouC.color = new Color(255, 255, 255, time - 1.5f);
                    break;
                case 5:// case 5, hamood habibi
                    HamoodFoundYou.color = new Color(255, 255, 255, time - 1.5f);
                    break;
            }
            time = time + Time.deltaTime/2;
            if (time >= 3.5f)
            {
                DeathUI.SetActive(true);
                switch (killer)
                {
                    case 0:
                        DeathText.text = "Ejected";
                        break;
                    case 1:
                        DeathText.text = "Shreked";
                        break;
                    case 2:
                        DeathText.text = "Dingled";
                        break;
                    case 3:
                        DeathText.text = "Dingled";
                        break;
                    case 4:
                        DeathText.text = "Dingled";
                        break;
                    case 5:
                        DeathText.text = "Arabic";
                        break;
                }
            }
        }
    }
    public void DeathAnim(Transform Killer, Vector3 Offset, Quaternion OffsetRotation)
    {
        QuandaleA.seekPhase = 0;
        QuandaleB.seekPhase = 0;
        QuandaleC.seekPhase = 0;
        Shrek.seekPhase = 0;
        Imposter.seekPhase = 0;
        UICanvas.enabled = false;
        menuUp = false;
        canMove = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        //transform.rotation = Quaternion.Euler(transform.rotation.x, Mathf.Atan2(transform.position.x - Killer.position.x, transform.position.z - Killer.position.z) * Mathf.Rad2Deg + 180, transform.rotation.z);
        transform.parent = Killer;
        transform.localRotation = OffsetRotation;
        transform.localPosition = Offset;
        
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
