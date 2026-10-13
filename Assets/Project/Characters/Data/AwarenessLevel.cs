using UnityEngine;

//What a unit knows about an opposing unit (separate from DetectionState, which covers where it is)
public enum AwarenessLevel
{
    Unknown,    //Name only
    Basic,      //HP band (healthy / wounded / critical) and threat level (stronger / on par / weaker)
    Detailed,   //Basic, plus Defense and saves
    Full        //Exact HP, resistances, known powers and abilities
}