using UnityEngine;
using System.Collections.Generic;

public class RosterPanel : MonoBehaviour
{
    //Called by HubUIController.Show() and after any roster change
    public void Refresh()
    {
        var roster = SquadManager.Instance.GetRoster();
        DisplayRoster(roster);
    }

    void DisplayRoster(List<CharacterData> roster)
    {
        // TODO Milestone 27: Populate roster slot UI elements
        // Each slot should show: character name, level, incapacitated state
        // and wire the select and delete callbacks below
    }

    //Wire to each character slot's select button in Milestone 27
    public void OnSelectCharacter(CharacterData character)
    {
        HubUIController.Instance.OnCharacterSelected(character);
    }

    //Wire to each character slot's delete button in Milestone 27
    public void OnDeleteCharacter(string characterId)
    {
        SquadManager.Instance.RemoveCharacter(characterId);
        Refresh();
        HubUIController.Instance.OnCharacterSelected(null);
    }
}
