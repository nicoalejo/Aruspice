using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string MainSceneToLoad;
    [SerializeField] private string TutorialToLoad;
    [SerializeField] private Slider progressBar;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private GameObject creditPanel;
    [SerializeField] private Slider sliderVolume;
    [SerializeField] private TextMeshProUGUI textSlideVolume;
    [SerializeField] private Dropdown resolutionDropdown;

    private float volume;
    private float startingVolume;
    private Resolution[] resolutions;
    private void Start()
    {
        //List of all resolutions
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        
        List<string> options = new List<string>();
        
        int currentResolutionIndex = 0;
        
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height + " @ " + resolutions[i].refreshRate + "hz"; ;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        AudioManager.instance.StopAll();
        AudioManager.instance.StartOnMainPlay(AudioManager.Gamesound.menuTheme);
    }

    public void SimpleMainSceneLoader()
    {
        AudioManager.instance.PlayOnShotByDictionary(AudioManager.Gamesound.btnStartGameSFX);
        SceneManager.LoadScene(MainSceneToLoad);
        AudioManager.instance.StopAll();
    }

    public void LoadTutorial()
    {
        AudioManager.instance.PlayOnShotByDictionary(AudioManager.Gamesound.btnStartGameSFX);
        SceneManager.LoadScene(TutorialToLoad);
        AudioManager.instance.StopAll();
    }
    
    public void LoaderScene(){
       StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(MainSceneToLoad);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;
            
            yield return null;
        }
    }

    public void Options()
    {
        PlayBtnSFX();
        optionPanel.SetActive(!optionPanel.activeSelf);
        if (optionPanel.activeSelf)
        {
            sliderVolume.value = AudioManager.instance.MasterVolume * 100.0f;
            startingVolume = sliderVolume.value;
        }
    }

    public void OptionsAccept()
    {
        AudioManager.instance.ChangeVolume(sliderVolume.value);
        Options();
        //TODO: Create Resolution changer
    }
    
    public void OptionsCancel()
    {
        PlayBtnSFX();
        optionPanel.SetActive(!optionPanel.activeSelf);
        AudioManager.instance.ChangeVolume(startingVolume);
    }

    public void OnChangeVolume()
    {
        AudioManager.instance.ChangeVolume(sliderVolume.value);
        textSlideVolume.text = sliderVolume.value + "%";
    }
    
    public void Credits()
    {
        PlayBtnSFX();
        creditPanel.SetActive(!creditPanel.activeSelf);
    }

    public void PlayBtnSFX()
    {
        AudioManager.instance.PlayOnShotByDictionary(AudioManager.Gamesound.btnMainMenuSFX);
    }

    public void Exit()
    {
        PlayBtnSFX();
        Application.Quit();
    }
    
    //Function that sets Quality Settings
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    
    //Function that sets Fullscreen
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    
    //Function that sets Resolution
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = Screen.resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
