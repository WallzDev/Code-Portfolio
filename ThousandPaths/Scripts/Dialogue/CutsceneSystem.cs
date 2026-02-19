using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneSystem : MonoBehaviour
{
    [Header("Core")]
    public static CutsceneSystem instance;
    public AudioSource audioSource;

    [Header("Dialogue")]
    public bool isInDialogue;
    public DialogueLines[] lines;
    public int currentLine;
    public float ogTextSpeed;
    public float textSpeed;
    [SerializeField][TextArea] private string[] textInfo;

    [Header("UI")]
    public GameObject dialogueBox;
    public TMP_Text currentText;
    public TMP_Text currentNameText;
    public Image characterImage;
    public Image indicator;
    [HideInInspector] public Animator indicatorAnim;

    [Header("CharacterCheck")]
    public bool solveigTalking;
    public bool asaTalking;
    public bool kurohaTalking;
    public bool ichikaTalking;
    public bool kingTalking;
    public bool mayukaTalking;
    public bool mikaelTalking;


    private Queue<DialogueLines> dialogueQueue;
    private bool isScrolling;

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
        dialogueQueue = new Queue<DialogueLines>();
        ogTextSpeed = textSpeed;
        indicatorAnim = indicator.GetComponent<Animator>();
        this.gameObject.SetActive(false);
    }

    public void Update()
    {
        if (!isInDialogue) return;

        if (textSpeed > ogTextSpeed)
        {
            textSpeed = ogTextSpeed;
        }

        if (Input.GetButtonDown("Submit") || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
        {
            DisplayNextLine();
            if (isScrolling)
            {
                textSpeed /= 7;
            }
        }
        else if (Input.GetButtonUp("Submit") || Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Mouse0))
        {
            textSpeed *= 7;
        }
    }

    public void StartDialogue(DialogueLines[] lines)
    {
        if (isInDialogue) return;

        textSpeed = ogTextSpeed;
        dialogueQueue.Clear();
        foreach (var line in lines)
        {
            if (!PlayerController.Instance.playedDialogueIds.Contains(line.id))
            {
                dialogueQueue.Enqueue(line);

                if (line.deleteAfterPlay)
                {
                    PlayerController.Instance.playedDialogueIds.Add(line.id);
                }
            }
        }
        isInDialogue = true;
        PlayerController.Instance.pState.cantControl = true;
        PlayerController.Instance.rb.velocity = new Vector3(0, 0, 0);
        PlayerController.Instance.ReturnToIdle();
        dialogueBox.SetActive(true);
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (isScrolling) return;

        indicatorAnim.SetBool("End", false);
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }
        if (dialogueQueue.Count == 1)
        {
            indicatorAnim.SetBool("End",true);
        }

        var currentLine = dialogueQueue.Dequeue();
        UpdateUI(currentLine);
        PlayDialogueSound(currentLine);
        StartCoroutine(ScrollText(currentLine.dialogueText));
        
        if (currentLine.action.GetPersistentEventCount() > 0)
        {
            currentLine.action.Invoke();
        }
    }

    private void UpdateUI(DialogueLines line)
    {
        if (line.character.ToString() == "NONE")
        {
            currentNameText.text = "";
            characterImage.sprite = line.GetCharacterPortrait();
        }    
        else
        {
            currentNameText.text = line.character.ToString();
            characterImage.sprite = line.GetCharacterPortrait();
        }
    }
    private IEnumerator ScrollText(string text)
    {
        indicatorAnim.SetTrigger("Scrolling");
        isScrolling = true;
        currentText.text = "";

        foreach (char c in text)
        {
            currentText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        isScrolling = false;
        if (indicatorAnim.GetBool("End") == false)
        {
            indicatorAnim.SetTrigger("Next");
        }

    }

    private void PlayDialogueSound(DialogueLines line)
    {
        if (audioSource == null) return;

        AudioClip clip = line.GetSoundEffect();
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void EndDialogue()
    {
        dialogueBox.SetActive(false);
        isInDialogue = false;
        PlayerController.Instance.pState.cantControl = false;
        this.gameObject.SetActive(false);
    }
}
