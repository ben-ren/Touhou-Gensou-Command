using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class MapViewUI : MonoBehaviour
{
    [Header("References")]
    public Sprite shipSprite;

    private UIDocument _document;
    private GameData _gameData;

    // -----------------------------
    // Display Data
    // -----------------------------
    private VisualElement _ship_image;
    private TextField orbValue;
    private IntegerField moneyValue;
    private IntegerField fuelValue;
    private ProgressBar _flight_range;

    // -----------------------------
    // Character Cards
    // -----------------------------
    private List<CharacterCardUI> characterCards = new();

    // -----------------------------
    // Confirm Turn Menu
    // -----------------------------
    private VisualElement confirmTurnMenu;
    private Button confirmTurnButton;
    private Dictionary<string, System.Action> _buttonActions;
    private List<Button> _buttons;

    // -----------------------------
    // Encounter Menu
    // -----------------------------
    private VisualElement encounterMenu;
    private List<Button> encounterButtons = new();
    private List<TokenController> activeEncounterTokens = new();

    // -----------------------------
    // Root
    // -----------------------------
    private VisualElement root = null;

    // -----------------------------
    // Refresh Timer
    // -----------------------------
    private float encounterRefreshTimer = 0f;
    private const float encounterRefreshInterval = 0.5f;

    void Awake()
    {
        _document = GetComponent<UIDocument>();
        root = _document.rootVisualElement;

        _gameData = GameState.Instance.Data;

        DisplayFlightRange(root);

        _ship_image = root.Q<VisualElement>("ShipImage");
        if (shipSprite != null)
        {
            _ship_image.style.backgroundImage =
                new StyleBackground(shipSprite);
        }
    }

    void OnEnable()
    {
        FillDisplayData(root);

        //-----------------------------------
        // Character Cards Setup
        //-----------------------------------
        var cardsRoot = root.Q<VisualElement>("CharacterCards");
        characterCards.Clear();

        foreach (var cardElement in cardsRoot.Children())
        {
            characterCards.Add(new CharacterCardUI(cardElement));
        }

        FillCharacterCardData();

        //-----------------------------------
        // Subscribe Flight Range Events
        //-----------------------------------
        DrawPathGenerator[] generators =
            FindObjectsByType<DrawPathGenerator>(FindObjectsSortMode.None);

        foreach (var gen in generators)
        {
            gen.OnKnotCountChanged += UpdateFlightRange;
        }

        //-----------------------------------
        // Confirm Turn Menu Setup
        //-----------------------------------
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
        {
            button.RegisterCallback<ClickEvent>(OnAllButtonsClick);
        }

        //-----------------------------------
        // Encounter Menu Setup
        //-----------------------------------
        encounterMenu = root.Q<VisualElement>("EncounterMenu");
        encounterButtons.Clear();

        foreach (var child in encounterMenu.Children())
        {
            if (child is Button button)
            {
                encounterButtons.Add(button);
            }
        }

        // Register encounter button callbacks ONCE
        for (int i = 0; i < encounterButtons.Count; i++)
        {
            int capturedIndex = i;
            encounterButtons[i].clicked +=
                () => StartSelectedEncounter(capturedIndex);
        }

        RefreshEncounterList();
    }

    void OnDisable()
    {
        //-----------------------------------
        // Unsubscribe Flight Range Events
        //-----------------------------------
        DrawPathGenerator[] generators =
            FindObjectsByType<DrawPathGenerator>(FindObjectsSortMode.None);

        foreach (var gen in generators)
        {
            gen.OnKnotCountChanged -= UpdateFlightRange;
        }

        //-----------------------------------
        // Unregister Confirm Buttons
        //-----------------------------------
        if (_buttons != null)
        {
            foreach (var button in _buttons)
            {
                button.UnregisterCallback<ClickEvent>(OnAllButtonsClick);
            }
        }
    }

    void Update()
    {
        FillDisplayData(root);
        FillCharacterCardData();

        //-----------------------------------
        // Refresh encounter list every 0.5 sec
        //-----------------------------------
        encounterRefreshTimer += Time.deltaTime;

        if (encounterRefreshTimer >= encounterRefreshInterval)
        {
            encounterRefreshTimer = 0f;
            RefreshEncounterList();
        }
    }

    // =====================================================
    // DISPLAY DATA
    // =====================================================

    void FillDisplayData(VisualElement root)
    {
        orbValue = root.Q<TextField>("OrbValue");
        moneyValue = root.Q<IntegerField>("MoneyValue");
        fuelValue = root.Q<IntegerField>("FuelValue");

        orbValue.value = $"x {_gameData.orbs}";
        moneyValue.value = _gameData.money;
        fuelValue.value = _gameData.fuel;
    }

    // =====================================================
    // CHARACTER CARDS
    // =====================================================

    void FillCharacterCardData()
    {
        var party = _gameData.partyMembers;

        int cardCount = characterCards.Count;
        int partyCount = party.Count;
        int count = Mathf.Min(cardCount, partyCount);

        for (int i = 0; i < count; i++)
        {
            CharacterData data = party[i];

            characterCards[i].SetHealth(data.healthData, 100);
            characterCards[i].SetGraze(data.grazeData, 100);
            characterCards[i].SetPower(data.powerData, 4);
            characterCards[i].SetBombs(data.bombsData);

            SetCardVisible(characterCards[i], true);
        }

        for (int i = count; i < cardCount; i++)
        {
            SetCardVisible(characterCards[i], false);
        }
    }

    void SetCardVisible(CharacterCardUI card, bool visible)
    {
        card.root.style.visibility =
            visible ? Visibility.Visible : Visibility.Hidden;
    }

    // =====================================================
    // FLIGHT RANGE
    // =====================================================

    void DisplayFlightRange(VisualElement root)
    {
        _flight_range = root.Q<ProgressBar>("FlightRangeBar");
        _flight_range.style.visibility = Visibility.Hidden;
        _flight_range.value = 100f;
    }

    void UpdateFlightRange(int currentKnotCount, int knotLimit)
    {
        _flight_range.style.visibility = Visibility.Visible;

        float remaining =
            1f - ((float)currentKnotCount / knotLimit);

        _flight_range.value =
            Mathf.Clamp01(remaining) * 100f;

        if (currentKnotCount == 0)
        {
            _flight_range.style.visibility =
                Visibility.Hidden;
        }
    }

    // =====================================================
    // CONFIRM TURN BUTTONS
    // =====================================================

    private void OnAllButtonsClick(ClickEvent e)
    {
        if (e.currentTarget is Button button &&
            _buttonActions.TryGetValue(button.name, out var action))
        {
            action.Invoke();
        }
    }

    private void OnConfirmTurnClicked()
    {
        confirmTurnMenu.style.visibility =
            Visibility.Visible;

        confirmTurnButton.style.visibility =
            Visibility.Hidden;
    }

    private void OnYesButtonClicked()
    {
        UIController.instance.ExecuteTurn();

        confirmTurnMenu.style.visibility =
            Visibility.Hidden;

        confirmTurnButton.style.visibility =
            Visibility.Visible;

        Debug.Log("Run token movement and iterate turncounter by 1");
    }

    private void OnNoButtonClicked()
    {
        confirmTurnMenu.style.visibility =
            Visibility.Hidden;

        confirmTurnButton.style.visibility =
            Visibility.Visible;
    }

    // =====================================================
    // ENCOUNTER SYSTEM
    // =====================================================

    void RefreshEncounterList()
    {
        activeEncounterTokens.Clear();

        TokenController[] tokens =
            FindObjectsByType<TokenController>(
                FindObjectsSortMode.None);

        foreach (var token in tokens)
        {
            if (token.enemiesList != null &&
                token.enemiesList.Count > 0)
            {
                activeEncounterTokens.Add(token);
            }
        }

        UpdateEncounterButtons();
    }

    void UpdateEncounterButtons()
    {
        bool hasEncounters =
            activeEncounterTokens.Count > 0;

        encounterMenu.style.display =
            hasEncounters
            ? DisplayStyle.Flex
            : DisplayStyle.None;

        for (int i = 0; i < encounterButtons.Count; i++)
        {
            Button button = encounterButtons[i];

            if (i < activeEncounterTokens.Count)
            {
                TokenController token =
                    activeEncounterTokens[i];

                button.style.display =
                    DisplayStyle.Flex;

                button.text =
                    $"{token.GetCharacterData().characterName} - {token.enemiesList.Count} Enemies";
            }
            else
            {
                button.style.display =
                    DisplayStyle.None;
            }
        }
    }

    void StartSelectedEncounter(int index)
    {
        if (index >= activeEncounterTokens.Count)
            return;

        TokenController selectedToken =
            activeEncounterTokens[index];

        EncounterManager manager =
            FindFirstObjectByType<EncounterManager>();

        manager.StartEncounter(selectedToken);
    }
}