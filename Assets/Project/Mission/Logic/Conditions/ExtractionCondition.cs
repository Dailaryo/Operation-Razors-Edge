using UnityEngine;
using System.Collections.Generic;

// Checks that at least one living player unit is present in the extraction zone.
// Used as a visible objective indicator only — actual extraction is resolved
// by ExtractionMissionManager when the deadline fires.
// Units in the zone remain valid targets and are NOT extracted early.
public class ExtractionCondition : ReachZoneCondition
{
    public ExtractionCondition(string zoneId, int requiredUnitCount = 1)
        : base(zoneId, requiredUnitCount)
    {
    }
}