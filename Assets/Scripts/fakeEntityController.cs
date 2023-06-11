using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fakeEntityController : MonoBehaviour
{
    public int type;
    private AudioSource Source;
    public List<AudioClip> clipList;
    private float time2;
    public float mood;
    public float speed;
    public Transform Player;
    private Rigidbody physicsBody;
    private Vector3 dst;

    // Start is called before the first frame update
    void Start()
    {
        Source = gameObject.AddComponent<AudioSource>();
        Source.volume = mainMenuCtrl.entityV *mood/2;
        Source.pitch = Random.Range(0 + Mathf.Clamp01(mood / 3), 2 - Mathf.Clamp01(mood / 3));
        Player.gameObject.GetComponent<HallucinationController>().moodModifier = 1;
        physicsBody = GetComponent<Rigidbody>();
        if (type != 1)
        {
            Source.clip = clipList[0];
            Source.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (type)
        {
            case 0://shrek
                if (time2 >= 0.32)
                {
                    Source.Play();
                    time2 = 0;
                }
                time2+= Time.deltaTime;
                break;
            case 1://quandale
                if (Source.isPlaying == false)
                {
                    Source.clip = clipList[Random.Range(0, clipList.Count - 1)];
                    Source.Play();
                }
                break;
            default:
                break;
        }
        dst = transform.position - Player.position;
        transform.rotation = Quaternion.Euler(0, Mathf.Atan2(dst.x, dst.z) * Mathf.Rad2Deg + 180, 0);
        physicsBody.AddRelativeForce(Vector3.forward * speed * Time.deltaTime);
        if (dst.x*dst.x + dst.z*dst.z < 16)
        {
            Player.gameObject.GetComponent<HallucinationController>().moodModifier = 0;
            Destroy(gameObject, 1);
        }
    }
}
