using UnityEngine;
using System.Collections.Generic;
using static ClassDefinition;

public static class ClassDatabase
{
    public static Dictionary<string, ClassDefinition> Definitions = new Dictionary<string, ClassDefinition>()
    {
        { ClassIds.Technosavant, TechnosavantClass.Create() },
        { ClassIds.HelixWarrior, HelixWarriorClass.Create() },
        { ClassIds.Infiltrator, InfiltratorClass.Create() },
        { ClassIds.Dreadnought, DreadnoughtClass.Create() },
        { ClassIds.FieldMedic, FieldMedicClass.Create() },
        { ClassIds.Telepath, TelepathClass.Create() },
    };

    public static ClassDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out ClassDefinition classDef)) return classDef;

        Debug.LogWarning($"[ClassNodeDatabase] Class not found: {id}");
        return null;
    }
}
