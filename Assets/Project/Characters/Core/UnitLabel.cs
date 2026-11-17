using UnityEngine;
using TMPro;

public class UnitLabel : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text hpText;
    public TMP_Text effectsText;

    private Unit unit;

    public Color allyColor = Color.green;
    public Color enemyColor = Color.red;

    public void Initialize(Unit u)
    {
        unit = u;

        nameText.text = unit.unitName;
        UpdateHP();
        UpdateEffects();

        if (unit.isEnemy)
        {
            nameText.color = enemyColor;
        }
        else
        {
            nameText.color = allyColor;
        }

        unit.OnHealthChanged += HandleHealthChanged;
        unit.OnDeath += HandleDeath;
    }

    void HandleHealthChanged(Unit u)
    {
        UpdateHP();
        UpdateEffects();
    }

    void UpdateHP()
    {
        hpText.text = $"HP: {unit.currentHP}";
    }

    void HandleDeath(Unit u)
    {
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (unit != null)
        {
            unit.OnHealthChanged -= HandleHealthChanged;
            unit.OnDeath -= HandleDeath;
        }
    }

    void UpdateEffects()
    {
        if (unit == null) return;

        string text = "";

        foreach (var effect in unit.activeEffects)
        {
            if (effect.isPassive) continue; //feats and passives aren't conditions
            text += effect.effectName + "\n";
        }

        effectsText.text = text;
    }
}
