using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class MapViewUI : MonoBehaviour
{
    [Header("References")]
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

    VisualElement root = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _document = GetComponent<UIDocument>();
        root = _document.rootVisualElement;
        DisplayFlightRange(root);

        // ----- TEST DATA -----    TODO: Figure out how to store data dynamically
        _gameData = new GameData();

        // Resources
        _gameData.orbs = 5;
        _gameData.money = 123;
        _gameData.fuel = 99;

        // Party members
        _gameData.partyMembers = new List<CharacterData>
        {
            new CharacterData
            {
                characterName = "Alice",
                characterIcon = null, // assign a sprite if you have one
                healthData = 80,
                powerData  = 2,
                grazeData  = 50,
                bombsData  = 3
            },
            new CharacterData
            {
                characterName = "Bob",
                characterIcon = null,
                healthData = 60,
                powerData  = 3,
                grazeData  = 40,
                bombsData  = 2
            }
        };
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
    }

    void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        DrawPathGenerator[] generators = FindObjectsByType<DrawPathGenerator>(FindObjectsSortMode.None);
        foreach (var gen in generators)
        {
            gen.OnKnotCountChanged -= UpdateFlightRange;
        }
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
}
