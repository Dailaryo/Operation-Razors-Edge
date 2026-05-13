using UnityEngine;
using System;

public static class AudioEventBus
{
    public static event Action<string> OnAudioEvent;

    public static void Fire(string eventId)
    {
        OnAudioEvent?.Invoke(eventId);
    }
}