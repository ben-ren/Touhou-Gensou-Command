using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class MapViewUI : MonoBehaviour
{
    [Header("References")]
    public Sprite shipSprite;
    private UIDocument _document;
    private GameData _gameData;
    //Display Data
    private VisualElement _ship_image;
    private TextField orbValue;
    private IntegerField moneyValue;
    private IntegerField fuelValue;
    private ProgressBar _flight_range;
    //Character Cards
    private List<CharacterCardUI> characterCards = new();
    /*************************************************************************/
    //--------------ConfirmTurnMenu--------------------
    /*************************************************************************/
    private VisualElement confirmTurnMenu;
    private Button confirmTurnButton;
    private Dictionary<string, System.Action> _buttonActions;
    private List<Button> _buttons;

    VisualElement root = null;
    
    void Awake()
    {
        _document = GetComponent<UIDocument>();
        root = _document.rootVisualElement;
        DisplayFlightRange(root);
        _gameData = GameState.Instance.Data;    // Grab the global data
        _ship_image = root.Q<VisualElement>("ShipImage");
        if(shipSprite!= null)
        {
            _ship_image.style.backgroundImage = new StyleBackground(shipSprite);
        }
    }

    void Update()
    {
        FillCharacterCardData();
        FillDisplayData(root);
    }

    void OnEnable()
    {
        FillDisplayData(root);

        // Step 2: grab the cards container
        var cardsRoot = root.Q<VisualElement>("CharacterCards");

        // Step 3: build card controllers
        characterCards.Clear();

        foreach (var cardElement in cardsRoot.Children())
        {
            characterCards.Add(new CharacterCardUI(cardElement));
        }

        // Subscribe to all path draw generators
        DrawPathGenerator[] generators = FindObjectsByType<DrawPathGenerator>(FindObjectsSortMode.None);
        foreach (var gen in generators)
        {
            gen.OnKnotCountChanged += UpdateFlightRange;
        }

        FillCharacterCardData();

        //ConfirmTurnMenu
        confirmTurnMenu = root.Q<VisualElement>("ConfirmTurnMenu");
        confirmTurnButton = root.Q<Button>("ConfirmTurnButton");
        _buttons = root.Query<Button>().ToList();
        _buttonActions = new()
        {
            { "ConfirmTurnButton", OnConfirmTurnClicked },
            { "YesButton", OnYesButtonClicked },
            { "NoButton", OnNoButtonClicked }
        };
        confirmTurnMenu.style.visibility = Visibility.Hidden;

        foreach (var button in _buttons)
            button.RegisterCallback<ClickEvent>(OnAllButtonsClick);
    }

    void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        DrawPathGenerator[] generators = FindObjectsByType<DrawPathGenerator>(FindObjectsSortMode.None);
        foreach (var gen in generators)
        {
            gen.OnKnotCountChanged -= UpdateFlightRange;
        }

        //ConfirmTurnMenu
        if (_buttons == null) return;
        foreach (var button in _buttons)
            button.UnregisterCallback<ClickEvent>(OnAllButtonsClick);
    }

    void FillDisplayData(VisualElement root)
    {
        // Grab the Orbs value TextField directly by name
        orbValue = root.Q<TextField>("OrbValue");
        moneyValue = root.Q<IntegerField>("MoneyValue");
        fuelValue = root.Q<IntegerField>("FuelValue");

        orbValue.value = $"x {_gameData.orbs}";
        moneyValue.value = _gameData.money;
        fuelValue.value = _gameData.fuel;
    }

    void FillCharacterCardData()
    {
        var party = _gameData.partyMembers;

        int cardCount  = characterCards.Count;
        int partyCount = party.Count;

        int count = Mathf.Min(cardCount, partyCount);

        // Populate active members
        for (int i = 0; i < count; i++)
        {
            CharacterData data = party[i];

            characterCards[i].SetHealth(data.healthData, 100);
            characterCards[i].SetGraze(data.grazeData, 100);
            characterCards[i].SetPower(data.powerData, 4);
            characterCards[i].SetBombs(data.bombsData);

            // Make sure card is visible
            SetCardVisible(characterCards[i], true);
        }

        // Hide unused cards
        for (int i = count; i < cardCount; i++)
        {
            SetCardVisible(characterCards[i], false);
        }
    }

    void SetCardVisible(CharacterCardUI card, bool visible)
    {
        card.root.style.visibility = visible
            ? Visibility.Visible
            : Visibility.Hidden;
    }

    void DisplayFlightRange(VisualElement root)
    {
        _flight_range = root.Q<ProgressBar>("FlightRangeBar");
        _flight_range.style.visibility = Visibility.Hidden;
        _flight_range.value = 100f;
    }

    void UpdateFlightRange(int currentKnotCount, int knotLimit)
    {
        // Show bar when drawing
        _flight_range.style.visibility = Visibility.Visible;

        float remaining = 1f - ((float)currentKnotCount / knotLimit);
        _flight_range.value = Mathf.Clamp01(remaining) * 100f;

        // Hide if full (optional)
        if (currentKnotCount == 0)
            _flight_range.style.visibility = Visibility.Hidden;
    }

    // Central button click handler
    private void OnAllButtonsClick(ClickEvent e)
    {
        if (e.currentTarget is Button button && _buttonActions.TryGetValue(button.name, out var action))
        {
            action.Invoke();
        }
    }

    private void OnConfirmTurnClicked()
    {
        confirmTurnMenu.style.visibility = Visibility.Visible;
        confirmTurnButton.style.visibility = Visibility.Hidden;
    }

    private void OnYesButtonClicked()
    {
        UIController.instance.ExecuteTurn();
        confirmTurnMenu.style.visibility = Visibility.Hidden;
        confirmTurnButton.style.visibility = Visibility.Visible;
        
        Debug.Log("Run token movement and iterate turncounter by 1");
    }

    private void OnNoButtonClicked()
    {
        confirmTurnMenu.style.visibility = Visibility.Hidden;
        confirmTurnButton.style.visibility = Visibility.Visible;
    }
}
