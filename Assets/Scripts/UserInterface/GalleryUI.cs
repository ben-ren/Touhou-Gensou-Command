using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GalleryUI : MonoBehaviour
{
    private UIDocument _document;
    [SerializeField] private VisualTreeAsset _characterCardTemplate;
    //---------------------------------------------------
    //***************Display*Data************************
    //---------------------------------------------------    
    List<CharacterCardUI> charactersList = new List<CharacterCardUI>();
    List<string> levelsList = new List<string>();

    //---------------------------------------------------
    //******************Buttons************************
    //---------------------------------------------------
    Button _characterButton;
    Button _levelsButton;

    //---------------------------------------------------
    //***************Visual*Elements*********************
    //---------------------------------------------------
    private VisualElement _characterGallery;
    private VisualElement _levelsGallery;

    void Awake()
    {
        _document = GetComponent<UIDocument>();

        var root = _document.rootVisualElement;
        

        //-------------Containers----------------
        _characterGallery = root.Q<VisualElement>("CharacterGallery");
        _levelsGallery = root.Q<VisualElement>("LevelGallery");

        //--------------Buttons------------------
        _characterButton = root.Q<Button>("CharacterGalleryButton");
        _levelsButton = root.Q<Button>("LevelGalleryButton");

        //--------------Template------------------

        PopulateDisplays();
        RegisterButtonCallbacks();
    }

    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        
    }

    private void PopulateDisplays()
    {
        BuildCharacterGallery();
        BuildLevelGallery();
        
        
    }

    private void RegisterButtonCallbacks()
    {
        _characterButton.clicked += () => ShowCharacters();
        _levelsButton.clicked += () => ShowLevels();
    }

    private void ShowCharacters()
    {
        _characterGallery.style.visibility = Visibility.Visible;
        _levelsGallery.style.visibility = Visibility.Hidden;
    }

    private void ShowLevels()
    {
        _levelsGallery.style.visibility = Visibility.Visible;
        _characterGallery.style.visibility = Visibility.Hidden;
    }

    /**
    *   Populate character list from CharacterDatabase
    *   Assembles Gallery cards
    */
    private void BuildCharacterGallery()
    {
        _characterGallery.Clear();
        charactersList.Clear();

        foreach (var pair in CharacterDatabase.GetAllCharacters())
        {
            CharacterData data = pair.Value;

            // Create copy from template
            TemplateContainer card = _characterCardTemplate.Instantiate();

            // Wrap UI controller
            CharacterCardUI cardUI = new(card);

            // Bind database data
            cardUI.Bind(data);

            // Store reference
            charactersList.Add(cardUI);

            // Add to gallery
            _characterGallery.Add(card);
        }
    }

    /**
    *   Populate levels list from LevelDatabase
    *   Assemble Gallery cards
    */
    private void BuildLevelGallery()
    {
        for (int i = 0; i < LevelDatabase.GetLevelCount(); i++)
        {
            levelsList.Add(LevelDatabase.GetSceneName(i));
        }
        Debug.Log("Levels" + levelsList);
    }
}