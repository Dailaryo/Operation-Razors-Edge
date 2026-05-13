using UnityEngine;

public interface IMetamanifestingFeat
{
    //Additional power point cost imposed by this feat
    int GetAdditionalPowerPointCost(PsionicPowerDefinition power);

    //Whether this feat can be appied to the given power
    bool CanApplyTo(PsionicPowerDefinition power);
}
