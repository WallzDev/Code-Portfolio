using GameCreator.Runtime.Common;
using GameCreator.Runtime.Inventory;
using GameCreator.Runtime.VisualScripting;
using GameCreator.Runtime.Cameras;
using NinjutsuGames.StateMachine.Runtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class PlayerHUDManager : MonoBehaviour
{
    public static PlayerHUDManager Instance;

    public int targetFrameRate = 60;

    [Header("Pointer Variables")]
    [SerializeField] public GameObject pointer;
    [SerializeField] private GameObject escArrow;
    private Trigger escTrigger;

    [Header("Item Box Variables")]
    [SerializeField] public GameObject itemBox;
    [SerializeField] private Image itemBoxItem;
    [SerializeField] private Sprite defaultItemBoxSprite;
    [HideInInspector] public Bag playerBag;
    public List<RuntimeItem> equippableItems = new List<RuntimeItem>();
    public int currentEquppedIndex = -1;
    public int currentViewIndex = -1;
    public int cellIndexNumber;
    public int currentClickedCellIndex = -1;
    public bool itemEquipped = false;
    private int lastRemovedEquippedIndex = -1;

    [Header("Item Notification Variables")]
    [SerializeField] private GameObject itemNotifObj;
    [SerializeField] private TextMeshProUGUI itemTitleObj;
    [SerializeField] private Transform itemPivot;
    private GameObject currentItem;
    public bool finishedItemNotif;
    [SerializeField] public Light light1;
    [SerializeField] public Light light2;
    [SerializeField] public Light light3;
    [SerializeField] public Light light4;

    [Header("Other")]
    [SerializeField] public GameObject holdPoint;
    public bool isHoldingPickUpObject = false;
    [SerializeField] public GameObject contentGameObject;
    [SerializeField] public GameObject memoNotificationObj;
    public bool firstTimeMemoCollected = false;
    [SerializeField] public Actions firstTimeMemoActions;
    [SerializeField] public Actions subsequentMemoActions;


    private void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        playerBag = gameObject.GetComponent<Bag>();
        defaultItemBoxSprite = itemBoxItem.sprite;
    }


    public void TogglePointer(bool active)
    {
        if (active)
        {
            pointer.SetActive(true);
        }
        else
        {
            pointer.SetActive(false);
        }
    }

    public void ToggleEscArrow(bool active, Trigger newTrigger)
    {
        if (active)
        {
            escArrow.SetActive(true);
            escTrigger = newTrigger;
        }
        else
        {
            escArrow.SetActive(false);
            escTrigger = null;
        }
    }

    public async void RunEscArrowTrigger()
    {
        if (escTrigger != null)
        {
            await
            escTrigger.Execute();
        }
    }

    //FOR ITEM EQUIPS AND ETC
    public void ToggleItemBox(bool active)
    {
        itemBox.SetActive(active);
    }

    public void ChangeItemBoxIcon(Sprite newIcon)
    {
        if (itemBoxItem != null && newIcon != null)
        {
            itemBoxItem.sprite = newIcon;
        }
    }

    //Toda la funcion del item en pantalla es esto, no cambiar al menos de que quieras agregar al on enable/disable
    public void OnEnable()
    {
        playerBag.Equipment.EventEquip += OnEquip;
        playerBag.Equipment.EventUnequip += OnUnequip;
        playerBag.EventOpenUI += OnBagOpen;
        playerBag.EventChange += RebuildEquippableItems;

    }
    public void OnDisable()
    {
        playerBag.Equipment.EventEquip -= OnEquip;
        playerBag.Equipment.EventUnequip -= OnUnequip;
        playerBag.EventOpenUI -= OnBagOpen;
        playerBag.EventChange -= RebuildEquippableItems;
    }

    public void OnEquip(RuntimeItem item, int slotId)
    {
        RebuildEquippableItems();
    	UpdateItemBoxUI(item);

    	int equippedIndex = equippableItems.FindIndex(
        	i => i.RuntimeID == item.RuntimeID
    	);

    	if (equippedIndex != -1)
    	{
        	currentEquppedIndex = equippedIndex;
    	}
    	else
   	{
        	Debug.LogWarning("[HUD] Equipped item not found by RuntimeID");
    	}
    }

    public void OnUnequip(RuntimeItem item, int slotId)
    {
    	int removedIndex = equippableItems.FindIndex(i => i.RuntimeID == item.RuntimeID);

   	RebuildEquippableItems();
   	UpdateItemBoxUI(null);

    	if (removedIndex == currentEquppedIndex)
   	{
     	      lastRemovedEquippedIndex = removedIndex;
   	}
    }

    public void UpdateItemBoxUI(RuntimeItem item)
    {

        if (item != null)
        {
            Sprite icon = item.Item.Info.Sprite(Args.EMPTY);

            if (icon != null)
            {
                ChangeItemBoxIcon(icon);
                itemBoxItem.gameObject.SetActive(true);
                return;
            }
        }

        itemBoxItem.sprite = defaultItemBoxSprite;
    }

    public void OnBagOpen()
    {
        cellIndexNumber = 0;
        RebuildEquippableItems();
    }

    public void EquipItemAtIndex(int index)
    {
        if (index < 0 || index >= equippableItems.Count) return;

        Vector2Int? position = playerBag.Content.FindPosition(equippableItems[index].RuntimeID);
        if (position.HasValue)
        {
            playerBag.Equipment.Equip(playerBag.Content.GetRuntimeItem(equippableItems[index].RuntimeID));
        }

        currentEquppedIndex = index;
    }

    public void ViewItemAtIndex(int index)
    {
        if (index < 0 || index >= equippableItems.Count) return;

        Vector2Int? position = playerBag.Content.FindPosition(equippableItems[index].RuntimeID);
        if (position.HasValue)
        {
            playerBag.Content.Use(position.Value);
        }

        currentViewIndex = index;
    }

    // COSAS DE ITEM NOTIFICATION
    public void ShowItemNotif(Item item, float scale, bool resetPos)
    {
        finishedItemNotif = false;
        itemTitleObj.text = "Obtained: " + item.Info.Name(Args.EMPTY);

        itemNotifObj.SetActive(true);

        if (currentItem != null)
        {
            Destroy(currentItem);
        }

        currentItem = Instantiate(item.m_Prefab);
        currentItem.GetComponent<Trigger>().enabled = false;
        currentItem.transform.SetParent(itemPivot, false);
        if (resetPos)
        {
            currentItem.transform.localPosition = Vector3.zero;
            currentItem.transform.localRotation = Quaternion.identity;
            currentItem.transform.localScale = Vector3.one;
        }
        SetLayer(currentItem, LayerMask.NameToLayer("ItemViewer"));
        itemPivot.transform.localScale = new Vector3(scale, scale, scale);
        
        OpenInvisInventory();

    }

    public void CloseItemNotif()
    {
        if (currentItem != null)
        {
            Destroy(currentItem);
            currentItem = null;
        }

        itemNotifObj.SetActive(false);
        finishedItemNotif = true;
    }

    void SetLayer(GameObject obj, int newLayer)
    {
        if (obj == null) return;
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayer(child.gameObject, newLayer);
        }
    }

    // METODOS PARA CICLAR ITEMS EQUIPADOS Y DE VIEWER
    
    private bool HasEquippableItems()
    {
        foreach (RuntimeItem item in equippableItems)
        {
            if (item.Item.Parent != null && item.Item.Parent.name == "Equipables_Mano")
            {
                return true;
            }
        }
        return false;
    }

    public void CycleEquipRight()
    {
        if (equippableItems.Count == 0 || !HasEquippableItems()) return;

        int startIndex = currentEquppedIndex;

        // If the equipped item was removed, start from its old index - 1
        if (lastRemovedEquippedIndex != -1)
        {
            startIndex = lastRemovedEquippedIndex - 1;
            lastRemovedEquippedIndex = -1;
        }

        if (startIndex >= equippableItems.Count - 1)
        {
            currentEquppedIndex = -1;
        }
        else
        {
            currentEquppedIndex = startIndex;
        }

        int nextIndex = currentEquppedIndex + 1;

        bool isEquippable = equippableItems[nextIndex].Item.Parent != null &&
                        equippableItems[nextIndex].Item.Parent.name == "Equipables_Mano";

        if (isEquippable)
        {
            currentEquppedIndex = nextIndex;
            EquipItemAtIndex(currentEquppedIndex);
         }
        else
        {
            currentEquppedIndex = nextIndex;
            CycleEquipRight();
        }
    }

    public void CycleEquipLeft()
    {
        if (equippableItems.Count == 0 || !HasEquippableItems()) return;

        int startIndex = currentEquppedIndex;

        // If the equipped item was removed, start from its old index
        if (lastRemovedEquippedIndex != -1)
        {
            startIndex = lastRemovedEquippedIndex;
            lastRemovedEquippedIndex = -1;
        }

        if (startIndex <= 0)
        {
            currentEquppedIndex = equippableItems.Count;
        }
        else
        {
            currentEquppedIndex = startIndex;
        }

        int prevIndex = currentEquppedIndex - 1;

        bool isEquippable = equippableItems[prevIndex].Item.Parent != null &&
                        equippableItems[prevIndex].Item.Parent.name == "Equipables_Mano";

        if (isEquippable)
        {
            currentEquppedIndex = prevIndex;
            EquipItemAtIndex(currentEquppedIndex);
        }
        else
        {
            currentEquppedIndex = prevIndex;
            CycleEquipLeft();
        }
    }

    public void CycleViewRight()
    {
        if (currentViewIndex >= equippableItems.Count - 1) return;
        currentViewIndex ++;
        ViewItemAtIndex(currentViewIndex);
    }

    public void CycleViewLeft()
    {
        if (currentViewIndex <= 0) return;
        currentViewIndex--;
        ViewItemAtIndex(currentViewIndex);
    }
    

    public void OpenInvisInventory()
    {
        //StartCoroutine(OpenInvisibleInventoryCoroutine());
    }

    public IEnumerator OpenInvisibleInventoryCoroutine()
    {
        CanvasGroup canvasGroup = ItemViewerController.Instance.gameObject.GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;

        ItemViewerController.Instance.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        ItemViewerController.Instance.gameObject.SetActive(false);

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
    }


    public void CloseDocumentHUD()
    {
        if (firstTimeMemoCollected == true)
        {
            firstTimeMemoActions.Invoke();
            firstTimeMemoCollected = false;
        }
        else
        {
            subsequentMemoActions.Invoke();
            firstTimeMemoCollected = false;
        }
    }

    public void RebuildEquippableItems()
    {
        equippableItems.Clear();

        foreach (Bucket bucket in playerBag.Content.RuntimeItemsClone)
        {
            if (bucket == null || bucket.RuntimeItems == null) continue;

            foreach (RuntimeItem runtimeItem in bucket.RuntimeItems)
            {
                if (runtimeItem == null) continue;
                if (runtimeItem.Item?.Parent == null) continue;

                if (runtimeItem.Item.Parent.name == "Equipables_Mano")
                {
                    equippableItems.Add(runtimeItem);
                }
            }
        }

        if (equippableItems.Count == 0)
	{
   	 currentEquppedIndex = -1;
	}

    }


}
