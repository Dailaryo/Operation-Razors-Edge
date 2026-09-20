public class FPPharmacistApotheosisEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        // Stim Injection becomes a Free action with effects lasting until end of combat.
        // Duration override requires buff system changes — deferred to buff duration pass.
        CombatLog.Add($"{ctx.attacker?.unitName} channels Pharmacist Apotheosis " +
                      $"(full implementation pending buff duration system).");
    }
}
