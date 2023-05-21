using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string MainSceneToLoad;
    [SerializeField] private Slider progressBar;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private GameObject creditPanel;
    [SerializeField] private Slider sliderVolume;
    [SerializeField] private TextMeshProUGUI textSlideVolume;

    private float volume;
    private float startingVolume;

    private void Start()
    {
        AudioManager.instance.StopAll();
        AudioManager.instance.StartOnMainPlay(AudioManager.Gamesound.menuTheme);
    }

    public void SimpleMainSceneLoader()
    {
        AudioManager.instance.PlayOnShotByDictionary(AudioManager.Gamesound.btnStartGameSFX);
        SceneManager.LoadScene(MainSceneToLoad);
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
        //TODO: Create Resolution changer
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
        
    }
    
}
