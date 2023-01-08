using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shrekBrain : MonoBehaviour
{
    public Animation Animations;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
            GetComponent<Animator>().Play("Run");
        } else if (Input.GetKey(KeyCode.G))
        {
            GetComponent<Animator>().Play("Scream");
        } else
        {
            GetComponent<Animator>().Play("Walk");
        }
    }
}
