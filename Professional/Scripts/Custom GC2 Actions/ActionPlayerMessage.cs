using GameCreator.Runtime.Common;
using GameCreator.Runtime.Inventory;
using GameCreator.Runtime.VisualScripting;
using System.Threading.Tasks;
using UnityEngine;

[Title("Pop up Message")]
[Description("Makes a custom message appear on Screen (custom action)")]
[Category("UI/Pop up Message")]
[Image(typeof(IconMessage), ColorTheme.Type.Purple)]

[Keywords("Pop up", "Message", "Notify")]

public class InstructionPlayerMessage : Instruction
{
    [SerializeField][TextArea] private string message;
    [SerializeField] private float lifeTime;

    public override string Title => $"Show Pop up Message";

    protected override async Task Run(Args args)
    {
        if (PlayerMessageManager.Instance == null || PlayerMessageManager.Instance.messageFinished == true) return;

        PlayerMessageManager.Instance.lifeTime = lifeTime;
        PlayerMessageManager.Instance.ShowMessage(message);
        
        await this.Until(() => PlayerMessageManager.Instance.messageFinished);
    }
}

