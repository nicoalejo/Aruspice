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
        eventCard,

        //Music and ambience in JAM/Sounds
        introTheme,
        gameTheme,
        ambience,

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
    //sfx_evento, for the cards that are not people
    [SerializeField] private AudioClip eventCard;

    [Header("Music (JAM/Sounds)")]
    //Intro_final, plays over the start panel and the intro text
    [SerializeField] private AudioClip introTheme;
    //Cancion_final, takes over once the run begins
    [SerializeField] private AudioClip gameTheme;

    [Header("Ambience (JAM/Sounds)")]
    //sfx_ambiente, looped under the music once the run begins
    [SerializeField] private AudioClip ambience;

    [Header("Mixing")]
    [Range(0f, 1f)]
    [SerializeField] private float startingVolume = 1.0f;
    //How loud the ambience sits under the music and the sfx
    [Range(0f, 1f)]
    [SerializeField] private float ambienceVolume = 0.3f;

    private float masterVolume = 1.0f;

    public float MasterVolume
    {
        get => masterVolume;
        set => SetVolume(value);
    }

    //Split in three so a one shot never cuts the music that is playing and the
    //ambience can keep its own, lower volume
    private AudioSource musicSource;
    private AudioSource sfxSource;
    private AudioSource ambienceSource;
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

        ambienceSource = gameObject.AddComponent<AudioSource>();
        ambienceSource.playOnAwake = false;
        ambienceSource.loop = true;
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
        Register(Gamesound.eventCard, eventCard);
        Register(Gamesound.introTheme, introTheme);
        Register(Gamesound.gameTheme, gameTheme);
        Register(Gamesound.ambience, ambience);
    }

    //Empty slots are skipped, so a missing clip is silence and not an error
    private void Register(Gamesound gamesound, AudioClip clip)
    {
        if (clip == null) return;
        audioDictionary[gamesound] = clip;
    }

    //Continuous reproduction of a sound. Asking again for the track that is
    //already playing does nothing, so the music is never restarted from the top.
    public void StartOnMainPlay(Gamesound gamesound)
    {
        if (!audioDictionary.TryGetValue(gamesound, out AudioClip audioClip)) return;
        if (musicSource.clip == audioClip && musicSource.isPlaying) return;

        musicSource.clip = audioClip;
        musicSource.loop = true;
        musicSource.Play();
    }

    //Loops the ambience under whatever the music is doing. Quiet and harmless
    //when no ambience clip was assigned.
    public void StartAmbience()
    {
        if (ambience == null || ambienceSource.isPlaying) return;

        ambienceSource.clip = ambience;
        ambienceSource.Play();
    }

    public void StopAmbience()
    {
        ambienceSource.Stop();
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
        //Kept under the music, so the ambience never fights with the song
        ambienceSource.volume = masterVolume * ambienceVolume;
    }

    public void StopAll()
    {
        musicSource.Stop();
        sfxSource.Stop();
        ambienceSource.Stop();
    }
}