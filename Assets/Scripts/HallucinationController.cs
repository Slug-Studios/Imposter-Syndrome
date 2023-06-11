using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallucinationController : MonoBehaviour
{
    public List<AudioClip> AmbientSounds;
    private AudioSource AmbientPlayer;
    public float mood;
    public List<GameObject> fakeEntityList;
    private fakeEntityController fakeEntity;
    public float timeElapsed;
    public List<AudioClip> clipList;
    public float moodModifier;
    public AudioClip helloThere;

    // Start is called before the first frame update
    void Start()
    {
        AmbientPlayer = gameObject.AddComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        //Play Ambient sounds
        if (Random.Range(0, 1000f) <= mood && AmbientPlayer.isPlaying == false || Input.GetKeyDown(KeyCode.P))
        {
            AmbientPlayer.clip = AmbientSounds[Random.Range(0, AmbientSounds.Count)];
            AmbientPlayer.volume = mainMenuCtrl.ambientV * Random.Range(30, 100) / 200 * mood;
            AmbientPlayer.pitch = Random.Range(0.5f, 1.5f);
            AmbientPlayer.Play();
        }
        //Hallucination stages
        if (timeElapsed > 600)
        {
            //Auditory
            if (Random.Range(0, 3000f) <= mood && AmbientPlayer.isPlaying == false || Input.GetKeyDown(KeyCode.O))
            {
                int clip = Random.Range(0, 3);
                if (clip == 2)
                {
                    clip = Random.Range(2, clipList.Count);
                    AmbientPlayer.volume = mainMenuCtrl.entityV * mood * Random.Range(0.3f, 0.7f);
                }
                else
                {
                    AmbientPlayer.volume = mainMenuCtrl.entityV * mood * Random.Range(0.01f, 0.02f);
                }
                AmbientPlayer.clip = clipList[clip];
                AmbientPlayer.pitch = Random.Range(0 + mood / 2, 2 - mood / 2);
                AmbientPlayer.Play();
                StartCoroutine(soundDelay());
            }
            if (timeElapsed > 1200)
            {
                //Visual
                if (Random.Range(0, 4000f) <= mood && fakeEntity != null || Input.GetKeyDown(KeyCode.L))
                {
                    float angle = Random.Range(-Mathf.PI * 2, Mathf.PI * 2);
                    int C = Random.Range(0, fakeEntityList.Count - 1);
                    fakeEntity = Instantiate(fakeEntityList[C], new Vector3(50 * Mathf.Sin(angle), fakeEntityList[C].transform.position.y, 50 / Mathf.Cos(angle)), Quaternion.identity).GetComponent<fakeEntityController>();
                    fakeEntity.Player = gameObject.transform;
                    fakeEntity.mood = mood + 0.35f;
                    if (C == 1)
                    {
                        AmbientPlayer.clip = helloThere;
                        AmbientPlayer.volume = mainMenuCtrl.entityV;
                        AmbientPlayer.Play();
                    }
                }
            }
        } 
        mood = timeElapsed / 1200 + moodModifier;
    }
    IEnumerator soundDelay()
    {
        float seconds = Random.Range(5, 5 + mood * 20);
        yield return new WaitForSeconds(seconds);
        AmbientPlayer.Stop();
    }
}
