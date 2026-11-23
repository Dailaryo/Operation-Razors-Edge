using System.Collections.Generic;
using UnityEngine;

public class StatusEffect
{
    public string effectName;
    public int duration;

    public bool isPersistent = false;

    //Granted by a feat or passive rather than an action; hidden from the unit label
    public bool isPassive = false;

    //Class passive that needs its own per-unit copy in activeEffects: once-per-combat state, OnApply changes,
    //or code that looks it up in activeEffects. Unit.ApplyClass routes these through AddEffect(CreateInstance());
    //every other class passive stays a shared effectSources entry
    public bool instancedPassive = false;

    //Fresh copy for one unit. Database feat effects are single shared instances, and effects that keep state
    //(Resilience's once-per-combat flag, Target) must not be shared between units, including throwaway portrait Units
    public StatusEffect CreateInstance()
    {
        return (StatusEffect)MemberwiseClone();
    }

    public int sourceLevel = 0;

    //Number of ConcentrationEffects currently holding this effect open
    public int maintainerCount = 0;

    //1 hour+ durations: never expires in combat, carries between encounters, cleared by the Hub restore
    //Will be adjusted once exploration and rest time are implemented
    public bool lastsUntilHub = false;

    //Harmful condition on its bearer (debuffs, lockouts, drains). Sympathetic Link can only transfer these
    public bool isHarmful = false;

    //True for conditions that prevent all actions (Dazed, Stunned, Brain Locked)
    public virtual bool BlocksActions => false;

    //Set by an effect that has finished on its own terms; expires even while held by concentration
    public bool ended = false;
    public Unit Target { get; private set; }
    public int Priority { get; protected set; } = 0;

    //Descriptors this effect carries (e.g. Shaken = Fear | MindAffecting)
    public EffectDescriptor descriptors = EffectDescriptor.None;

    //Bonuses that apply only when the incoming effect carries matching descriptors
    public virtual List<StatModifier> GetConditionalStatModifiers(Unit unit, EffectDescriptor incoming)
    {
        return new List<StatModifier>();
    }

    //Return true to block an incoming effect or power with these descriptors
    public virtual bool BlocksDescriptors(EffectDescriptor incoming, int sourceLevel, Unit bearer)
    {
        return false;
    }

    public StatusEffect(string name, int turns)
    {
        effectName = name;
        duration = turns;
    }

    public virtual void OnApply(Unit unit)
    {
        Target = unit;
    }

    public virtual void OnExpire(Unit unit)
    {
        // Override in subclasses
    }

    //Called on the existing instance when the same effect is applied again (Unit.AddEffect refresh rule)
    public virtual void OnRefresh(StatusEffect incoming) { }

    //Default behavior: decrease duration each turn
    public virtual void OnTurnEnd(Unit unit)
    {
        if (!isPersistent)
            duration--;
    }

    public virtual void OnBeforeAction(ActionContext ctx) { }
    public virtual void OnAfterAction(ActionContext ctx) { }

    //Right before an attack's hit roll (CombatResolver.ApplyPreAttackEffects): adjust ctx.attackRollModifier.
    //Execution phase only. OnBeforeAction runs after the roll, on a hit
    public virtual void OnBeforeAttackRoll(ActionContext ctx) { }

    //Aura hook: called once per end phase for each living unit carrying this effect (as a passive or an active
    //effect). Override to refresh buffs on nearby units (Tactical Uplink, Protective Aura)
    public virtual void OnEndPhaseAura(Unit source, List<Unit> allUnits) { }

    public bool IsExpired()
    {
        if (ended) return true;

        if (isPersistent || lastsUntilHub || maintainerCount > 0) return false;
        return duration <= 0;
    }

    public virtual List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>();
    }

    //Miss chance (percent) on attacks this unit makes (Synesthesia).
    //Not reduced by effects that negate concealment.
    public virtual int GetOutgoingMissChance(Unit owner) => 0;

    //Miss chance (percent) on attacks made against this unit (concealment, Blurred Presence)
    public virtual int GetIncomingMissChance(Unit owner) => 0;

    //Descriptors this effect suppresses on its bearer (Intellect Fortress):
    //matching effects keep ticking down but contribute nothing while suppressed
    public virtual EffectDescriptor SuppressedDescriptors => EffectDescriptor.None;

    //Multiplier on damage the bearer takes from mind-affecting sources (Intellect Fortress II+: 0.5)
    public virtual float MindAffectingDamageMultiplier => 1f;

    //True if a successful save against an incoming effect with these descriptors negates it entirely
    public virtual bool NegatesOnSuccessfulSave(EffectDescriptor incoming) => false;

    //Called when the bearer fails a save (Thought Shield V ends on a failed Will save vs mind-affecting)
    public virtual void OnSaveFailed(Unit owner, Unit.SaveType save, EffectDescriptor incoming) { }

    //True if the bearer cannot manifest psionic powers (Primal Regression)
    public virtual bool BlocksManifesting => false;

    //True if the bearer moves at half speed (Slowed, Synesthesia). Halving never stacks
    public virtual bool HalvesSpeed => false;

    //True if the bearer acts in every surprise round, whichever side gained surprise
    public virtual bool ActsInSurpriseRound => false;

    //DC of a concentration check the bearer must pass to manifest this power; 0 = none (Synesthesia)
    public virtual int GetManifestConcentrationDC(Unit owner, PsionicPowerDefinition power) => 0;

    //Called when this effect actually blocks an incoming effect (not on queries or AI simulation).
    //One-use immunities (Unshakable Zeal) end themselves here
    public virtual void OnBlockedDescriptors(Unit bearer, EffectDescriptor incoming) { }

    //Conditions that must ALL be true for this effect to fire.
    //Null or empty = always fires
    public ConditionType[] Conditions { get; protected set; } = null;

    // Returns true if all declared conditions pass, or if no conditions
    // are declared. Call this at the top of OnBeforeAction/OnAfterAction
    // in subclasses that use the condition system.
    public bool ConditionsMet(ActionContext ctx)
    {
        if (Conditions == null || Conditions.Length == 0) return true;
        return ConditionEvaluator.EvaluateAll(Conditions, ctx);
    }
}
