using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class SaveGameMenu : MonoBehaviour
{
    private UIDocument _document;
    private ListView _listView;

    private VisualElement _rootMenu;
    private VisualElement _saveGameMenu;

    private VisualElement _returnMenu;

    private List<SaveSlotData> saveSlots = new();

    void Awake()
    {
        _document = GetComponent<UIDocument>();
        var root = _document.rootVisualElement;

        _saveGameMenu = root.Q<VisualElement>("SaveGameMenu");
        _listView = _saveGameMenu.Q<ListView>("SaveList");

        BuildSaveSlotList();
        SetupListView();
    }

    private void BuildSaveSlotList()
    {
        saveSlots.Clear();

        for (int i = 0; i < 5; i++)
        {
            bool exists = SaveSystem.SaveExists(i);

            SaveGameData loaded = exists ? SaveSystem.LoadGame(i) : null;

            saveSlots.Add(new SaveSlotData
            {
                slotIndex = i,
                saveName = $"Save Slot {i}",

                lastPlayed = loaded != null
                    ? loaded.saveDateTime
                    : "Empty Slot",

                hasSave = exists
            });
        }
    }

    private void SetupListView()
    {
        _listView.itemsSource = saveSlots;

        _listView.makeItem = () =>
        {
            VisualElement row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.height = 60;

            Button saveButton = new Button();
            saveButton.name = "SaveButton";
            saveButton.style.flexGrow = 1;

            Button deleteButton = new Button();
            deleteButton.name = "DeleteButton";
            deleteButton.text = "X";
            deleteButton.style.width = 40;

            row.Add(saveButton);
            row.Add(deleteButton);

            return row;
        };

        _listView.bindItem = (element, index) =>
        {
            SaveSlotData slot = saveSlots[index];

            Button saveButton = element.Q<Button>("SaveButton");
            Button deleteButton = element.Q<Button>("DeleteButton");

            saveButton.text = $"{slot.saveName}\n{slot.lastPlayed}";

            saveButton.userData = slot.slotIndex;
            deleteButton.userData = slot.slotIndex;

            saveButton.clicked += () => OnSaveClicked(slot.slotIndex);

            deleteButton.clicked += () => OnDeleteClicked(slot.slotIndex);
        };

        _listView.fixedItemHeight = 60;
        _listView.selectionType = SelectionType.None;
    }

    private void OnSaveClicked(int slot)
    {
        SaveGameData save = GameState.Instance.CreateSaveData();
        SaveSystem.SaveGame(save, slot);

        BuildSaveSlotList();
        _listView.Rebuild();

        Debug.Log($"Game saved to slot {slot}");
    }

    private void OnDeleteClicked(int slot)
    {
        SaveSystem.DeleteSave(slot);

        BuildSaveSlotList();
        _listView.itemsSource = saveSlots;
        _listView.Rebuild();

        Debug.Log($"Data deleted from slot {slot}");
    }

    public void SetReturnMenu(VisualElement returnToMenu = null)
    {
        _returnMenu = returnToMenu;
    }
}
