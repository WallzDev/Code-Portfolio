using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionHandler : MonoBehaviour
{
    public List<IInteractable> inRange = new List<IInteractable>();

    void Update()
    {
        if (PlayerController.Instance.pState.cutscene || PlayerController.Instance.pState.upgradeCutscene || PlayerController.Instance.pState.cantControl
            || PlayerController.Instance.pState.gameMenu || CutsceneSystem.instance.isInDialogue || GameManager.Instance.gameIsPaused || GameManager.Instance.gameOverScreen) return;

        if (inRange.Count > 0 && PlayerController.Instance.Grounded() && PlayerController.Instance.InteractInput)
        {
            inRange.RemoveAll(item => item == null);
            inRange[0].OnInteract();
        }
    }

    public void Register(IInteractable interactable)
    {
        inRange.Add(interactable);
    }

    public void Unregister(IInteractable interactable)
    {
        inRange.Remove(interactable);
    }
}
