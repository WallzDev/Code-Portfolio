using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    public static LogManager Instance;
    public InformationLog[] logs;

    public GameObject logButtons;
    public TextMeshProUGUI sectionTitle;
    public GameObject[] logPages;
    public GameObject peopleSelectItem;
    public GameObject historySelectItem;
    public GameObject placesSelectItem;

    [HideInInspector] public int currentTabIndex = 0;
    [HideInInspector] public int selectedIndex = 0;

    public TextMeshProUGUI logTitle;
    public TextMeshProUGUI logDescription;
    public AudioSource audioSource;
    public AudioClip onSelect;
    public AudioClip onShift;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);
        UpdateLogUI();
    }

    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.A) || (Gamepad.current != null && Gamepad.current.dpad.left.wasPressedThisFrame))
        {
            TrackLastSelected();
            currentTabIndex = (currentTabIndex - 1 + logPages.Length) % logPages.Length;
            selectedIndex = 0;
            UpdateLogUI();
        }

        if (Input.GetKeyDown(KeyCode.D) || (Gamepad.current != null && Gamepad.current.dpad.right.wasPressedThisFrame))
        {
            TrackLastSelected();
            currentTabIndex = (currentTabIndex + 1) % logPages.Length;
            selectedIndex = 0;
            UpdateLogUI();
        }
    }

    public void UnlockLog(string name)
    {
        foreach (var log in logs)
        {
            if (log.logId.Equals(name))
            {
                if (!PlayerController.Instance.unlockedLogs.Contains(log.logId))
                {
                    PlayerController.Instance.unlockedLogs.Add(log.logId);
                    log.CheckLogs();
                    Notifier.instance.NotifyLogEntry("Entry Added: <color=#00FFEF>" + log.logTitle + "</color>");
                }
            }

        }
    }

    public void OnLogSelect(InformationLog log)
    {
        audioSource.PlayOneShot(onShift,.2f);
        if (log.isUnlocked)
        {
            logTitle.text = log.logTitle;
            logDescription.text = log.logInformation;
            log.logImage.gameObject.SetActive(true);
            audioSource.PlayOneShot(onSelect);
        }
    }
    public void OnLogDeselect(InformationLog log)
    {
        log.logImage.gameObject.SetActive(false);
        logTitle.text = "";
        logDescription.text = "";
    }

    public void UpdateLogUI()
    {
        for (int i = 0; i < logPages.Length; i++)
        {
            logPages[i].SetActive(i == currentTabIndex);
        }

        GameObject firstSelectedItem = null;

        switch (currentTabIndex)
        {
            case 0:
                logButtons.GetComponent<Animator>().SetTrigger("People");
                firstSelectedItem = peopleSelectItem;
                //peopleSelectItem.GetComponent<Selectable>().Select();
                sectionTitle.text = "People";
                break;
            case 1:
                logButtons.GetComponent<Animator>().SetTrigger("History");
                firstSelectedItem = historySelectItem;
                //historySelectItem.GetComponent<Selectable>().Select();
                sectionTitle.text = "History";
                break;
            case 2:
                logButtons.GetComponent<Animator>().SetTrigger("Places");
                firstSelectedItem = placesSelectItem;
                //placesSelectItem.GetComponent<Selectable>().Select();
                sectionTitle.text = "Locations";
                break;
        }
        StartCoroutine(ForceSelection(firstSelectedItem));
    }

    public void ChangeToIndex(int index)
    {
        currentTabIndex = index;
        UpdateLogUI();
    }
    private IEnumerator ForceSelection(GameObject item)
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return null;
        EventSystem.current.SetSelectedGameObject(item);
    }

    public void TrackLastSelected()
    {
        switch (currentTabIndex)
        {
            case 0:
                peopleSelectItem = EventSystem.current.currentSelectedGameObject;
                break;
            case 1:
                historySelectItem = EventSystem.current.currentSelectedGameObject;
                break;
            case 2:
                placesSelectItem = EventSystem.current.currentSelectedGameObject;
                break;
        }
    }

}
