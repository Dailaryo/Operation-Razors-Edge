using UnityEngine;
using System.Collections.Generic;

public interface IEffectSource
{
    List<StatusEffect> GetEffects(Unit unit);
}
