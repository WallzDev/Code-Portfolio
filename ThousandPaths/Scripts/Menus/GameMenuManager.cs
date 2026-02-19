using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameMenuManager : MonoBehaviour
{
    public static GameMenuManager instance;

    public GameObject[] menuTabs;
    public GameObject[] menuButtons;
    public GameObject buttonPanel;
    public GameMenuAudio menuAudio;
    public GameObject buttonIndicators;

    public GameObject equipmentSelectItem;
    public GameObject inventorySelectItem;
    public GameObject mapSelectItem;

    public int currentTabIndex = 0;
    public int selectedIndex = 0;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void Update()
    {
        if (UIManager.Instance.subMenusActive == true) return;

        if (Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("BumperLeft"))
        {
            currentTabIndex = (currentTabIndex - 1 + menuTabs.Length) % menuTabs.Length;
            selectedIndex = 0;
            UpdateMenu();
        }

        if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("BumperRight"))
        {
            currentTabIndex = (currentTabIndex + 1) % menuTabs.Length;
            selectedIndex = 0;
            UpdateMenu();
        }
    }

    void UpdateMenu()
    {
        PlayerController.Instance.pState.gameMenuEquipment = false;
        PlayerController.Instance.pState.gameMenuInventory = false;
        PlayerController.Instance.pState.gameMenuMap = false;
        PlayerController.Instance.pState.gameMenuLogs = false;

        var _button = buttonPanel.GetComponent<Animator>();

        for (int i = 0; i < menuTabs.Length; i++)
        {
            menuTabs[i].SetActive(i == currentTabIndex);
        }

        switch (currentTabIndex)
        {
            case 0: _button.SetTrigger("Equipment");
                PlayerController.Instance.pState.gameMenuEquipment = true; break;
            case 1: _button.SetTrigger("Inventory");
                PlayerController.Instance.pState.gameMenuInventory = true; break;
            case 2: _button.SetTrigger("Map");
                PlayerController.Instance.pState.gameMenuMap = true; break;
            case 3: _button.SetTrigger("Logs");
                PlayerController.Instance.pState.gameMenuLogs = true; break;
        }

        menuAudio.PlayDrum();
        
        HighlightSelected();
    }

    void HighlightSelected()
    {
        EventSystem.current.SetSelectedGameObject(null);
        switch (currentTabIndex)
        {
            case 0: equipmentSelectItem.GetComponent<Selectable>().Select(); break;
            case 1: EventSystem.current.SetSelectedGameObject(inventorySelectItem); break;
            case 2: mapSelectItem.GetComponent<Selectable>().Select(); break;
        }
    }
    
    public void ChangeToIndex(int index)
    {
        currentTabIndex = index;
        UpdateMenu();
    }

    public void UnlockSeal(string _seal)
    {
        if (!PlayerController.Instance.unlockedSeals.Contains(_seal))
        {
            PlayerController.Instance.unlockedSeals.Add(_seal);
        }
    }

}
