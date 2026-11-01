using UnityEngine;

//One round of a scripted enemy pattern (D4). EnemyDefinition.attackPattern holds these in order.
//Each round the unit uses the step at Unit.patternStepIndex, then the index advances and wraps.
//Read by EnemyPlanner (Step 1.6); an empty pattern means the utility planner runs as before.
public class PatternStep
{
    public enum TargetRule
    {
        Nearest,             //closest living opponent
        LowestHP,            //living opponent with the lowest current HP
        HighestHP,           //living opponent with the highest current HP
        SpecificCharacterId, //living opponent whose CharacterData.characterId matches (e.g. PregenIds.HarveyJones)
        Self,                //the acting unit (self-buffs, Total Defense)
        None                 //no target (action needs none)
    }

    //null or empty = do nothing this round
    public string actionId;
    public TargetRule targetRule;

    //SpecificCharacterId only: matched against Unit.sourceCharacterData.characterId
    public string specificCharacterId;

    //D5: true = if the action isn't usable from where the unit stands, move toward the target first
    //(attack if that brings it into range, otherwise just close the distance). false = never moves.
    public bool approach;

    public PatternStep(string actionId, TargetRule targetRule = TargetRule.Nearest, 
        string specificCharacterId = null, bool approach = false)
    {
        this.actionId = actionId;
        this.targetRule = targetRule;
        this.specificCharacterId = specificCharacterId;
        this.approach = approach;
    }
}
