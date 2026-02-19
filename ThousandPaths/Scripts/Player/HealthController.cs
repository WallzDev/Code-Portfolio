using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    PlayerController player;

    private GameObject[] sakuraContainer;
    private Image[] sakuraFilled;
    public Transform sakuraParent;
    public GameObject sakuraContainerPrefab;
    public TextMeshProUGUI riceText;
    public TextMeshProUGUI coinsText;
    [SerializeField] public Animator riceCounter;
    [SerializeField] public Animator coinsCounter;
    public bool riceCollected;
    public bool coinsCollected;

    void Start()
    {
        player = PlayerController.Instance;
        sakuraContainer = new GameObject[PlayerController.Instance.maxTotalHealth];
        sakuraFilled = new Image[PlayerController.Instance.maxTotalHealth];

        PlayerController.Instance.onHealthChangedCallback += UpdateSakuraHUD;
        InstantiateSakuraContainers();
        UpdateSakuraHUD();
        UpdateMoneyHUD();
    }

    void SetSakuraContainer()
    {
        for (int i = 0; i < sakuraContainer.Length; i++)
        {
            if (i < PlayerController.Instance.maxHealth)
            {
                sakuraContainer[i].SetActive(true);
            }
            else
            {
                sakuraContainer[i].SetActive(false);
            }
        }
    }

    void SetFilledSakura()
    {
        for (int i = 0; i < sakuraFilled.Length; i++)
        {
            if (i < PlayerController.Instance.Health)
            {
                sakuraFilled[i].GetComponent<Animator>().SetBool("Filled", true);
            }
            else
            {
                sakuraFilled[i].GetComponent<Animator>().SetBool("Filled", false);
            }
        }
    }

    void InstantiateSakuraContainers()
    {
        for(int i = 0; i < PlayerController.Instance.maxTotalHealth; i++)
        {
            GameObject temp = Instantiate(sakuraContainerPrefab);
            temp.transform.SetParent(sakuraParent, false);
            sakuraContainer[i] = temp;
            sakuraFilled[i] = temp.transform.Find("Flower").GetComponent<Image>();
        }
    }

    void UpdateSakuraHUD()
    {
        SetSakuraContainer();
        SetFilledSakura();
    }

    public void UpdateMoneyHUD()
    {
        if (riceCollected)
        {
            StartCoroutine(RiceCoroutine());
        }
        if (coinsCollected)
        {
            StartCoroutine(CoinsCoroutine());
        }
    }
    public void UpdateMoneyHUDForKatana()
    {
        StartCoroutine(RiceCoroutine());
        StartCoroutine(CoinsCoroutine());
    }

    public IEnumerator RiceCoroutine()
    {
        if (riceCounter.GetBool("Opened") == false)
        {

            riceCollected = false;
            riceCounter.SetTrigger("Open");
            riceCounter.SetBool("Opened", true);
            riceText.text = player.riceOwned.ToString();
            yield return new WaitForSeconds(3f);

            if (!riceCounter.GetBool("Closing"))
            {
                riceCounter.SetBool("Closing", true);
                riceCounter.SetTrigger("Close");
                riceCounter.SetBool("Opened", false);
                yield return new WaitForSeconds(.1f);
                riceCounter.SetBool("Closing", false);
            }
        }
        else
        {
            riceText.text = player.riceOwned.ToString();
        }
    }
    public IEnumerator CoinsCoroutine()
    {
        if (coinsCounter.GetBool("Opened") == false)
        {

            coinsCollected = false;
            coinsCounter.SetTrigger("Open");
            coinsCounter.SetBool("Opened", true);
            coinsText.text = player.coinsOwned.ToString();
            yield return new WaitForSeconds(3f);

            if (!coinsCounter.GetBool("Closing"))
            {
                coinsCounter.SetBool("Closing", true);
                coinsCounter.SetTrigger("Close");
                coinsCounter.SetBool("Opened", false);
                yield return new WaitForSeconds(.1f);
                coinsCounter.SetBool("Closing", false);
            }
        }
        else
        {
            coinsText.text = player.coinsOwned.ToString();
        }
    }

}
