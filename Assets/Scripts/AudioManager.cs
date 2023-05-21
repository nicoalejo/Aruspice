using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum Gamesound
    {
        menuTheme,
        level01MainTheme,
        winTheme,
        loseTheme,
        btnStartGameSFX,
        btnMainMenuSFX,
        selectAltarNumberSFX,
        clickTrickSFX,
        clickAcceptTrickSFX,
        cardDragSFX,
        cardAddAltar,
        allWinTheme,
        cardShuffleSFX,
        cardDealSFX,
        cardSelectSFX
    }

    private float masterVolume = 1.0f;

    public float MasterVolume
    {
        get => masterVolume;
        set => masterVolume = value;
    }

    public static AudioManager instance = null;

    //Sound clips
    [SerializeField] private AudioClip menutheme;
    [SerializeField] private AudioClip level01MainTheme;
    [SerializeField] private AudioClip winTheme;
    [SerializeField] private AudioClip loseTheme;
    [SerializeField] private AudioClip btnStartGameSFX;
    [SerializeField] private AudioClip btnMainMenuSFX;
    [SerializeField] private AudioClip selectAltarNumberSFX;
    [SerializeField] private AudioClip clickTrickSFX;
    [SerializeField] private AudioClip clickAcceptTrickSFX;
    [SerializeField] private AudioClip cardDragSFX;
    [SerializeField] private AudioClip cardAddAltar;
    [SerializeField] private AudioClip allWinTheme;
    [SerializeField] private AudioClip cardShuffleSFX;
    [SerializeField] private AudioClip cardDealSFX;
    [SerializeField] private AudioClip cardSelectSFX;
    
    private AudioSource audioSource;
    private Dictionary<Gamesound, AudioClip> audioDictionary;
    
    //Create singleton for Audio Manager
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            gameObject.AddComponent<AudioSource>();

        CreateDictionary();
        masterVolume = audioSource.volume;
    }

    //Dictionary Creation
    private void CreateDictionary()
    {
        audioDictionary = new Dictionary<Gamesound, AudioClip>();

        audioDictionary.Add(Gamesound.menuTheme, menutheme);
        audioDictionary.Add(Gamesound.winTheme, winTheme);
        audioDictionary.Add(Gamesound.allWinTheme, allWinTheme);
        audioDictionary.Add(Gamesound.loseTheme, loseTheme);
        audioDictionary.Add(Gamesound.btnStartGameSFX, btnStartGameSFX);
        audioDictionary.Add(Gamesound.btnMainMenuSFX, btnMainMenuSFX);
        audioDictionary.Add(Gamesound.clickTrickSFX, clickTrickSFX);
        audioDictionary.Add(Gamesound.selectAltarNumberSFX, selectAltarNumberSFX);
        audioDictionary.Add(Gamesound.clickAcceptTrickSFX, clickAcceptTrickSFX);
        audioDictionary.Add(Gamesound.cardDragSFX, cardDragSFX);
        audioDictionary.Add(Gamesound.cardAddAltar, cardAddAltar);
        audioDictionary.Add(Gamesound.level01MainTheme, level01MainTheme);
        audioDictionary.Add(Gamesound.cardShuffleSFX, cardShuffleSFX);
        audioDictionary.Add(Gamesound.cardDealSFX, cardDealSFX);
        audioDictionary.Add(Gamesound.cardSelectSFX, cardSelectSFX);
        
    }

    //Continuous reproduction of a sound
    public void StartOnMainPlay(Gamesound gamesound)
    {
        if (audioDictionary.TryGetValue(gamesound, out AudioClip audioClip))
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }

    // Play one shot from dictionary
    public void PlayOnShotByDictionary(Gamesound gamesound)
    {
        if (audioDictionary.TryGetValue(gamesound, out AudioClip audioClip))
            audioSource.PlayOneShot(audioClip);
    }

    //Volume received in range 0 - 100
    public void ChangeVolume(float newVolume)
    {
        masterVolume = newVolume/100.0f;
        audioSource.volume = masterVolume;
    }

    public void StopAll()
    {
        if(audioSource.isPlaying)
            audioSource.Stop();
    }
}