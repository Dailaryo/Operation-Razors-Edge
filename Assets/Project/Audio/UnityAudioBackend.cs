using UnityEngine;

public class UnityAudioBackend : IAudioBackend
{
    private AudioSource[] sfxPool;
    private AudioSource musicSourceA;
    private AudioSource musicSourceB;

    private int oldestSFXIndex = 0;

    public UnityAudioBackend(AudioSource[] sfxPool, AudioSource musicSourceA, AudioSource musicSourceB)
    {
        this.sfxPool = sfxPool;
        this.musicSourceA = musicSourceA;
        this.musicSourceB = musicSourceB;
    }

    public void PlaySFX(AudioClip clip, float volume)
    {
        AudioSource source = AcquireSFXSource();
        source.clip = clip;
        source.volume = volume;
        source.Play();
    }

    public void PlayMusic(AudioClip clip, float volume)
    {
        AudioSource inactive = GetInactiveMusicSource();
        inactive.clip = clip;
        inactive.volume = volume;
        inactive.Play();
    }

    public void StopMusic()
    {
        AudioSource active = GetActiveMusicSource();
        active.Stop();
        active.clip = null;
    }

    public void SetMusicVolume(AudioSource source, float volume)
    {
        if (source == null) return;
        source.volume = volume;
    }

    public bool IsPlaying(AudioSource source)
    {
        if (source == null) return false;
        return source.isPlaying;
    }

    private AudioSource AcquireSFXSource()
    {
        // First pass: find a source that isn't playing
        for (int i = 0; i < sfxPool.Length; i++)
        {
            if (!sfxPool[i].isPlaying)
                return sfxPool[i];
        }

        // All occupied: interrupt the oldest
        AudioSource oldest = sfxPool[oldestSFXIndex];
        oldestSFXIndex = (oldestSFXIndex + 1) % sfxPool.Length;
        return oldest;
    }

    private AudioSource GetActiveMusicSource()
    {
        if (musicSourceA.isPlaying) return musicSourceA;
        if (musicSourceB.isPlaying) return musicSourceB;
        return musicSourceA;
    }

    private AudioSource GetInactiveMusicSource()
    {
        if (!musicSourceA.isPlaying) return musicSourceA;
        if (!musicSourceB.isPlaying) return musicSourceB;
        return musicSourceB;
    }
}