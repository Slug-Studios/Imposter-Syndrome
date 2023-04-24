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
    public static float ambientV = 0.10f;
    public Slider entValue;
    public static float entityV = 1;
    public static float musicV = 0.25f;
    public Slider musValue;
    public List<AudioSource> musicSources;
    public Slider footStepValue;
    public static float footStepV = 0.175f;
    public Slider SFXValue;
    public static float SFXV = 0.25f;
    public static int lightQuality = 1; //0 is low, 1 is medium, 2 is high
    public static bool postProccessing = false;
    public static float MouseSensitivity = 1;
    public Slider MouseSenSlider;
    public Text MouseSenText;
    public Slider LightQualitySlider;
    public Text LightQText;
    public Toggle PostProcToggle;


    // Start is called before the first frame update
    void Start()
    {
        PostProcToggle.isOn = postProccessing;
        footStepValue.value = footStepV;
        entValue.value = entityV;
        ambValue.value = ambientV;
        musValue.value = musicV;
        SFXValue.value = SFXV;
        LightQualitySlider.value = lightQuality;
        LightQSlider();
        MouseSenSlider.value = MouseSensitivity;
        MouseSenText.text = "Mouse Sensitivity: " + MouseSensitivity.ToString("0.0");
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
    public void FootSlider()
    {
        footStepV = footStepValue.value;
    }
    public void SFXSlider()
    {
        SFXV = SFXValue.value;
    }
    public void MouseSlider()
    {
        MouseSensitivity = MouseSenSlider.value;
        MouseSenText.text = "Mouse Sensitivity: " + MouseSensitivity.ToString("0.0");
    }
    public void LightQSlider()
    {
        lightQuality = (int)LightQualitySlider.value;
        string message = "";
        switch (lightQuality)
        {
            case 0:
                message = "Shit";
                break;
            case 1:
                message = "Ok";
                break;
            case 2:
                message = "Decent";
                break;
        }
        LightQText.text = "Light Quality: " + message;
    }
    public void PostProcessingTogg()
    {
        postProccessing = PostProcToggle.isOn;
    }
}
