using System.IO;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    void Awake() { instance = this; }

    private AudioSource forestAmbienceSource;
    private AudioSource voiceSource;
    private AudioSource buttonSource;
    private AudioSource windowSource;
    private AudioSource moneySource;
    private AudioSource pullSource;

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

    void Start()
    {
        forestAmbienceSource = transform.Find("ForestAmbience").gameObject.GetComponent<AudioSource>();
        voiceSource = transform.Find("Voice").gameObject.GetComponent<AudioSource>();
        buttonSource = transform.Find("Button").gameObject.GetComponent<AudioSource>();
        windowSource = transform.Find("Window").gameObject.GetComponent<AudioSource>();
        moneySource = transform.Find("Money").gameObject.GetComponent<AudioSource>();
        pullSource = transform.Find("Pull").gameObject.GetComponent<AudioSource>();

        DirectoryInfo audioDir = new DirectoryInfo("Assets/Resources/Audio");
        FileInfo[] wildlifeFiles = audioDir.GetFiles("ambience_*.wav");
        wildlifeAudioClips = new AudioClip[wildlifeFiles.Length];
        for (int fileIndex = 0; fileIndex < wildlifeAudioClips.Length; fileIndex++) { wildlifeAudioClips[fileIndex] = Resources.Load<AudioClip>("Audio/" + wildlifeFiles[fileIndex].Name.Replace(".wav", "")); }
        wildlifeAudioDelay = GameManager.instance.GetWildlifeAudioDelay();

        FileInfo[] voiceFiles = audioDir.GetFiles("voice_*.wav");
        voiceAudioClips = new AudioClip[voiceFiles.Length];
        for (int fileIndex = 0; fileIndex < voiceFiles.Length; fileIndex++) { voiceAudioClips[fileIndex] = Resources.Load<AudioClip>("Audio/" + voiceFiles[fileIndex].Name.Replace(".wav", "")); }

        FileInfo[] moneyFiles = audioDir.GetFiles("money_*.wav");
        moneyAudioClips = new AudioClip[moneyFiles.Length];
        for (int fileIndex = 0; fileIndex < moneyFiles.Length; fileIndex++) { moneyAudioClips[fileIndex] = Resources.Load<AudioClip>("Audio/" + moneyFiles[fileIndex].Name.Replace(".wav", "")); }

        FileInfo[] pullFiles = audioDir.GetFiles("pull_*.wav");
        pullAudioClips = new AudioClip[pullFiles.Length];
        for (int fileIndex = 0; fileIndex < pullFiles.Length; fileIndex++) { pullAudioClips[fileIndex] = Resources.Load<AudioClip>("Audio/pull_" + fileIndex); }

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
}