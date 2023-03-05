using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ending : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<playerController>() != null)
        {
            other.GetComponent<playerController>().Death(-1);
        }
    }
}
