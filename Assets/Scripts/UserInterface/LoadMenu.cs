using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class LoadMenu : MonoBehaviour
{
    private UIDocument _document;
    private ListView _listView;

    private List<SaveSlotData> saveSlots = new List<SaveSlotData>();

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        var root = _document.rootVisualElement;

        VisualElement loadMenuRoot = root.Q<VisualElement>("LoadMenu");

        _listView = loadMenuRoot.Q<ListView>("SaveList");

        BuildSaveSlotList();
        SetupListView();
    }

    // --------------------------------------------------
    // Create Save Slot Data
    // --------------------------------------------------

    private void BuildSaveSlotList()
    {
        saveSlots.Clear();

        for (int i = 0; i < 5; i++)
        {
            bool exists = SaveSystem.SaveExists(i);

            saveSlots.Add(new SaveSlotData
            {
                slotIndex = i,
                saveName = exists ? $"Save Slot {i}" : "Empty Slot",
                lastPlayed = exists ? "Has Save Data" : "No Save Found",
                hasSave = exists
            });
        }
    }

    // --------------------------------------------------
    // Setup ListView
    // --------------------------------------------------

    private void SetupListView()
    {
        _listView.itemsSource = saveSlots;

        // Creates each row
        _listView.makeItem = () =>
        {
            Button button = new Button();
            button.style.height = 60;
            return button;
        };

        // Fills each row
        _listView.bindItem = (element, index) =>
        {
            Button button = element as Button;
            SaveSlotData slot = saveSlots[index];

            button.text = $"{slot.saveName}\n{slot.lastPlayed}";

            button.userData = slot.slotIndex;

            button.clicked -= OnSlotClicked;
            button.clicked += OnSlotClicked;
        };

        _listView.fixedItemHeight = 60;
        _listView.selectionType = SelectionType.None;

        _listView.Rebuild();
    }

    private void OnSlotClicked()
    {
        Button button = _listView.panel.focusController.focusedElement as Button;

        if (button == null)
            return;

        int slot = (int)button.userData;

        LoadSlot(slot);
    }

    // --------------------------------------------------
    // Load Save
    // --------------------------------------------------

    private void LoadSlot(int slot)
    {
        SaveGameData loadedSave =
            SaveSystem.LoadGame(slot);

        if (loadedSave == null)
            return;

        GameState.Instance.LoadFromSave(loadedSave);

        SceneManager.LoadScene("MapScene");
    }
}