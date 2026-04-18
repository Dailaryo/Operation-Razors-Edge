using System.Collections.Generic;
using UnityEngine;

public interface IActionSource
{
    List<ActionDefinition> GetActions(Unit unit);
}
