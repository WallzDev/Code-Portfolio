using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BlessingMenuManager : MonoBehaviour
{
    public List<int> equippedBlessings;
    public static BlessingMenuManager Instance;
    public int casesUsed;
    public int maxCases;
    public BlessingCase[] blessingCases;
    [SerializeField] public BlessingEquip[] blessings;
    public GameObject selector;
    public Image b1Img, b2Img, b3Img, b4Img, b5Img, b6Img;
    private Image emptyImage;

    #region Blessing Bools
    public bool b1;public bool b2;public bool b3;public bool b4;public bool b5;public bool b6;
    public bool b7;public bool b8;public bool b9;public bool b10;public bool b11;public bool b12;
    public bool b13;public bool b14;public bool b15;public bool b16;public bool b17;public bool b18;
    #endregion


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
        DontDestroyOnLoad(gameObject);

        emptyImage = b1Img;
        casesUsed = 0;
        equippedBlessings = new List<int>();
    }

    public void Update()
    {
        foreach (int blessing in equippedBlessings)
        {
            foreach (BlessingEquip item in blessings)
            {
                if (item.blessingIndex == blessing)
                {
                    if (item.blessing == null) continue;

                    item.blessing.Tick();
                    item.blessing.HandleInput();
                }
            }
        }
    }

    public void EquipBlessing(int blessing)
    {

        if (casesUsed < maxCases)
        {
            equippedBlessings.Add(blessing);
            casesUsed++;
            BuildBlessings();
        }
        else
        {
            Debug.Log("This Text is Impossible");
        }
    }
    public void BuildBlessings()
    {
        foreach (BlessingCase i in blessingCases)
        {
            i.blessingStored = 0;
            i.caseUsed = false;
            i.blessingImage.sprite = i.clearImage;
        }

        int index = 0;
        foreach (BlessingCase i in blessingCases)
        {
            if (!i.caseUsed && index < equippedBlessings.Count)
            {
                i.blessingStored = equippedBlessings[index];
                foreach (BlessingEquip blessing in blessings)
                {
                    if (blessing.blessingIndex == i.blessingStored)
                    {
                        i.blessingImage.sprite = blessing.blessingImage;
                    }
                }
                i.caseUsed = true;
                index++;
            }
        }

        UpdateMenuImages();
    }
    public void BuildBlessingFromLoad()
    {
        foreach (int blessing in equippedBlessings)
        {
            foreach (BlessingEquip item in blessings)
            {
                if (item.blessingIndex == blessing)
                {
                    item.OnBlessingPressed(false);
                    if (casesUsed < maxCases)
                    {
                        casesUsed++;
                        BuildBlessings();
                    }
                    else
                    {
                        Debug.Log("This Text is Impossible");
                    }
                }
            }
        }
    }

    public void UnequipBlessing(int blessing)
    {
        equippedBlessings.Remove(blessing);
        casesUsed--;
        BuildBlessings();
    }

    public void InitializeBlessings()
    {
        b1 = false;b2 = false;b3 = false;b4 = false;b5 = false;b6 = false;
        b7 = false;b8 = false;b9 = false;b10 = false;b11 = false;b12 = false;
        b13 = false;b14 = false;b15 = false;b16 = false;b17 = false;b18 = false;
        UpdateUnlockedBlessings();
    }

    public void UpdateUnlockedBlessings()
    {
        blessings[0].gameObject.SetActive(b1); blessings[1].gameObject.SetActive(b2);
        blessings[2].gameObject.SetActive(b3); blessings[3].gameObject.SetActive(b4);
        blessings[4].gameObject.SetActive(b5); blessings[5].gameObject.SetActive(b6);
        blessings[6].gameObject.SetActive(b7); blessings[7].gameObject.SetActive(b8);
        blessings[8].gameObject.SetActive(b9); blessings[9].gameObject.SetActive(b10);
        blessings[10].gameObject.SetActive(b11); blessings[11].gameObject.SetActive(b12);
        blessings[12].gameObject.SetActive(b13); blessings[13].gameObject.SetActive(b14);
        blessings[14].gameObject.SetActive(b15); blessings[15].gameObject.SetActive(b16);
        blessings[16].gameObject.SetActive(b17); blessings[17].gameObject.SetActive(b18);

    }


    public void DoSlideSelector(Vector3 vector)
    {
        StopAllCoroutines();
        StartCoroutine(SlideSelector(vector));
    }
    public IEnumerator SlideSelector(Vector3 target)
    {
        while (Vector3.Distance(selector.transform.position, target) > 0.01f)
        {
            selector.transform.position =
                Vector3.Lerp(selector.transform.position, target, Time.deltaTime * 10);

            yield return null;
        }

        selector.transform.position = target;
    }

    public void UpdateMenuImages()
    {
        foreach (BlessingCase _case in blessingCases)
        {
            if (_case.caseIndex == 1)
            {
                b1Img.sprite = _case.blessingImage.sprite;
            }
            if (_case.caseIndex == 2)
            {
                b2Img.sprite = _case.blessingImage.sprite;
            }
            if (_case.caseIndex == 3)
            {
                b3Img.sprite = _case.blessingImage.sprite;
            }
            if (_case.caseIndex == 4)
            {
                b4Img.sprite = _case.blessingImage.sprite;
            }
            if (_case.caseIndex == 5)
            {
                b5Img.sprite = _case.blessingImage.sprite;
            }
            if (_case.caseIndex == 6)
            {
                b6Img.sprite = _case.blessingImage.sprite;
            }
        }
    }

}
