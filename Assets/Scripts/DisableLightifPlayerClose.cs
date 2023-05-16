using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableLightifPlayerClose : MonoBehaviour
{

    public GameObject mainLight;
    public GameObject mainLightHigh;
    public Transform Player_;
    private float dst;
    private Vector3 dst3;

    private void OnEnable()
    {
        InvokeRepeating("updateLight", 0, 1);

    }
    void updateLight()
    {
        dst3 = Player_.transform.position - transform.position;
        dst = (dst3.x) * (dst3.x) + (dst3.z) * (dst3.z);
        if (dst > 10000)
        {
            mainLight.SetActive(false);
            mainLightHigh.SetActive(false);
        }
        else
        {
            mainLight.SetActive(true);
            if (mainMenuCtrl.lightQuality == 1)
            {
                mainLight.SetActive(true);
            } else if (mainMenuCtrl.lightQuality == 2)
            {
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
