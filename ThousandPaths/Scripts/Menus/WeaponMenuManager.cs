using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WeaponMenuManager : MonoBehaviour
{
    public static WeaponMenuManager instance;
    public int lastEquipped;

    [SerializeField] public GameObject[] weaponButtons;
    public bool none;
    public bool katana;
    public bool thiefsKnife;
    public bool woodenStick;
    //SET WEAPON BOOLS HERE ^ AND IN SAVE DATA SCRIPT


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
    public void InitializeWeapons()
    {
        //REMEMBER TO ADD ANY NEW WEAPON BOOLS HERE
        none = true;
        katana = false;
        thiefsKnife = false;
        woodenStick = false;
        UpdateWeaponButtons();
    }

    public void UnlockWeapon(ref bool weapon)
    {
        weapon = true;
        UpdateWeaponButtons();
    }

    public void UpdateWeaponButtons()
    {
        // Assuming weaponButtons in the editor is in the same order as the weapon booleans
        weaponButtons[0].SetActive(none);
        weaponButtons[1].SetActive(katana);
        weaponButtons[2].SetActive(thiefsKnife);
        weaponButtons[3].SetActive(woodenStick);
    }  

    public void SetLastEquipped(int weapon)
    {
        lastEquipped = weapon;
    }

}
