using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioMapping", menuName = "Audio/AudioMapping")]
public class AudioMapping : ScriptableObject
{
    public List<SFXEntry> sfxMappings = new List<SFXEntry>();
    public List<MusicEntry> musicMappings = new List<MusicEntry>();

    [Serializable]
    public class SFXEntry
    {
        public string eventId;
        public AudioClip clip;
        [Range(0f, 1f)] public float volumeScale = 1.0f;
    }

    [Serializable]
    public class MusicEntry
    {
        public GameFlowManager.GameState state;
        public AudioClip clip;
        [Range(0f, 1f)] public float volumeScale = 1.0f;
    }
}