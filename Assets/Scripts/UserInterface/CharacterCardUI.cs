using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterCardUI
{
    // Root
    public VisualElement root;

    // Icons
    private VisualElement characterIcon;
    private List<VisualElement> bombIcons = new();

    // Value display
    private ProgressBar healthBar;
    private ProgressBar grazeBar;
    private TextField powerValue;

    public CharacterCardUI(VisualElement root)
    {
        this.root = root;
        CacheElements();
    }

    private void CacheElements()
    {
        // Icons
        characterIcon = root.Q<VisualElement>("CharacterIcon");

        var spellcards = root.Q<VisualElement>("Spellcards");
        bombIcons.AddRange(spellcards.Children());

        // Value display
        healthBar = root.Q<ProgressBar>("HealthBar");
        grazeBar  = root.Q<ProgressBar>("GrazeBar");
        powerValue = root.Q<TextField>("PowerValue");
    }

    // ======================
    // Public API
    // ======================

    public void SetHealth(int current, int max)
    {
        healthBar.lowValue = 0;
        healthBar.highValue = max;
        healthBar.value = current;
    }

    public void SetGraze(int current, int max)
    {
        grazeBar.lowValue = 0;
        grazeBar.highValue = max;
        grazeBar.value = current;
    }

    public void SetPower(int current, int max)
    {
        powerValue.SetValueWithoutNotify(
            $"{current / 100f:0.00} / {max:0.00}"
        );
    }

    public void SetBombs(int bombsRemaining)
    {
        for (int i = 0; i < bombIcons.Count; i++)
        {
            bombIcons[i].style.opacity = i < bombsRemaining ? 1f : 0.25f;
        }
    }
}
