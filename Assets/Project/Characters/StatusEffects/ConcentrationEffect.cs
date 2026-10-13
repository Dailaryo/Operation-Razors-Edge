using UnityEngine;
using System.Collections.Generic;

//Lives on the manifester while a concentration power is maintained.
//Maintaining costs no action. It ends when:
//  - the manifester fails a damaged concentration check (Unit.TakeDamage)
//  - the manifester can no longer act
//  - the manifester manifests another concentration power (CombatResolver)
//  - every maintained effect has ended on its own
public class ConcentrationEffect : StatusEffect
{
    public PsionicPowerDefinition power;
    private List<StatusEffect> maintained;

    public ConcentrationEffect(PsionicPowerDefinition power, List<StatusEffect> maintained, int maxRounds = 0)
        :base ("Concentration", maxRounds)
    {
        this.power = power;
        this.maintained = maintained;
        isPersistent = maxRounds <= 0;

        foreach (var effect in maintained)
            effect.maintainerCount++;
    }

    public override void OnTurnEnd(Unit unit)
    {
        base.OnTurnEnd(unit); //ticks down the cap when one is set

        //Forget effects that ended on their own (expired or removed from their bearer)
        maintained.RemoveAll(e => e.Target == null || !e.Target.activeEffects.Contains(e));

        //Backstop for incapacitation that didn't come through TakeDamage
        if (maintained.Count == 0 || !unit.CanAct())
        {
            isPersistent = false;
            duration = 0;
        }
    }

    public override void OnExpire(Unit unit)
    {
        foreach (var effect in maintained)
        {
            effect.maintainerCount--;

            //Remove only if no other concentration holds it and it has no time of its own left
            if (effect.maintainerCount <= 0 && effect.IsExpired())
                effect.Target?.RemoveEffect(effect);
        }

        maintained.Clear();
    }
}
