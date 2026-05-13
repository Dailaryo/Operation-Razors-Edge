using UnityEngine;

public interface IAudioBackend
{
    void PlaySFX(AudioClip clip, float volume);
    void PlayMusic(AudioClip clip, float volume);
    void StopMusic();
    void SetMusicVolume(AudioSource source, float volume);
    bool IsPlaying(AudioSource source);
}