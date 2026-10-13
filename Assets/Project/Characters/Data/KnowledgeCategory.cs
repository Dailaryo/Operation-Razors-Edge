using UnityEngine;

//What kind of creature a unit is, for awareness checks. A fixed classification:
//growing familiarity comes from skill ranks, not from changing the category
public enum KnowledgeCategory
{
    Human,          //Knowledge
    KnownAlien,     //Knowledge
    UnknownAlien,   //Xenology
    Construct       //Computer Use (robots, drones, constructs)
}