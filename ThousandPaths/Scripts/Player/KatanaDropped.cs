using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaDropped : MonoBehaviour, IInteractable
{
    private IInteractable target;
    private PlayerInteractionHandler interactHandler;
    [SerializeField] private GameObject hintCanvas;
    [SerializeField] private GameObject recoveredParticles;
    private Animator anim;

    public static KatanaDropped Instance;
    public bool recovered;
    public int storedRice;
    public int storedCoins;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject); // This destroys the last katana
        }

        Instance = this;
        target = this;
        anim = this.GetComponent<Animator>();

        if (!PlayerController.Instance.pState.alive || PlayerController.Instance.deathMarked)
        {
            StoreMoney();
        }
        else
        {
            SaveData.Instance.WriteDKatanaMoney();
        }
        SaveData.Instance.SaveDKatanaData();
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerInteractionHandler>(out interactHandler))
        {
            interactHandler.Register(target);
        }

        if (collision.CompareTag("Player") && PlayerController.Instance.Grounded())
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
            if (hintCanvas != null)
            {
                if (interactHandler.inRange.Contains(target))
                {
                    hintCanvas.SetActive(true);
                }
            }
        }
        else if(_collision.CompareTag("Player") && !PlayerController.Instance.Grounded())
        {
            if (hintCanvas != null)
            {
                hintCanvas.SetActive(false);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerInteractionHandler>(out interactHandler))
        {
            interactHandler.Unregister(target);
        }

        if (hintCanvas != null)
        {
            hintCanvas.SetActive(false);
        }
    }
    
    public void StoreMoney()
    {
        storedRice = PlayerController.Instance.riceOwned;
        storedCoins = PlayerController.Instance.coinsOwned;
    }

    public void OnInteract()
    {
        if (recovered == false)
        {
            recovered = true;
            recoveredParticles.SetActive(true);
            anim.SetTrigger("Recover");
            PlayerController.Instance.RestoreInk();
            PlayerController.Instance.RestoreMoney();
            SaveData.Instance.SavePlayerData();
            StartCoroutine(DestroyAfterSeconds());
        }
    }

    public IEnumerator DestroyAfterSeconds()
    {
        yield return new WaitForSeconds(3f); //.35f but it needs time to stop the particles
        Destroy(gameObject);
    }

    public void OnDestroy()
    {
        interactHandler?.Unregister(target);
    }

}
