using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] Image seeds;


    [SerializeField] GameObject dash, doubleJump, wallJump, map;
    [SerializeField] public TextMeshProUGUI riceText;
    [SerializeField] public TextMeshProUGUI coinsText;
    [SerializeField] GameObject[] seals;
    [SerializeField] GameObject sealCases;
    public GameObject selector;


    private void OnEnable()
    {
        //GodSeeds
        seeds.fillAmount = PlayerController.Instance.godSeeds * .39f;

        if (PlayerController.Instance.unlockedWallJump)
        {
            wallJump.SetActive(true);
        }
        else
        {
            wallJump.SetActive(false);
        }

        //Double Jump
        if (PlayerController.Instance.unlockedDoubleJump)
        {
            doubleJump.SetActive(true);
        }
        else
        {
            doubleJump.SetActive(false);
        }

        //Dash
        if (PlayerController.Instance.unlockedDash)
        {
            dash.SetActive(true);
        }
        else
        {
            dash.SetActive(false);
        }

        //Map
        if (PlayerController.Instance.hasMapItem)
        {
            map.SetActive(true);
        }
        else
        {
            map.SetActive(false);
        }

        riceText.text = PlayerController.Instance.riceOwned.ToString();
        coinsText.text = PlayerController.Instance.coinsOwned.ToString();

        //Seal Cases
        if (PlayerController.Instance.learnedSeals)
        {
            sealCases.SetActive(true);
        }
        else
        {
            sealCases.SetActive(false);
        }

        //Each Seal
        foreach (string unlockedSeal in PlayerController.Instance.unlockedSeals)
        {
            foreach (GameObject seal in seals)
            {
                if (PlayerController.Instance.unlockedSeals.Contains(seal.name))
                {
                    seal.SetActive(true);
                }
                else
                {
                    seal.SetActive(false);
                }
            }
        }

    }

    public void DoSlideSelector(Vector3 vector, RectTransform size, Vector3 scale)
    {
        StopAllCoroutines();
        StartCoroutine(SlideSelector(vector, size, scale));
    }

    public IEnumerator SlideSelector(Vector3 target, RectTransform size, Vector3 scale)
    {
        RectTransform selectorRect = selector.GetComponent<RectTransform>();

        while (Vector3.Distance(selector.transform.position, target) > 0.01f ||
            Vector3.Distance(selectorRect.sizeDelta, size.sizeDelta) > 0.01f)
        {
            selector.transform.position =
                Vector3.Lerp(selector.transform.position, target, Time.deltaTime * 20);

            selector.transform.localScale = 
                Vector3.Lerp(selector.transform.localScale, scale, Time.deltaTime * 20);

            selectorRect.sizeDelta = 
                Vector2.Lerp(selectorRect.sizeDelta, size.sizeDelta, Time.deltaTime * 20);

            yield return null;
        }

        selector.transform.position = target;
        selector.transform.localScale = scale;
        selectorRect.sizeDelta = size.sizeDelta;

    }

}
