public class CMPriorityPatientStatus : StatusEffect
{
    public Unit designatedTarget;

    public CMPriorityPatientStatus(Unit target) : base("CMPriorityPatient", 0)
    {
        isPersistent = true;
        designatedTarget = target;
    }

    // Checked by HealAllyEffect: if target == designatedTarget, double Expert Healer bonus.
    // Persists for the combat duration (no expiry).
}
