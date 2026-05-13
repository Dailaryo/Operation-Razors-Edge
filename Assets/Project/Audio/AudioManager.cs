using System.Collections;
using UnityEngine;
using static AudioMapping;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public IAudioBackend backend;

    [SerializeField] private AudioMapping mapping;
    [SerializeField] private int sfxPoolSize = 8;
    [SerializeField] private float crossfadeDuration = 1.5f;
    [SerializeField] private float masterVolume = 1.0f;
    [SerializeField] private float sfxVolume = 1.0f;
    [SerializeField] private float musicVolume = 0.8f;

    private AudioSource[] sfxPool;
    private AudioSource musicSourceA;
    private AudioSource musicSourceB;
    private bool musicSourceAActive = true;

    private AudioClip currentMusicClip;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CreateAudioSources();

        backend = new UnityAudioBackend(sfxPool, musicSourceA, musicSourceB);

        AudioEventBus.OnAudioEvent += HandleAudioEvent;
        GameFlowManager.OnStateChanged += HandleStateChanged;
    }

    void OnDestroy()
    {
        AudioEventBus.OnAudioEvent -= HandleAudioEvent;
        GameFlowManager.OnStateChanged -= HandleStateChanged;
    }

    void CreateAudioSources()
    {
        sfxPool = new AudioSource[sfxPoolSize];
        for (int i = 0; i < sfxPoolSize; i++)
        {
            GameObject sfxGo = new GameObject($"SFXSource_{i}");
            sfxGo.transform.SetParent(transform);
            sfxPool[i] = sfxGo.AddComponent<AudioSource>();
        }

        GameObject musicGoA = new GameObject("MusicSourceA");
        musicGoA.transform.SetParent(transform);
        musicSourceA = musicGoA.AddComponent<AudioSource>();
        musicSourceA.loop = true;

        GameObject musicGoB = new GameObject("MusicSourceB");
        musicGoB.transform.SetParent(transform);
        musicSourceB = musicGoB.AddComponent<AudioSource>();
        musicSourceB.loop = true;
    }

    void HandleAudioEvent(string eventId)
    {
        if (mapping == null) return;

        SFXEntry entry = mapping.sfxMappings.Find(e => e.eventId == eventId);
        if (entry == null || entry.clip == null) return;

        float volume = entry.volumeScale * sfxVolume * masterVolume;
        backend.PlaySFX(entry.clip, volume);
    }

    void HandleStateChanged(GameFlowManager.GameState state)
    {
        if (mapping == null) return;

        MusicEntry entry = mapping.musicMappings.Find(e => e.state == state);
        if (entry == null || entry.clip == null) return;
        if (entry.clip == currentMusicClip) return;

        currentMusicClip = entry.clip;
        float volume = entry.volumeScale * musicVolume * masterVolume;
        StartCoroutine(CrossfadeMusic(entry.clip, volume));
    }

    IEnumerator CrossfadeMusic(AudioClip incoming, float targetVolume)
    {
        AudioSource outgoing = musicSourceAActive ? musicSourceA : musicSourceB;
        AudioSource incomingSource = musicSourceAActive ? musicSourceB : musicSourceA;

        backend.PlayMusic(incoming, 0f);

        if (crossfadeDuration <= 0f)
        {
            backend.SetMusicVolume(incomingSource, targetVolume);
            backend.StopMusic();
            musicSourceAActive = !musicSourceAActive;
            yield break;
        }

        float elapsed = 0f;
        float outgoingStartVolume = outgoing.volume;

        while (elapsed < crossfadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / crossfadeDuration);

            backend.SetMusicVolume(incomingSource, Mathf.Lerp(0f, targetVolume, t));
            backend.SetMusicVolume(outgoing, Mathf.Lerp(outgoingStartVolume, 0f, t));

            yield return null;
        }

        backend.SetMusicVolume(incomingSource, targetVolume);
        backend.StopMusic();
        musicSourceAActive = !musicSourceAActive;
    }

    public void SetMasterVolume(float v)
    {
        masterVolume = Mathf.Clamp01(v);
        backend.SetMusicVolume(musicSourceA, musicVolume * masterVolume);
        backend.SetMusicVolume(musicSourceB, musicVolume * masterVolume);
    }

    public void SetSFXVolume(float v)
    {
        sfxVolume = Mathf.Clamp01(v);
    }

    public void SetMusicVolume(float v)
    {
        musicVolume = Mathf.Clamp01(v);
        backend.SetMusicVolume(musicSourceA, musicVolume * masterVolume);
        backend.SetMusicVolume(musicSourceB, musicVolume * masterVolume);
    }
}