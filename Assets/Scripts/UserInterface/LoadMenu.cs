using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class LoadMenu : MonoBehaviour
{
    private UIDocument _document;
    private ListView _listView;

    private List<SaveSlotData> saveSlots = new List<SaveSlotData>();

    private void OnEnable()
    {
        BuildSaveSlotList();
        SetupListView();
    }

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        var root = _document.rootVisualElement;

        VisualElement loadMenuRoot = root.Q<VisualElement>("LoadMenu");

        _listView = loadMenuRoot.Q<ListView>("SaveList");
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

             SaveGameData loadedSave = null;

            if (exists)
                loadedSave = SaveSystem.LoadGame(i);

            saveSlots.Add(new SaveSlotData
            {
                slotIndex = i,
                saveName = exists 
                    ? $"Save Slot {i}" : "Empty Slot",

                lastPlayed = loadedSave != null 
                    ? loadedSave.saveDateTime : "No Save Found",

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
            VisualElement row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.height = 60;

            Button loadButton = new Button();
            loadButton.name = "LoadButton";
            loadButton.style.flexGrow = 1;

            Button deleteButton = new Button();
            deleteButton.name = "DeleteButton";
            deleteButton.text = "X";
            deleteButton.style.width = 40;

            row.Add(loadButton);
            row.Add(deleteButton);

            return row;
        };

        // Fills each row
        _listView.bindItem = (element, index) =>
        {
            SaveSlotData slot = saveSlots[index];

            Button loadButton = element.Q<Button>("LoadButton");
            Button deleteButton = element.Q<Button>("DeleteButton");

            loadButton.text = $"{slot.saveName}\n{slot.lastPlayed}";

            int slotIndex = slot.slotIndex;

            loadButton.clicked += () => LoadSlot(slotIndex);

            deleteButton.clicked += () => OnDeleteClicked(slotIndex);
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

    private void OnDeleteClicked(int slot)
    {
        SaveSystem.DeleteSave(slot);

        BuildSaveSlotList();
        _listView.itemsSource = saveSlots;
        _listView.Rebuild();

        Debug.Log($"Data deleted from slot {slot}");
    }

    // --------------------------------------------------
    // Load Save
    // --------------------------------------------------

    private void LoadSlot(int slot)
    {
        SaveGameData loadedSave = SaveSystem.LoadGame(slot);

        if (loadedSave == null)
            return;

        GameState.Instance.LoadFromSave(loadedSave);

        LoadSceneFromSave(loadedSave);
    }

    private void LoadSceneFromSave(SaveGameData save)
    {
        int index = save.currentLevelIndex;

        string sceneName = LevelDatabase.GetSceneName(index);
        SceneManager.LoadScene(sceneName);
    }
}