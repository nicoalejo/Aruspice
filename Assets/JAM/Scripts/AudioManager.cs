using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum Gamesound
    {
        //Sounds in JAM/Sounds
        ring,
        choose,
        discard,
        win,
        failure,

        //Legacy sounds of the old project. Kept only so the older scripts still
        //compile, they have no clip and playing them does nothing.
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

    public static AudioManager instance = null;

    [Header("Sound Clips (JAM/Sounds)")]
    //sfx_ring
    [SerializeField] private AudioClip ring;
    //sfx_escoger
    [SerializeField] private AudioClip choose;
    //sfx_descartar
    [SerializeField] private AudioClip discard;
    //sfx_win
    [SerializeField] private AudioClip win;
    //sfx_failure
    [SerializeField] private AudioClip failure;

    [Header("Mixing")]
    [Range(0f, 1f)]
    [SerializeField] private float startingVolume = 1.0f;

    private float masterVolume = 1.0f;

    public float MasterVolume
    {
        get => masterVolume;
        set => SetVolume(value);
    }

    //Split in two so a one shot never cuts the music that is playing
    private AudioSource musicSource;
    private AudioSource sfxSource;
    private Dictionary<Gamesound, AudioClip> audioDictionary;

    //Create singleton for Audio Manager
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        CreateSources();
        CreateDictionary();
        SetVolume(startingVolume);
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    private void CreateSources()
    {
        //Reuses an AudioSource already on the object before adding new ones
        AudioSource existing = GetComponent<AudioSource>();

        musicSource = existing != null ? existing : gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
    }

    //Dictionary Creation
    private void CreateDictionary()
    {
        audioDictionary = new Dictionary<Gamesound, AudioClip>();

        Register(Gamesound.ring, ring);
        Register(Gamesound.choose, choose);
        Register(Gamesound.discard, discard);
        Register(Gamesound.win, win);
        Register(Gamesound.failure, failure);
    }

    //Empty slots are skipped, so a missing clip is silence and not an error
    private void Register(Gamesound gamesound, AudioClip clip)
    {
        if (clip == null) return;
        audioDictionary[gamesound] = clip;
    }

    //Continuous reproduction of a sound
    public void StartOnMainPlay(Gamesound gamesound)
    {
        if (!audioDictionary.TryGetValue(gamesound, out AudioClip audioClip)) return;

        musicSource.clip = audioClip;
        musicSource.Play();
    }

    // Play one shot from dictionary
    public void PlayOnShotByDictionary(Gamesound gamesound)
    {
        if (!audioDictionary.TryGetValue(gamesound, out AudioClip audioClip)) return;

        sfxSource.PlayOneShot(audioClip);
    }

    //Volume received in range 0 - 100
    public void ChangeVolume(float newVolume)
    {
        SetVolume(newVolume / 100.0f);
    }

    //Volume received in range 0 - 1
    public void SetVolume(float newVolume)
    {
        masterVolume = Mathf.Clamp01(newVolume);
        musicSource.volume = masterVolume;
        sfxSource.volume = masterVolume;
    }

    public void StopAll()
    {
        musicSource.Stop();
        sfxSource.Stop();
    }
}