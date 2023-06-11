using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableLightifPlayerClose : MonoBehaviour
{

    public GameObject mainLight;
    public GameObject mainLightHigh;
    public Transform Player_;
    private float dst;
    Vector3 dst3;//I don't like this name
    Vector3 pos;
    /*to avoid null references in main menu
    public void OnEnable()
    {
        if (Player_ != null)
        {
            InvokeRepeating("updateLight", 0, 1);
            Debug.Log("This was enabled on time!"); //never gets called
        }
        else { Debug.Log("Skipped light disable script for " + mainLight.name + ": Player not assigned."); }
    }/**/
    public void Init(in Transform Player__)//in keyword so these can be inited in one line
    {
        Player_ = Player__;
        pos = transform.position;
        //InvokeRepeating("updateLight", 0, 1);
    }
    public void updateLight()//honestly, throwing this in a single class that iterates over all of the lights would be better (only one cal to Player.position)
    {
        dst3 = Player_.transform.position - pos;
        dst = (dst3.x) * (dst3.x) + (dst3.z) * (dst3.z);
        if (dst > 10000)
        {
            mainLight.SetActive(false);//set active is a bit slow, so could only setActive() if object is not yet active
            mainLightHigh.SetActive(false);
        }
        else
        {
            if (mainMenuCtrl.lightQuality == 1)
            {
                mainLight.SetActive(true);
            } else if (mainMenuCtrl.lightQuality == 2)
            {
                mainLight.SetActive(true);
                mainLightHigh.SetActive(true);
            }
            else
            {
                mainLight.SetActive(false);
                mainLightHigh.SetActive(false);
            }
        }
    }
    
}
