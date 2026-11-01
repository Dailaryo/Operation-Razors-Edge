using UnityEngine;
using System;
using System.Collections.Generic;

//D12: id -> portrait sprite, mirroring AudioMapping. Keys are PregenIds values (EnemyIds can be added later).
//No CharacterData or save-schema change; a portraitId on CharacterData can be layered on with custom characters.
[CreateAssetMenu(fileName = "PortraitMapping", menuName = "UI/PortraitMapping")]
public class PortraitMapping : ScriptableObject
{
    public List<PortraitEntry> portraits = new List<PortraitEntry>();

    //Shown for any unmapped id (e.g. a future custom character)
    public Sprite fallbackAlly;

    [Serializable]
    public class PortraitEntry
    {
        public string id;      //PregenIds value, e.g. "harvey_jones"
        public Sprite sprite;
    }

    //Mapped sprite for this id, or the fallback when the id is missing, unmapped or has no sprite
    public Sprite GetPortrait(string id)
    {
        if (!string.IsNullOrEmpty(id))
        {
            PortraitEntry entry = portraits.Find(e => e.id == id);
            if (entry != null && entry.sprite != null) return entry.sprite;
        }

        return fallbackAlly;
    }
}
