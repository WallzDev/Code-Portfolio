using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMessageManager : MonoBehaviour
{
    public static PlayerMessageManager Instance { get; private set; }
    [Header("Player Message Variables")]
    [SerializeField] private GameObject mainObject;
    [SerializeField] private GameObject continueObj;
    [SerializeField] private Animator messageAnim;
    [SerializeField] private TextMeshProUGUI textObject;
    private string hiddenMessage;
    [HideInInspector] public float lifeTime = 0;
    [SerializeField] private float textSpeed;
    public bool messageFinished = false;

    [Header("Player Document Variables")]
    [SerializeField] private GameObject mainDocObject;
    [SerializeField] private Animator docAnim;
    [SerializeField] private TextMeshProUGUI docTitleObject;
    [SerializeField] private TextMeshProUGUI docTextObject;
    [SerializeField] private Image docBackgroundObject;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowMessage(string message)
    {
        if (hiddenMessage == message) return;

        StopAllCoroutines();
        mainObject.SetActive(true);
        messageAnim.Play("PlayerMessageAppear");
        StartCoroutine(EnableClickObj()); // added
        //StartCoroutine(ScrollText(message)); REMOVED SO THAT THE MESSAGE APPEARS INSTANTLY
        textObject.text = message; // ADDED SO THAT THE MESSAGE APPEARS INSTANTLY
        hiddenMessage = message; // Also added
    }

    public void HideMessage()
    {
        StartCoroutine(DissappearMessage());
    }

    private IEnumerator ScrollText(string text)
    {
        hiddenMessage = text;
        textObject.text = "";

        foreach (char c in text)
        {
            textObject.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        StartCoroutine(DissappearMessage());
    }

    public IEnumerator DissappearMessage()
    {
        //yield return new WaitForSeconds(lifeTime); REMOVED
        messageAnim.SetTrigger("Disappear");
        continueObj.SetActive(false);
        messageFinished = true;
        yield return new WaitForSeconds(.669f); // Lo que tarde la animacion de desaparicion del texto
        textObject.text = "";
        hiddenMessage = "";
        mainObject.SetActive(false);
        messageFinished = false;
    }

    public void ShowDocument(PlayerDocSO document)
    {
        docTitleObject.text = document.title;
        docTextObject.text = document.bodyText;
        docBackgroundObject.sprite = document.backgroundImage;
        docBackgroundObject.color = document.backgroundColor;
        mainDocObject.SetActive(true);
        docAnim.Play("PlayerDocumentAppear");
        docBackgroundObject.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
    }

    public IEnumerator EnableClickObj()
    {
        yield return new WaitForSeconds(.4f);
        continueObj.SetActive(true);
    }

}
