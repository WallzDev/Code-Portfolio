using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MagicMenuManager : MonoBehaviour
{
    public static MagicMenuManager instance;
    [SerializeField] public GameObject[] magicButtons;

    public bool kunai = false;
    public bool magic2 = false;
    public bool magic3 = false;
    public bool magic4 = false;
    public bool magic5 = false;
    public bool magic6 = false;

    public int lastEquipped;

    private void Awake()
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
        lastEquipped = 0;
    }

    public void InitializeMagics()
    {
        //REMEMBER TO UPDATE BOOL NAMES
        kunai = false;
        magic2 = false;
        magic3 = false;
        magic4 = false;
        magic5 = false;
        magic6 = false;
        UpdateMagicButtons();

    }

    public void UnlockMagic(ref bool magic)
    {
        magic = true;
        UpdateMagicButtons();
    }

    public void UpdateMagicButtons()
    {
        magicButtons[0].SetActive(kunai);
        magicButtons[1].SetActive(magic2);
        magicButtons[2].SetActive(magic3);
        magicButtons[3].SetActive(magic4);
        magicButtons[4].SetActive(magic5);
        magicButtons[5].SetActive(magic6);
    }

    public void SetLastEquipped(int magic)
    {
        lastEquipped = magic;
    }

}
