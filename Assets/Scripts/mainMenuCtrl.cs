using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class mainMenuCtrl : MonoBehaviour
{
    public Canvas mainCanvas;
    public Canvas settingsCanvas;
    public Slider ambValue;
    public static float ambientV = (float)0.05;
    public Slider entValue;
    public static float entityV = 1;
    public static float musicV = (float)0.5;
    public Slider musValue;
    public List<AudioSource> musicSources;
    
    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Slowly rotate camera
        transform.Rotate(Vector3.down * Time.deltaTime * 10);

    }
    //Button stuff
    public void playPress()
    {
        SceneManager.LoadScene("Level 0");
    }
    public void quitPress()
    {
        Application.Quit();
    }
    public void settPress()
    {
        mainCanvas.enabled = !mainCanvas.enabled;
        settingsCanvas.enabled = !settingsCanvas.enabled;
    }
    //Slider shit
    public void ambSlider()
    {
        ambientV = ambValue.value / 10;
        musicSources[0].volume = ambientV;
    }
    public void entSlider()
    {
        entityV = entValue.value;
    }
    public void MusSlider()
    {
        musicV = musValue.value;
        musicSources[1].volume = musicV;
    }
}
