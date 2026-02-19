using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using Unity.VisualScripting;
using UnityEngine;

public class NewPickUp : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] float moveForce = 50f; // Que tan fuerte el objeto es jalado hacia el punto

    [Space(10)]
    [SerializeField] float holdDistance = 3f; // Que tan enfrente del jugador estara agarrado el objeto
    [SerializeField] float maxGrabDistance = 3f; // Que cerca tienes que estar del objeto para agarrarlo
    [SerializeField] float holdHeight = 0;

    [Space(10)]
    [SerializeField] float dragWhileHeld = 10f; // El drag del objeto al recogerlo
    [SerializeField] float angularDragWhileHeld = 10f; // El drag angular del objeto al recogerlo
    [Space(5)]
    [SerializeField] float physicsMatDynamicWhileHeld = 0f; // Valor al Cargar
    [SerializeField] float physicsMatDynamic = 0f; // Valor Original
    [SerializeField] float physicsMatStaticWhileHeld = 0f; // Valor al Cargar
    [SerializeField] float physicsMatStatic = 0f; // Valor Original
    [SerializeField] private Actions carryActions;
    [SerializeField] private Actions dropActions;


    [HideInInspector] public Rigidbody rb;
    private Collider col;
    private PhysicsMaterial pm;
    private GameObject holdPoint;
    public bool isHolding = false;
    public bool canSnap = true;

    private float originalDrag;
    private float originalAngularDrag;

    [Space(10)]
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] string heldLayerName = "holdLayer";

    [Space(5)]
    [SerializeField] private float dropForce = -.05f;

    private int originalLayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        pm = col.material;

        originalDrag = rb.linearDamping;
        originalAngularDrag = rb.angularDamping;
    }

    private void Update()
    {
        if (isHolding)
        {
            Hold();

            // Auto tirar si el objeto esta muy lejos del jugador
            float distanceFromPoint = Vector3.Distance(transform.position, holdPoint.transform.position);
            float dropDistance = maxGrabDistance;

            if (distanceFromPoint > dropDistance)
            {
                Drop();
                return;
            }
        }

        if (rb.isKinematic)
        {
            return;
        }
        else if (!rb.isKinematic)
        {
            if (!isHolding && rb.IsSleeping())
            {
                rb.isKinematic = true;
            }
        }
    }

    private void OnMouseDown()
    {
        if (isHolding && PlayerHUDManager.Instance.isHoldingPickUpObject)
        {
            Drop();
            return;
        }
        else if (!isHolding && !PlayerHUDManager.Instance.isHoldingPickUpObject)
        {
            Camera cam = Camera.main;
            float distance = Vector3.Distance(transform.position, cam.transform.position);

            if (distance <= maxGrabDistance)
            {
                Grab();
            }
        }

    }

    private void Hold()
    {
        if (!holdPoint) return;

        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 cameraPos = Camera.main.transform.position;
        Vector3 targetPos = cameraPos + camForward * holdDistance;

        targetPos.y = player.position.y + holdHeight;

        Vector3 moveDir = targetPos - transform.position;
        rb.linearVelocity = moveDir * moveForce * Time.fixedDeltaTime;
    }

    private void Grab()
    {
        if (isHolding) return;

        PlayerHUDManager.Instance.isHoldingPickUpObject = true;
        isHolding = true;
        canSnap = true;
        rb.isKinematic = false;
        rb.useGravity = false;
        rb.freezeRotation = true;
        rb.linearDamping = dragWhileHeld;
        rb.angularDamping = angularDragWhileHeld;
        pm.dynamicFriction = physicsMatDynamicWhileHeld;
        pm.staticFriction = physicsMatStaticWhileHeld;

        originalLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer(heldLayerName);


        holdPoint = PlayerHUDManager.Instance.holdPoint;
        holdPoint.transform.position = Camera.main.transform.position + Camera.main.transform.forward * holdDistance;
        holdPoint.transform.SetParent(Camera.main.transform);

        
        carryActions.Invoke();
    }

    public void Drop()
    {
        if (!isHolding) return;

        PlayerHUDManager.Instance.isHoldingPickUpObject = false;
        isHolding = false;
        rb.useGravity = true;
        rb.freezeRotation = false;

        gameObject.layer = originalLayer;

        rb.linearDamping = originalDrag;
        rb.angularDamping = originalAngularDrag;
        pm.dynamicFriction = physicsMatDynamic;
        pm.staticFriction = physicsMatStatic;

        rb.linearVelocity = new Vector3(0, dropForce, 0);
        dropActions.Invoke();
    }

}
