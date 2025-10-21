using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    void Awake()
    {
        instance = this;

        audioMixer = Resources.Load<AudioMixer>("Audio/Game");

        forestAmbienceSource = transform.Find("ForestAmbience").gameObject.GetComponent<AudioSource>();
        voiceSource = transform.Find("Voice").gameObject.GetComponent<AudioSource>();
        buttonSource = transform.Find("Button").gameObject.GetComponent<AudioSource>();
        windowSource = transform.Find("Window").gameObject.GetComponent<AudioSource>();
        moneySource = transform.Find("Money").gameObject.GetComponent<AudioSource>();
        pullSource = transform.Find("Pull").gameObject.GetComponent<AudioSource>();
        musicSource = transform.Find("Music").gameObject.GetComponent<AudioSource>();
    }

    private AudioMixer audioMixer;
    public void SetMainVolume() { audioMixer.SetFloat("MainVolume", -50 + GameManager.instance.GetGameMainVolume() * 60); }
    public void SetSfxVolume() { audioMixer.SetFloat("SFXVolume", -50 + GameManager.instance.GetGameSfxVolume() * 60); }
    public void SetMusicVolume() { audioMixer.SetFloat("MusicVolume", -50 + GameManager.instance.GetGameMusicVolume() * 60); }

    private AudioSource forestAmbienceSource;
    private AudioSource voiceSource;
    private AudioSource buttonSource;
    private AudioSource windowSource;
    private AudioSource moneySource;
    private AudioSource pullSource;
    private AudioSource musicSource;

    [SerializeField]
    private AudioClip[] wildlifeAudioClips;
    private float wildlifeAudioTimer = 0;
    private float wildlifeAudioDelay;

    [SerializeField]
    private AudioClip[] voiceAudioClips;
    private float voiceTimer = 0;

    [SerializeField]
    private AudioClip[] moneyAudioClips;

    [SerializeField]
    private AudioClip[] pullAudioClips;

    [SerializeField]
    private AudioClip[] musicClips;
    [SerializeField]
    private List<AudioClip> playedMusicClips;
    [SerializeField]
    private float musicTimer = 0;
    private AudioClip lastPlayedMusicClip;

    void Start()
    {
        audioMixer.SetFloat("MainVolume", -50 + GameManager.instance.GetGameMainVolume() * 60);
        audioMixer.SetFloat("SFXVolume", -50 + GameManager.instance.GetGameSfxVolume() * 60);
        audioMixer.SetFloat("MusicVolume", -50 + GameManager.instance.GetGameMusicVolume() * 60);
        
        DirectoryInfo audioDir = new DirectoryInfo(Application.dataPath + "/Resources/Audio");
        // FileInfo[] wildlifeFiles = audioDir.GetFiles("ambience_*.wav");
        wildlifeAudioClips = new AudioClip[GameManager.instance.GetAmbienceSoundCount()];
        // for (int fileIndex = 0; fileIndex < wildlifeAudioClips.Length; fileIndex++) { wildlifeAudioClips[fileIndex] = Resources.Load<AudioClip>("Audio/" + wildlifeFiles[fileIndex].Name.Replace(".wav", "")); }
        for (int fileIndex = 0; fileIndex < wildlifeAudioClips.Length; fileIndex++) { wildlifeAudioClips[fileIndex] = Resources.Load<AudioClip>("Audio/ambience_" + fileIndex); }
        wildlifeAudioDelay = GameManager.instance.GetWildlifeAudioDelay();

        // FileInfo[] voiceFiles = audioDir.GetFiles("voice_*.wav");
        voiceAudioClips = new AudioClip[GameManager.instance.GetVoiceSoundCount()];
        for (int fileIndex = 0; fileIndex < voiceAudioClips.Length; fileIndex++) { voiceAudioClips[fileIndex] = Resources.Load<AudioClip>("Audio/voice_" + fileIndex); }

        // FileInfo[] moneyFiles = audioDir.GetFiles("money_*.wav");
        moneyAudioClips = new AudioClip[GameManager.instance.GetMoneySoundCount()];
        for (int fileIndex = 0; fileIndex < moneyAudioClips.Length; fileIndex++) { moneyAudioClips[fileIndex] = Resources.Load<AudioClip>("Audio/money_" + fileIndex); }

        // FileInfo[] pullFiles = audioDir.GetFiles("pull_*.wav");
        pullAudioClips = new AudioClip[GameManager.instance.GetPullSoundCount()];
        for (int fileIndex = 0; fileIndex < pullAudioClips.Length; fileIndex++) { pullAudioClips[fileIndex] = Resources.Load<AudioClip>("Audio/pull_" + fileIndex); }

        // FileInfo[] musicFiles = audioDir.GetFiles("music_*.wav");
        musicClips = new AudioClip[GameManager.instance.GetMusicCount()];
        for (int fileIndex = 0; fileIndex < musicClips.Length; fileIndex++) { musicClips[fileIndex] = Resources.Load<AudioClip>("Audio/music_" + fileIndex); }
        musicTimer = GameManager.instance.GetGameMusicDelay() - 3;
        playedMusicClips = new List<AudioClip>();

        buttonSource.clip = Resources.Load<AudioClip>("Audio/button");
        buttonSource.loop = false;

        windowSource.clip = Resources.Load<AudioClip>("Audio/window");
        windowSource.loop = false;

        StartForestAmbience();
    }

    void Update()
    {
        ProcessWildlifeAudio();
        ProcessVoiceAudio();
        ProcessMusic();
    }

    private void StartForestAmbience()
    {
        forestAmbienceSource.clip = Resources.Load<AudioClip>("Audio/forest");
        forestAmbienceSource.loop = true;
        forestAmbienceSource.Play();
    }

    public void PlayButtonPress()
    {
        buttonSource.Play();
    }

    public void PlayWindowOpen()
    {
        windowSource.Play();
    }

    public void PlayMoneySound()
    {
        moneySource.clip = moneyAudioClips[Random.Range(0, moneyAudioClips.Length)];
        moneySource.loop = false;
        moneySource.Play();
    }

    public void PlayPullSound(int rarity)
    {
        pullSource.clip = pullAudioClips[rarity];
        pullSource.loop = false;
        pullSource.Play();
    }

    public void PlayMenuMusic()
    {
        musicSource.clip = GameManager.instance.GetGameMenuMusic();
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource.isPlaying) { musicSource.Stop(); }
    }

    private void ProcessWildlifeAudio()
    {
        wildlifeAudioTimer += Time.deltaTime;

        if (wildlifeAudioTimer < wildlifeAudioDelay) { return; }
        wildlifeAudioTimer = 0;
        wildlifeAudioDelay = GameManager.instance.GetWildlifeAudioDelay();

        // Create new audio object.
        GameObject newWildlifeAudioObject = Instantiate(
            Resources.Load<GameObject>("Prefabs/Map/WildlifeAudioPrefab"),
            Vector3.zero,
            Quaternion.identity
        );

        // Set location to randomly be around the field.
        float xCoord = Random.Range((GameManager.instance.GetMapBaseWidth() + GameManager.instance.GetMapTreeRingOffset() + GameManager.instance.GetMapFillerRingOffset()) / 2 * -1, (GameManager.instance.GetMapBaseWidth() + GameManager.instance.GetMapTreeRingOffset() + GameManager.instance.GetMapFillerRingOffset()) / 2);
        if (xCoord > GameManager.instance.GetMapBaseWidth() / 2 * -1 && xCoord < GameManager.instance.GetMapBaseWidth() / 2)
        {
            float calcValue = xCoord + (GameManager.instance.GetMapBaseWidth() / 2);
            if (calcValue - 0 < GameManager.instance.GetMapBaseWidth() - calcValue) { calcValue = 0; }
            else { calcValue = GameManager.instance.GetMapBaseWidth(); }
            xCoord = calcValue - (GameManager.instance.GetMapBaseWidth() / 2);
        }
        float zCoord = Random.Range((GameManager.instance.GetMapBaseWidth() + GameManager.instance.GetMapTreeRingOffset() + GameManager.instance.GetMapFillerRingOffset()) / 2 * -1, (GameManager.instance.GetMapBaseWidth() + GameManager.instance.GetMapTreeRingOffset() + GameManager.instance.GetMapFillerRingOffset()) / 2);
        if (zCoord > GameManager.instance.GetMapBaseWidth() / 2 * -1 && zCoord < GameManager.instance.GetMapBaseWidth() / 2)
        {
            float calcValue = zCoord + (GameManager.instance.GetMapBaseWidth() / 2);
            if (calcValue - 0 < GameManager.instance.GetMapBaseWidth() - calcValue) { calcValue = 0; }
            else { calcValue = GameManager.instance.GetMapBaseWidth(); }
            zCoord = calcValue - (GameManager.instance.GetMapBaseWidth() / 2);
        }
        newWildlifeAudioObject.transform.position = new Vector3(xCoord, 0, zCoord);

        newWildlifeAudioObject.transform.Find("Source").gameObject.GetComponent<AudioSource>().clip = wildlifeAudioClips[Random.Range(0, wildlifeAudioClips.Length)];
        newWildlifeAudioObject.transform.Find("Source").gameObject.GetComponent<AudioSource>().loop = false;
        newWildlifeAudioObject.transform.Find("Source").gameObject.GetComponent<AudioSource>().Play();
        Destroy(newWildlifeAudioObject, newWildlifeAudioObject.transform.Find("Source").gameObject.GetComponent<AudioSource>().clip.length + 1);
    }

    private void ProcessVoiceAudio()
    {
        if (!PlayerViewState.Dialogue.Equals(GameManager.instance.GetPlayerViewState())) { return; }
        if (!ViewManager.instance.IsSpokenTextRendering()) { return; }

        voiceTimer += Time.deltaTime;

        if (voiceTimer < 0.15f) { return; }
        voiceTimer = 0;

        voiceSource.clip = voiceAudioClips[Random.Range(0, voiceAudioClips.Length)];
        voiceSource.loop = false;
        voiceSource.Play();
    }
    
    private void ProcessMusic()
    {
        if (PlayerViewState.StartMenu.Equals(GameManager.instance.GetPlayerViewState())) { return; }
        if (!OpeningCutsceneState.None.Equals(CutsceneManager.instance.GetOpeningCutsceneState())) { return; }
        if (musicSource.isPlaying) { return; }

        musicTimer += Time.deltaTime;

        if (musicTimer < GameManager.instance.GetGameMusicDelay()) { return; }
        musicTimer = 0;

        List<AudioClip> musicList = new List<AudioClip>();
        musicList.AddRange(musicClips);
        foreach (AudioClip playedMusicClip in playedMusicClips) { musicList.Remove(playedMusicClip); }
        if (lastPlayedMusicClip != null && musicList.Contains(lastPlayedMusicClip)) { musicList.Remove(lastPlayedMusicClip); }

        AudioClip musicClip = musicList[Random.Range(0, musicList.Count)];
        musicSource.clip = musicClip;
        musicSource.loop = false;
        musicSource.Play();

        lastPlayedMusicClip = musicClip;
        playedMusicClips.Add(musicClip);

        if (playedMusicClips.Count == musicClips.Length) { playedMusicClips.Clear(); }
    }
}