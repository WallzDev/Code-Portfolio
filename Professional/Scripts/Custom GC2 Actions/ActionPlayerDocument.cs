using GameCreator.Runtime.Common;
using GameCreator.Runtime.Dialogue;
using GameCreator.Runtime.VisualScripting;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

[Title("Show Document")]
[Description("Shows a document text on screen with a title and an image (custom action)")]
[Category("UI/Show Document")]
[Image(typeof(IconTextArea), ColorTheme.Type.Yellow)]

[Keywords("Text", "Message", "Document")]

public class ActionPlayerDocument : Instruction
{
    [SerializeField] private PlayerDocSO document;
    public bool firstTimeCollected = false;

    public override string Title => $"Show Document";


    protected override Task Run(Args args)
    {
        if (firstTimeCollected == true)
        {
            PlayerHUDManager.Instance.firstTimeMemoCollected = true;
        }
        PlayerMessageManager.Instance.ShowDocument(document);
        return Task.CompletedTask;
    }
}
