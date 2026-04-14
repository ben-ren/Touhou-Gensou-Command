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
        
    }

    void OnEnable()
    {
        // Player might not exist yet → wait safely
        if (player == null)
        {
            InvokeRepeating(nameof(WaitForPlayer), 0f, 0.05f);
            return;
        }

        InitializeUI();
    }

    void OnDisable()
    {
        ResourceService.OnResourceChanged -= HandleResourceChanged;
        CancelInvoke(nameof(WaitForPlayer));
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

    void InitializeUI()
    {
        _healthValue.highValue = player.GetMaxHealth();
        _grazeValue.highValue = player.GetMaxGraze();
        _healthValue.value = player.GetHealth();
        _grazeValue.value = player.GetGraze();

        SetValues();

        ResourceService.OnResourceChanged += HandleResourceChanged;
    }

    void WaitForPlayer()
    {
        if (player == null)
            return;

        CancelInvoke(nameof(WaitForPlayer));
        InitializeUI();
    }

    public EntitySystems GetPlayer()
    {
        return player;
    }

    public void SetPlayer(EntitySystems player)
    {
        this.player = player;
    }
}
