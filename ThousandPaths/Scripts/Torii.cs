using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Torii : MonoBehaviour, IInteractable
{
    private IInteractable target;
    private PlayerInteractionHandler interactHandler;
    [SerializeField] public GameObject hintCanvas;

    public bool interacted;

    private void Awake()
    {
        target = this;
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.TryGetComponent<PlayerInteractionHandler>(out interactHandler))
        {
            interactHandler.Register(target);
        }

        if (_collision.CompareTag("Player") && PlayerController.Instance.Grounded())
        {
            if (hintCanvas != null)
            {
                if (interactHandler.inRange.Contains(target))
                {
                    hintCanvas.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player") && PlayerController.Instance.Grounded())
        {
            if (hintCanvas != null && !interacted)
            {
                if (interactHandler.inRange.Contains(target))
                {
                    hintCanvas.SetActive(true);
                }
            }
        }
        else if (_collision.CompareTag("Player") && !PlayerController.Instance.Grounded())
        {
            if (hintCanvas != null)
            {
                hintCanvas.SetActive(false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D _collision)
    {
        if (_collision.TryGetComponent<PlayerInteractionHandler>(out interactHandler))
        {
            interactHandler.Unregister(target);
        }

        if (_collision.CompareTag("Player"))
        {
            PlayerController.Instance.pState.sat = false;
            PlayerController.Instance.pState.sitting = false;
            interacted = false;
            PlayerController.Instance.anim.SetBool("Sitting", false);
        }
        if (hintCanvas != null)
        {
            hintCanvas.SetActive(false);
        }
    }

    public void ToriiActions()
    {
        interacted = true;

        if (hintCanvas != null)
        {
            hintCanvas.SetActive(false);
        }
        SaveData.Instance.toriiSceneName = SceneManager.GetActiveScene().name;
        SaveData.Instance.toriiPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);

        PlayerController.Instance.SitAtTorii();
    }

    public void OnInteract()
    {
        if (!interacted)
        {
            ToriiActions();
        }
    }

}
