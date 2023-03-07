using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eatingItemScript : MonoBehaviour
{
    private Vector3 originalPos;
    public Vector3 eatingPos;
    private Quaternion originalRot;
    public Quaternion eatingRot;
    public float eatingTime;
    public float speedMod;
    public bool wallPhase;
    public float modTime;
    private float time;
    private int eatPhase;
    private AudioSource Sound;
    public playerController player;


    private void Start()
    {
        originalPos = transform.localPosition;
        originalRot = transform.localRotation;
        Sound = GetComponent<AudioSource>();
        Sound.volume = mainMenuCtrl.SFXV;
    }

    // Update is called once per frame
    void Update()
    {
        switch (eatPhase)
        {
            case 0:
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    Sound.Play();
                    transform.localPosition = eatingPos;
                    transform.localRotation = eatingRot;
                }
                if (Input.GetKey(KeyCode.Mouse1))
                {
                    time+= Time.deltaTime;
                    if (time > eatingTime)
                    {
                        transform.localPosition = originalPos;
                        transform.localRotation = originalRot;
                        time = 0;
                        eatPhase = 1;
                    }
                }
                if (Input.GetKeyUp(KeyCode.Mouse1))
                {
                    transform.localPosition = originalPos;
                    transform.localRotation = originalRot;
                    Sound.Stop();
                    time = 0;
                }
                break;
            case 1:
                time += Time.deltaTime;
                player.itemRemainBarActive.SetActive(true);
                player.itemRemainingBar.maxValue = modTime;
                player.itemRemainingBar.value = modTime-time;
                if (wallPhase)
                {
                    player.Pcollider.enabled = false;
                }
                player.speedBoost = speedMod;
                if (time > modTime)
                {
                    player.itemRemainBarActive.SetActive(false);
                    player.Pcollider.enabled = true;
                    player.speedBoost = 1f;
                    Destroy(gameObject);
                }
                break;

        }
    }
}
