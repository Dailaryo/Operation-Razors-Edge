public class FPFieldSynthesisEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        // 1/combat (CombatCooldown). Creates a single-use pharmaceutical compound
        // from environmental materials. Compound type selection and crafting check
        // require systems not yet implemented (inventory injection mid-combat,
        // PharmaceuticalCraft skill check definition). Full implementation deferred.
        CombatLog.Add($"{ctx.attacker?.unitName} begins field synthesis " +
                      $"(full implementation pending pharmaceutical system).");
    }
}
