using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameViewUserInterface : MonoBehaviour
{
    [SerializeField] protected EntitySystems player;
    private UIDocument _document;
    private VisualElement _characterIcon;
    private List<VisualElement> _spellcards = new List<VisualElement>();
    private VisualElement _spellcardOne;
    private VisualElement _spellcardTwo;
    private VisualElement _spellcardThree;
    private Label _powerValue;
    private Label _fuelValue;
    private ProgressBar _healthValue;
    private ProgressBar _grazeValue;
    
    void Awake()
    {
        _document = GetComponent<UIDocument>();
        _characterIcon = _document.rootVisualElement.Q("CharacterIcon");

        _powerValue = _document.rootVisualElement.Q("PowerValue") as Label;
        _fuelValue = _document.rootVisualElement.Q("FuelValue") as Label;

        _healthValue = _document.rootVisualElement.Q("HealthBar") as ProgressBar;
        _grazeValue = _document.rootVisualElement.Q("GrazeBar") as ProgressBar;

        _spellcardOne = _document.rootVisualElement.Q("Spellcard1");
        _spellcardTwo = _document.rootVisualElement.Q("Spellcard2");
        _spellcardThree = _document.rootVisualElement.Q("Spellcard3");
        _spellcards.Add(_spellcardOne);
        _spellcards.Add(_spellcardTwo);
        _spellcards.Add(_spellcardThree);
    }

    void Start()
    {
        _healthValue.highValue = 50; // or player.GetHealth() if max health
        _healthValue.value = player.GetHealth();

        _grazeValue.highValue = 100; // whatever your max graze is
        _grazeValue.value = player.GetGraze();
    }

    void OnEnable()
    {
        // Make sure bars have correct highValues
        _healthValue.highValue = player.GetMaxHealth(); // e.g., 50
        _grazeValue.highValue = player.GetMaxGraze();   // e.g., 100

        // Refresh UI
        SetValues();

        ResourceService.OnResourceChanged += HandleResourceChanged;
    }

    void OnDisable()
    {
        ResourceService.OnResourceChanged -= HandleResourceChanged;
    }

    private void HandleResourceChanged(ResourceType type, int amount)
    {
        SetValues(); // Refresh the UI immediately
    }

    void SetValues()
    {
        _healthValue.value = player.GetHealth();
        _grazeValue.value = player.GetGraze();
        _fuelValue.text = $": {player.GetFuel()}";
        _powerValue.text = $"{(float)player.GetPower() / 100:F2} / 4.00";

        int bombs = player.GetBombs();

        for(int i=0; i<_spellcards.Count; i++)
        {
            _spellcards[i].SetEnabled(i < bombs);
        }
    }
}
