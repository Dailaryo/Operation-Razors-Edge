using UnityEngine;

public class CharacterBackup
{
    private CharacterSnapshot snapshot;
    private int respecCount;

    public static CharacterBackup Capture(CharacterData data)
    {
        var backup = new CharacterBackup();
        backup.snapshot = CharacterSnapshot.Capture(data);
        backup.respecCount = data.respecCount;
        return backup;
    }

    public void Restore(CharacterData data)
    {
        snapshot.Restore(data);
        data.respecCount = respecCount;
    }
}
