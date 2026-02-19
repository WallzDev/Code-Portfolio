using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class FileManager : MonoBehaviour
{
    public enum SaveSlot
    {
        Slot_1,
        Slot_2,
        Slot_3
    }
    public int saveSlotIndex => saveSlot switch
    {
        SaveSlot.Slot_1 => 1,
        SaveSlot.Slot_2 => 2,
        SaveSlot.Slot_3 => 3,
        _ => 0,

    };

    [Header("Slot Number")]
    public SaveSlot saveSlot;

    [Header("Text Elements")]
    public TMP_Text currencyText;

    public TMP_Text locationText;

    public TMP_Text playTimeText;

    public TMP_Text percentageText;

    public TMP_Text newGameText;

    [Header("Visual Elements")]
    public Image inkIcon;

    public Image currencyIcon;

    public Image JungleImage, CaveImage, CastleImage;

    public Image[] healthFlowers;

    private int numberOfFlowers;

    private int flowersToShow;



    void Awake()
    {
        SaveData.Instance.LoadPlayerDataForTitleScreen(saveSlotIndex);

        if (SaveData.Instance.slotUsed)
        {
            #region Things to Enable
            currencyText.enabled = true;
            locationText.enabled = true;
            playTimeText.enabled = true;
            percentageText.enabled = true;
            inkIcon.enabled = true;
            currencyIcon.enabled = true;
            JungleImage.enabled = true;
            CaveImage.enabled = true;
            CastleImage.enabled = true;
            foreach (var item in healthFlowers)
            {
                item.gameObject.SetActive(true);
            };
            #endregion
            newGameText.enabled = false;

            GetSaveLocation();
            ShowMenuHealth();
            SetProfileTime();
            SetMoney();
        }
        else
        {
            #region Things to Disable
            currencyText.enabled = false;
            locationText.enabled = false;
            playTimeText.enabled = false;
            percentageText.enabled = false;
            inkIcon.enabled = false;
            currencyIcon.enabled = false;
            JungleImage.enabled = false;
            CaveImage.enabled = false;
            CastleImage.enabled = false;
            numberOfFlowers = 0;
            flowersToShow = 0;
            foreach (var item in healthFlowers)
            {
                item.gameObject.SetActive(false);
            };
            #endregion

            newGameText.enabled = true;
        }
    }

    void GetSaveLocation()
    {

        string saveScene = SaveData.Instance.toriiSceneName;

        if (saveScene.Contains("JG_"))
        {
            locationText.text = "Jungle";

            JungleImage.gameObject.SetActive(true);
            CaveImage.gameObject.SetActive(false);
            CastleImage.gameObject.SetActive(false);
        }
        else if (saveScene.Contains("TestCave_"))
        {
            locationText.text = "Cave";

            JungleImage.gameObject.SetActive(false);
            CaveImage.gameObject.SetActive(true);
            CastleImage.gameObject.SetActive(false);
        }
        else if (saveScene.Contains("SpiralCave_"))
        {
            locationText.text = "Spiral Cave";

            JungleImage.gameObject.SetActive(false);
            CaveImage.gameObject.SetActive(false);
            CastleImage.gameObject.SetActive(false);
        }
    }

    public void ShowMenuHealth()
    {
        SaveData.Instance.LoadPlayerDataForTitleScreen(saveSlotIndex);
        numberOfFlowers = SaveData.Instance.playerMaxHealth;

        if (healthFlowers == null)
        {
            Awake();
        }
        if (numberOfFlowers <= 0)
        {
            return;
        }

        flowersToShow = numberOfFlowers;

        for (int i = 0; i < healthFlowers.Length; i++)
        {
            if (i < flowersToShow)
            {
                healthFlowers[i].gameObject.SetActive(true);
            }
            else
            {
                healthFlowers[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetProfileTime()
    {
        playTimeText.text = FormatTime();
    }

    public void SetMoney()
    {
        currencyText.text = SaveData.Instance.playerRice.ToString();
    }


    public string FormatTime()
    {
        float playTime = SaveData.Instance.playTime;

        int hours = Mathf.FloorToInt(playTime / 3600);
        int minutes = Mathf.FloorToInt((playTime % 3600) / 60);

        return string.Format("{0:D2}h {1:D2}m", hours, minutes);
    }

    public void SaveIndexSelected(int index)
    {
        SaveData.Instance.slotIndex = index;
    }

}
