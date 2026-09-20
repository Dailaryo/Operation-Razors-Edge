public class CMMedicApotheosisEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        // Medic Apotheosis: Battlefield Patch becomes unlimited per round;
        // once per round, treat any ally in line of sight as a Free action.
        // Full implementation deferred — the per-round LoS heal requires
        // a targeting model that differs from the standard adjacent pattern.
        // Stub for now; expand during M27 UI pass when LoS targeting is mature.
        CombatLog.Add($"{ctx.attacker?.unitName} channels Medic Apotheosis.");
    }
}
