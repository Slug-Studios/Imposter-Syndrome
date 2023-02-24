using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class radarScript : MonoBehaviour
{
    public static Vector3 target;
    
    // Update is called once per frame
    void Update()
    {
        float angle = Mathf.Atan2 (target.x - transform.position.x, target.z - transform.position.z) * Mathf.Rad2Deg - 180;
        transform.rotation = Quaternion.Euler (0, angle, 0);
    }
}
