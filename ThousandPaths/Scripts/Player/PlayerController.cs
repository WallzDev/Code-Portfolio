using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using Wallz.Weapons;
using Unity.VisualScripting;
using System.Text;
using UnityEngine.SceneManagement;
using static Unity.VisualScripting.Member;
using Cinemachine;

public class PlayerController : MonoBehaviour
{

    [Header("Weapon Settings")]
    public WeaponGenerator weapon;
    public bool noneEquipped;

    [Header("Currency Settings")]
    public int riceOwned;
    public int coinsOwned;
    [SerializeField] public HealthController healthController;


    [Header("Horizontal Movement Settings")]
    [SerializeField] public float walkSpeed = 1;

    [Header("Vertical Movement Settings")]
    [SerializeField] public float jumpForce;
    [SerializeField] public float doubleJumpForce;
    private float jumpBufferCounter;
    [SerializeField] private float jumpBufferFrames;
    private float coyoteTimeCounter = 0f;
    [SerializeField] private float coyoteTime;
    private int airJumpCounter = 0;
    [SerializeField] public int maxAirJumps;
    private float jumpCooldown = 0.2f;
    private float lastJumpTime = 0f;

    [Header("Wall Jump Settings")]
    [SerializeField] private float wallSlideSpeed;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallJumpDuration;
    [SerializeField] private Vector2 wallJumpPower;
    float wallJumpDirection;
    public bool isWallSliding;
    bool isWallJumping;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    [SerializeField] GameObject dashEffect;
    [HideInInspector] public bool sDiving = false;

    [Header("GroundCheck Settings")]
    [SerializeField] private Transform groundChecker;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Attack Settings")]
    [SerializeField] float timeBetweenAttack, timeSinceAttack;
    [SerializeField] Transform SideAttackTransform, UpAttackTransform, DownAttackTransform;
    [SerializeField] Vector2 SideAttackArea, UpAttackArea, DownAttackArea;
    [SerializeField] LayerMask attackableLayer;
    [SerializeField] LayerMask groundHitLayer;
    [SerializeField] float damage;
    [SerializeField] GameObject slashEffect;
    public bool isVerticalAttacking = false;
    public bool canDownKick = true;

    bool restoreTime;
    float restoreTimeSpeed;

    [Header("Recoil Settings")]
    [SerializeField] int recoilXSteps = 5;
    [SerializeField] int recoilYSteps = 5;
    [SerializeField] public float recoilXSpeed = 100;
    [SerializeField] public float recoilYSpeed = 100;
    int stepsXRecoiled, stepsYRecoiled;
    [SerializeField] public CinemachineImpulseSource impulse;

    [Header("Health Settings")]
    public int health;
    public int maxHealth;
    public int maxTotalHealth = 10;
    public int godSeeds;
    [SerializeField] GameObject bloodSpurt;
    [SerializeField] float hitFlashSpeed;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallback;
    float healTimer;
    [SerializeField] float healDuration;
    private bool doHeal;
    private Coroutine inkTween;

    [Header("Spells Settings")]
    [SerializeField] public float inkSpellCost = .3f;
    [SerializeField] public float timeBetweenCast = .5f;
    float timeSinceCast;
    [SerializeField] public GameObject forwardSpell;


    [Header("Ink Settings")]
    [SerializeField] UnityEngine.UI.Image inkStorage;
    [SerializeField] float ink;
    [SerializeField] public float inkUseSpeed;
    [SerializeField] public float inkGain;
    public bool halfInk;

    [Header("Camera Settings")]
    [SerializeField] private float playerFallSpeedThreshold = -10;
    [SerializeField] public float lookTimer;
    [SerializeField] public float lookDuration = 3;
    [Space(5)]

    [Header("Sound Settings")]
    [SerializeField] AudioClip landingSound;
    [SerializeField] AudioClip[] jumpSounds;
    [SerializeField] AudioClip jumpTakeOffSound;
    [SerializeField] AudioClip airJumpSound;
    [SerializeField] AudioClip[] attackSounds;
    [SerializeField] AudioClip dashSound;
    [SerializeField] AudioClip[] hurtSounds;
    [SerializeField] public AudioClip kunaiSound;
    [SerializeField] AudioClip dirtSound;
    [SerializeField] public AudioClip riceCollectSound;
    [SerializeField] public AudioClip coinCollectSound;
    [SerializeField] public AudioClip diveSound;
    [SerializeField] public AudioClip strongLandingSound;
    [SerializeField] public AudioClip deathMarkStartSnd;
    [Space(5)]

    [HideInInspector] public PlayerStateList pState;
    [HideInInspector] public float xAxis, yAxis;
    private float gravity;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator anim;
    private bool canDash = true;
    public bool dashed;
    private SpriteRenderer sr;
    public AudioSource audsrc;

    public bool dropDownPlatform;

    private bool canFlash = true;

    bool cancelCooldown;

    //Control Bools
    public Vector2 MoveInput { get; private set; }
    public bool JumpJustPressed { get; private set; }
    public bool JumpBeingHeld { get; private set; }
    public bool JumpReleased { get; private set; }
    public bool AttackInput { get; private set; }
    public bool DashInput { get; private set; }
    public bool CastInput { get; private set; }

    public bool MenuToggleInput;
    public bool EquipmentToggleInput { get; private set; }
    public bool InventoryToggleInput { get; private set; }
    public bool MapInput { get; private set; }
    public bool HealInput { get; private set; }
    public bool HealReleased { get; private set; }
    public bool InteractInput { get; private set; }
    public bool CancelInput { get; private set; }

    //Input Actions
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction attackAction;
    private InputAction dashAction;
    private InputAction castAction;
    private InputAction healAction;
    private InputAction menuTAction;
    private InputAction cancelAction;
    private InputAction equipmentTAction;
    private InputAction inventoryTAction;
    private InputAction mapAction;

    public float stickDeadzoneY = .73f;
    public float stickDeadzoneX = .37f;


    //Sound Stuff
    private bool landingSoundPlayed;

    public static PlayerController Instance;

    //Unlockables
    [Header("Unlockables")]
    public bool hasMapItem;
    public bool unlockedWallJump;
    public bool unlockedDash;
    public bool unlockedDoubleJump;
    public bool magicEquipped;

    [Header("Player Particle Effects")]
    [SerializeField] public ParticleSystem wallSlideParticles;
    [SerializeField] public ParticleSystem jumpParticles;
    [SerializeField] public ParticleSystem dashParticles;

    //Death Mark
    public bool deathMarked = false;
    public bool markTicking = false;
    public bool markPulsing = false;
    public bool markIntensified = false;

    //Scenes and Map Data
    public HashSet<string> scenesVisited;
    public HashSet<string> scenesOnMap;

    //Permanence
    public HashSet<string> playedDialogueIds;
    public HashSet<string> worldPermanence;
    public HashSet<string> unlockedLogs;
    public HashSet<string> unlockedSeals;
    public bool learnedSeals;

    //NPC States
    [HideInInspector] public int hisaoState;


    private enum MapBools
    {
        MapTestCave,
        MapJungle,
        MapDarkJungle,
        MapVillage,
        MapGiantBattleground,
        MapSpiralCave,
        MapRyukken,
        MapBridge,
        MapCity,
        MapBoneForest,
        MapKingCastle
    }


    private void SetupInputActions()
    {
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        attackAction = playerInput.actions["Attack"];
        dashAction = playerInput.actions["Dash"];
        castAction = playerInput.actions["Cast"];
        healAction = playerInput.actions["Heal"];
        menuTAction = playerInput.actions["ToggleMenu"];
        cancelAction = playerInput.actions["Cancel"];
        equipmentTAction = playerInput.actions["ToggleEquipment"];
        inventoryTAction = playerInput.actions["ToggleInventory"];
        mapAction = playerInput.actions["Map"];
    }
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);

        playerInput = GetComponent<PlayerInput>();
        SetupInputActions();

        scenesVisited = new HashSet<string>();
        scenesOnMap = new HashSet<string>();
        playedDialogueIds = new HashSet<string>();
        worldPermanence = new HashSet<string>();
        unlockedLogs = new HashSet<string>();
        unlockedSeals = new HashSet<string>();
    }


    void Start()
    {

        audsrc = GetComponent<AudioSource>();
        pState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        SaveData.Instance.LoadTorii();
        SaveData.Instance.LoadPlayerData();

        if (halfInk)
        {
            UIManager.Instance.SwitchInk(UIManager.InkState.HalfInk);
        }
        else
        {
            UIManager.Instance.SwitchInk(UIManager.InkState.FullInk);
        }

        gravity = rb.gravityScale;

        inkStorage.fillAmount = Ink;

        StartCoroutine(ActivateMenuForLoading());
        ToggleCursorOff();
    }

    void Update()
    {

        if (GameManager.Instance.gameIsPaused) return;

        //Stops player from sliding during a cutscene
        if (pState.upgradeCutscene)
        {
            rb.velocity = new Vector3(0, 0, 0);
        }

        if (pState.cutscene || pState.upgradeCutscene || pState.cantControl) return;
        if (pState.alive)
        {
            if (Grounded())
            {
                ToggleInventory();
                ToggleEquipment();
                ToggleLogs();
            }
            GetInputs();

            if (pState.gameMenu) return;

            ToggleMap();
        }
        
        UpdateJumpVariables();
        UpdateCameraYDampingForPlayerFall();
        RestoreTimeScale();
        if (pState.dashing) return;
        if (pState.alive && !pState.sitting)
        {
            if(!isWallJumping && !pState.healing)
            {
                Flip();
                Move();
                Jump();
            }
            if(unlockedWallJump)
            {
                WallSlide();
                WallJump();
            }
            if(unlockedDash)
            {
                StartDash();
            }
            Attack();
            if (Grounded())
            {
                Heal();
            }
            CastSpell();
        }
        
        FlashWhileInvincible();

    }

    private void FixedUpdate()
    {
        if (pState.cutscene) return;
        if (pState.dashing) return;
        Recoil();
    }


    public void GetInputs()
    {
        
        MoveInput = moveAction.ReadValue<Vector2>();

        if (Mathf.Abs(MoveInput.x) < stickDeadzoneX) xAxis = 0;
        else xAxis = Mathf.Sign(MoveInput.x);

        if (Mathf.Abs(MoveInput.y) < stickDeadzoneY) yAxis = 0;
        else yAxis = Mathf.Sign(MoveInput.y);

        AttackInput = attackAction.WasPressedThisFrame();
        CastInput = castAction.WasPressedThisFrame();
        MapInput = mapAction.IsPressed();
        EquipmentToggleInput = equipmentTAction.WasPressedThisFrame();
        InventoryToggleInput = inventoryTAction.WasPressedThisFrame();
        DashInput = dashAction.WasPressedThisFrame();
        JumpJustPressed = jumpAction.WasPressedThisFrame();
        JumpBeingHeld = jumpAction.IsPressed();
        JumpReleased = jumpAction.WasReleasedThisFrame();
        HealInput = healAction.IsPressed();
        HealReleased = healAction.WasReleasedThisFrame();
        MenuToggleInput = menuTAction.WasPressedThisFrame();
        InteractInput = yAxis > 0;
        CancelInput = cancelAction.WasPressedThisFrame();

        if (pState.sitting && xAxis != 0 && pState.sat)
        {
            StartCoroutine(StandUpFromToriiCR());
        }

    }

    public void ToggleCursorOn()
    {
        if (playerInput.currentControlScheme == "Controller") return;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ToggleCursorOff()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void ToggleMap()
    {
        if (MapInput && hasMapItem)
        {
            UIManager.Instance.mapHandler.SetActive(true);
        }
        else
        {
            UIManager.Instance.mapHandler.SetActive(false);
        }
    }

    public void ToggleInventory()
    {
        if (InventoryToggleInput && (pState.gameMenu == false))
        {
            MusicManager.instance.ChangeMusicVol(true, .4f, 3);
            pState.gameMenu = true;
            UIManager.Instance.gameMenu.SetActive(true);
            GameMenuManager.instance.ChangeToIndex(1);
            rb.velocity = new Vector3(0, 0, 0);
            ToggleCursorOn();
        }
        
        if ((pState.gameMenu && pState.gameMenuInventory) == true && CancelInput && !cancelCooldown)
        {
            float cooldown = .5f;
            cancelCooldown = true;
            StartCoroutine(InventoryClose());
            StartCoroutine(ResetCancelCooldown(cooldown));
        }
    }

    public void ToggleEquipment()
    {
        float cooldown = .5f;
        if (EquipmentToggleInput && (pState.gameMenu == false))
        {
            MusicManager.instance.ChangeMusicVol(true, .4f, 3);
            pState.gameMenu = true;
            UIManager.Instance.gameMenu.SetActive(true);
            GameMenuManager.instance.ChangeToIndex(0);
            UIManager.Instance.CloseSubMenus();
            rb.velocity = new Vector3(0, 0, 0);
            ToggleCursorOn();
        }

        if ((pState.gameMenu && pState.gameMenuEquipment) == true && CancelInput && !cancelCooldown)
        {
            cancelCooldown = true;
            if (UIManager.Instance.subMenusActive)
            {
                foreach (var item in UIManager.Instance.subMenus)
                {
                    if(item.activeSelf)
                    {
                        item.GetComponent<Animator>().SetTrigger("Close");
                        item.SetActive(false);
                        UIManager.Instance.subMenusActive = false;

                    }
                }
                GameMenuManager.instance.equipmentSelectItem.GetComponent<Button>().Select();
                GameMenuManager.instance.buttonIndicators.SetActive(true);
                StartCoroutine(ResetCancelCooldown(cooldown));
            }
            else if(UIManager.Instance.subMenusActive == false)
            {
                StartCoroutine(EquipmentClose());
                StartCoroutine(ResetCancelCooldown(cooldown));
            }
        }

    }

    public void ToggleLogs()
    {
        if ((pState.gameMenu && pState.gameMenuLogs) == true && CancelInput && !cancelCooldown)
        {
            float cooldown = .5f;
            cancelCooldown = true;
            StartCoroutine(LogsClose());
            StartCoroutine(ResetCancelCooldown(cooldown));
        }
    }

    public IEnumerator EquipmentClose()
    {
        ToggleCursorOff();
        UIManager.Instance.equipment.GetComponent<Animator>().SetTrigger("Close");
        yield return new WaitForSeconds(0.32f);
        UIManager.Instance.gameMenu.SetActive(false);
        pState.gameMenu = false;
        MusicManager.instance.ChangeMusicVol(false, .4f, 2);
    }

    public IEnumerator InventoryClose()
    {
        ToggleCursorOff();
        UIManager.Instance.inventory.GetComponent<Animator>().SetTrigger("Close");
        yield return new WaitForSeconds(.2f); //.17
        UIManager.Instance.gameMenu.SetActive(false);
        pState.gameMenu = false;
        MusicManager.instance.ChangeMusicVol(false, .4f, 2);
    }

    public IEnumerator LogsClose()
    {
        ToggleCursorOff();
        UIManager.Instance.logs.GetComponent<Animator>().SetTrigger("Close");
        yield return new WaitForSeconds(.2f); //.17
        UIManager.Instance.gameMenu.SetActive(false);
        pState.gameMenu = false;
        MusicManager.instance.ChangeMusicVol(false, .4f, 2);
    }

    public void SitAtTorii()
    {
        if (Grounded())
        {
            StartCoroutine(SitAtToriiCR());
        }
    }

    public IEnumerator SitAtToriiCR()
    {
        if (Grounded())
        {
            rb.velocity = Vector2.zero;
            pState.sitting = true;
            anim.SetBool("Sitting", true);
            yield return new WaitForSeconds(1.9333f);
            pState.sat = true;
            UpdateMap();
            RestoreFullHealth();
            SaveData.Instance.SaveTorii();
            SaveData.Instance.SavePlayerData();
        }

    }


    public IEnumerator StandUpFromToriiCR()
    {
        anim.Play("PlayerUnsitTorii");
        yield return new WaitForSeconds(.567f);
        pState.sitting = false;
        pState.sat = false;
        anim.SetBool("Sitting", false);
    }

    public void Flip()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2 (-1, transform.localScale.y);
            pState.lookingRight = false;
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
            pState.lookingRight = true;
        }
    }

    void FlipCharacterOposite()
    {
        if (transform.localScale.x > 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
            pState.lookingRight = false;
        }

        else if (transform.localScale.x < 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
            pState.lookingRight = true;
        }
    }

    void StartDash()
    {
        if (BlessingMenuManager.Instance.equippedBlessings.Contains(18) && PlayerController.Instance.yAxis < -0.6 && !Grounded()) return;


        if (DashInput && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }
        
        if (Grounded())
        {
            dashed = false;
        }

    }

    public IEnumerator WalkIntoNewScene(Vector2 _exitDir, float _delay)
    {
        //For exiting up
        if(_exitDir.y > 0)
        {
            rb.velocity = jumpForce * _exitDir;
        }

        //For horizontal exiting
        if(_exitDir.x != 0)
        {
            xAxis = _exitDir.x > 0 ? 1 : -1;
            Move();
        }

        Flip();
        yield return new WaitForSeconds(_delay);
        pState.cutscene = false;
    }

    IEnumerator Dash()
    {
        dashParticles.Play();
        if (!isWallSliding)
        {
            pState.sat = false;
            canDash = false;
            pState.dashing = true;
            anim.SetTrigger("Dashing");
            audsrc.pitch = UnityEngine.Random.Range(1f, 1.2f);
            audsrc.PlayOneShot(dashSound);
            rb.gravityScale = 0;
            int _dir = pState.lookingRight ? 1 : -1;
            rb.velocity = new Vector2(_dir * dashSpeed, 0);
            if (Grounded()) 
            { 
                Instantiate(dashEffect, transform);
                jumpParticles.Emit(10);
            }
            yield return new WaitForSeconds(dashTime);
            rb.gravityScale = gravity;
            pState.dashing = false;
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
            dashParticles.Stop();
        }
        else if (isWallSliding)
        {
            FlipCharacterOposite();
            pState.sat = false;
            canDash = false;
            pState.dashing = true;
            anim.SetBool("WallSliding",false);
            anim.Play("PlayerDash");
            audsrc.pitch = UnityEngine.Random.Range(1f, 1.2f);
            audsrc.PlayOneShot(dashSound);
            rb.gravityScale = 0;
            int _dir = pState.lookingRight ? 1 : -1;
            rb.velocity = new Vector2(_dir * dashSpeed, 0);
            yield return new WaitForSeconds(dashTime);
            rb.gravityScale = gravity;
            pState.dashing = false;
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
            dashParticles.Stop();
        }
        
    }

    IEnumerator Death()
    {
        MusicManager.instance.ChangeMusicVol(true, 0, 2);
        rb.velocity = Vector2.zero;
        pState.alive = false;
        Time.timeScale = 1f;
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        anim.SetTrigger("TakeDamage");
        anim.SetTrigger("Death");
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(1f);
        StartCoroutine(UIManager.Instance.DeathScreenActive());
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(2f);

        Instantiate(GameManager.Instance.KatanaDropped, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(.05f);
        MusicManager.instance.audioSource.Stop();
        riceOwned = 0;
        coinsOwned = 0;
    }

    public void Respawned()
    {
        if (!pState.alive)
        {
            pState.alive = true;
            halfInk = true;
            UIManager.Instance.SwitchInk(UIManager.InkState.HalfInk);
            Ink = 0;
            Health = maxHealth;
            anim.Play("PlayerIdle");
        }
    }
    
    public void RestoreFullHealth()
    {
        if (pState.alive)
        {
            Health = maxHealth;
        }    
    }


    void Attack()
    {
        if (AttackInput && !pState.healing && !isWallSliding && yAxis == 0)
        {
            if (noneEquipped || isVerticalAttacking) return;
            weapon.weapon.CurrentInput = true;
            weapon.weapon.TryAttack();
        }
    }

    public float Ink
    {
        get { return ink; }
        set
        {
            // if stats change
            if (ink != value)
            {
                float oldInk = ink;

                if (!halfInk)
                {
                    ink = Mathf.Clamp(value,0 , 1);
                }
                else
                {
                    ink = Mathf.Clamp(value, 0, 0.5f);
                }

                if (inkTween != null)
                {
                    StopCoroutine(inkTween);
                }

                inkTween = StartCoroutine(FillInkStorage(oldInk, ink, .1f));

                inkStorage.color = EnoughInk ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 215);
            }
        }
    }

    public IEnumerator FillInkStorage(float from, float to, float duration)
    {
        float elapsed = 0;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            inkStorage.fillAmount = Mathf.Lerp(from, to, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        inkStorage.fillAmount = Ink;
    }

    public void RestoreInk()
    {
        halfInk = false;
        UIManager.Instance.SwitchInk(UIManager.InkState.FullInk);
    }

    public void RestoreMoney()
    {
        riceOwned += KatanaDropped.Instance.storedRice;
        coinsOwned += KatanaDropped.Instance.storedCoins;
        healthController.UpdateMoneyHUDForKatana();
    }

    public bool EnoughInk
    {
        get
        {
            return Ink >= inkUseSpeed * healDuration;
        }
    }

    void Heal()
    {
        if (HealInput && Health < maxHealth && EnoughInk && !pState.jumping && !pState.dashing)
        {
            doHeal = true;
        }
        else if (HealReleased)
        {
            doHeal = false;
        }

        if (doHeal && Health < maxHealth && !pState.jumping && !pState.dashing)
        {
            pState.healing = true;
            anim.SetBool("Healing", true);
            rb.velocity = new Vector3(0, 0, 0);

            healTimer += Time.deltaTime;
            if (healTimer >= healDuration)
            {
                Health++;
                healTimer = 0;
                doHeal = false;
            }

            Ink -= Time.deltaTime * inkUseSpeed;
        }
        else
        {
            doHeal = false;
            pState.healing = false;
            anim.SetBool("Healing", false);
            healTimer = 0;
        }
    }



    public void Hit(ref bool _recoilBool, Vector2 _recoilDir, float _recoilStrength,float _recoilStrengthUp ,Collider2D[] objectsToHit, float damage)
    {
        HashSet<GameObject> hitObjects = new HashSet<GameObject>();

        for (int i = 0; i < objectsToHit.Length; i++)
        {
            GameObject obj = objectsToHit[i].gameObject;

            // Prevent hitting the same object multiple times
            if (hitObjects.Contains(obj)) continue;
            hitObjects.Add(obj);

            // If Hitting an Enemy
            Enemy enemy = obj.GetComponent<Enemy>();
            if (enemy != null)
            {
                _recoilBool = true;
                enemy.EnemyHit(damage, _recoilDir, _recoilStrength, _recoilStrengthUp);

                if (obj.CompareTag("Enemy"))
                {
                    Ink += inkGain;
                }
            }
            //If hitting a Breakable Wall
            BreakableWall wall = obj.GetComponent<BreakableWall>();
            if (wall != null)
            {
                _recoilBool = true;
                wall.bWallHit();
            }
            // If Hitting a Breakable Object (Grass or background objects)
            BreakableObject breakable = obj.GetComponent<BreakableObject>();
            if (breakable != null)
            {
                if (breakable.recoil)
                {
                    _recoilBool = true;
                }
                breakable.OnObjectHit();
            }
        }
    }

    void FlashWhileInvincible()
    {
        if (pState.invincible)
        {
            if(Time.timeScale > 0.2 && canFlash)
            {
                StartCoroutine(Flash());
            }
        }
        else
        {
            sr.enabled = true;
        }
    }

    void RestoreTimeScale()
    {
        if(restoreTime)
        {
            if(Time.timeScale < 1)
            {
                Time.timeScale += Time.deltaTime * restoreTimeSpeed;
            }
            else
            {
                Time.timeScale = 1;
                restoreTime = false;
                MusicManager.instance.ChangeMusicVol(false, .2f, 2);
            }
        }
    }

    public void HitStopTime(float _newTimeScale, float _restoreSpeed, float _delay)
    {
        MusicManager.instance.ChangeMusicVol(true, .2f, 3);
        restoreTimeSpeed = _restoreSpeed;
        Time.timeScale = _newTimeScale;
        if (_delay > 0)
        {
            StopCoroutine(StartTimeAgain(_delay));
            StartCoroutine(StartTimeAgain(_delay));
        }
        else
        {
            restoreTime = true;
        }    
    }

    IEnumerator StartTimeAgain(float _delay)
    {
        restoreTime = true;
        yield return new WaitForSeconds(_delay);
    }

    void Recoil()
    {
        if (pState.recoilingX)
        {
            if(pState.lookingRight)
            {
                rb.velocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                rb.velocity = new Vector2(recoilXSpeed, 0);
            }
        }
        if (pState.recoilingY)
        {   
            rb.gravityScale = 0;
            if(yAxis < 0)
            {
                
                rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, - recoilYSpeed);
            }
            airJumpCounter = 0;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        //stop recoil
        if(pState.recoilingX && stepsXRecoiled < recoilXSteps)
        {
            stepsXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }

        if (pState.recoilingY && stepsYRecoiled < recoilYSteps)
        {
            stepsYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }

        if(Grounded())
        {
            StopRecoilY();
        }
    }

    void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }
    void StopRecoilY()
    {
        stepsYRecoiled = 0;
        pState.recoilingY = false;
    }

    public void SetVelocity(float velocity, Vector2 angle)
    {
        int direction = 1;
        if (pState.lookingRight)
        {
            direction = 1;
        }
        else if (!pState.lookingRight)
        {
            direction = -1;
        }

        angle.Normalize();
        rb.velocity += new Vector2 (angle.x * velocity * direction, angle.y * velocity);
    }

    public void TakeDamage(float _damage)
    {
        if (pState.alive)
        {
            PlayerInterrupted();

            if (hurtSounds.Length > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, hurtSounds.Length);
                audsrc.pitch = UnityEngine.Random.Range(.9f, 1.1f);
                audsrc.PlayOneShot(hurtSounds[randomIndex]);
            }
            Health -= Mathf.RoundToInt(_damage);
            anim.SetTrigger("TakeDamage");
            if (Health <= 0)
            {
                Health = 0;
                StartCoroutine(Death());
            }
            else
            {
                StartCoroutine(StopTakingDamage());
            }

        }

    }

    IEnumerator StopTakingDamage()
    {
        pState.invincible = true;
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        yield return new WaitForSeconds(1f);
        pState.invincible = false;
    }

    IEnumerator Flash()
    {
        sr.enabled = !sr.enabled;
        canFlash = false;
        yield return new WaitForSeconds(0.05f);
        canFlash = true;
    }

    public int Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);

                if(onHealthChangedCallback != null)
                {
                    onHealthChangedCallback.Invoke();
                }
            }
        }
    }

    public void Move()
    {
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
        anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
        pState.sat = false;
    }

    void UpdateCameraYDampingForPlayerFall()
    {
        //if falling past certain threshold
        if (rb.velocity.y < playerFallSpeedThreshold && !CameraManager.Instance.isLerpingYDamping 
           && !CameraManager.Instance.hasLerpedYDamping)
        {
            StartCoroutine(CameraManager.Instance.LerpYDamping(true));
        }
        // if standing still or moving up
        if (rb.velocity.y >= 0 && !CameraManager.Instance.isLerpingYDamping && CameraManager.Instance.hasLerpedYDamping)
        {
            //reset camera function
            CameraManager.Instance.hasLerpedYDamping = false;
            StartCoroutine(CameraManager.Instance.LerpYDamping(false));
        }
    }

    private bool Walled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    public void WallSlide()
    {
        if (Walled() && !Grounded() && (xAxis != 0 || isWallSliding) && rb.velocity.y <= 0)
        {

            anim.SetBool("WallSliding", true);
            anim.Play("PlayerWallSlide");
            if (wallSlideParticles.isStopped)
            {
                wallSlideParticles.Play();
            }
            isWallSliding = true;
            dashed = false;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));


        }
        else if (Walled() && Grounded() && xAxis == 0)
        {
            anim.SetBool("WallSliding", false);
            wallSlideParticles.Stop();
            if (isWallSliding)
            {
                FlipCharacterOposite();
                isWallSliding = false;
            }
        }
        else
        {
            anim.SetBool("WallSliding", false);
            wallSlideParticles.Stop();
            isWallSliding = false;
        }
    }

    void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = !pState.lookingRight ? 1 : -1;

            CancelInvoke(nameof(StopWallJumping));
        }
        if (JumpJustPressed && isWallSliding)
        {
            isWallJumping = true;
            if (jumpParticles.isStopped)
            {
                jumpParticles.Play();
            }
            rb.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);

            dashed = false;
            airJumpCounter = 0;

            if (pState.lookingRight && transform.eulerAngles.y == 0 || (!pState.lookingRight && transform.eulerAngles.y != 0))
            {
                pState.lookingRight = !pState.lookingRight;
                int _yRotation = pState.lookingRight ? 0 : 360;

                transform.eulerAngles = new Vector2(transform.eulerAngles.x, _yRotation);
            }

            FlipCharacterOposite();
            Invoke(nameof(StopWallJumping), wallJumpDuration);
        }
    }

    void StopWallJumping()
    {
        jumpParticles.Stop();
        isWallJumping = false;
    }

    void CastSpell()
    {
        if (CastInput && timeSinceCast >= timeBetweenCast && ink >= inkSpellCost && !isWallSliding)
        {
            pState.casting = true;
            timeSinceCast = 0;
            StartCoroutine(CastCoroutine());
        }
        else
        {
            timeSinceCast += Time.deltaTime;
        }
    }

    IEnumerator CastCoroutine()
    {

        //side cast
        if ((xAxis == 0 || xAxis != 0||  (yAxis == 0 && Grounded())) && magicEquipped)
        {
            rb.velocity = new Vector3 (0, 0, 0);
            audsrc.pitch = UnityEngine.Random.Range(.9f, 1.1f);
            audsrc.PlayOneShot(kunaiSound);

            if (forwardSpell.name != "Galvanic Whip")
            {
                anim.SetBool("Throwing", true);
            }
            yield return new WaitForSeconds(0.10f);

            GameObject _spell = Instantiate(forwardSpell, transform.position, Quaternion.identity);

            //flip spell
            if (pState.lookingRight)
            {
                _spell.transform.eulerAngles = Vector3.zero;
            }
            else
            {
                _spell.transform.eulerAngles = new Vector2(_spell.transform.eulerAngles.x, 180);
            }
            Ink -= inkSpellCost;
            yield return new WaitForSeconds(0.10f);
            anim.SetBool("Throwing", false);
            pState.casting = false;
        }

        //up cast
        else if (yAxis > 0)
        {
            Debug.Log("UpCast");
            Ink -= inkSpellCost;
            yield return new WaitForSeconds(0.10f);
            pState.casting = false;
        }

        //down cast
        else if (yAxis < 0)
        {
            Debug.Log("DownCast");
            Ink -= inkSpellCost;
            yield return new WaitForSeconds(0.13f);
            pState.casting = false;
        }
    }


    public bool Grounded()
    {
        if (Physics2D.Raycast(groundChecker.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundChecker.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundChecker.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Jump()
    {
        if (Time.time - lastJumpTime > jumpCooldown)
        {
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !pState.jumping)
            {
                anim.SetBool("Jumping", true);
                // Play Random Sound
                if (jumpSounds.Length > 0)
                {
                    audsrc.pitch = UnityEngine.Random.Range(.9f, 1.1f);
                    int randomIndex = UnityEngine.Random.Range(0, jumpSounds.Length);
                    audsrc.PlayOneShot(jumpSounds[randomIndex]);
                }
                audsrc.PlayOneShot(jumpTakeOffSound);
                if(jumpParticles)
                {
                    jumpParticles.Emit(10);
                }
                rb.velocity = new Vector3(rb.velocity.x, jumpForce);
                pState.jumping = true;
                lastJumpTime = Time.time; // Update last jump time
            }

            if (!Grounded() && airJumpCounter < maxAirJumps && JumpJustPressed && unlockedDoubleJump && !isWallSliding)
            {
                audsrc.PlayOneShot(airJumpSound);
                pState.jumping = true;
                airJumpCounter++;
                rb.velocity = new Vector3(rb.velocity.x, doubleJumpForce);
                lastJumpTime = Time.time; // Update last jump time
            }
        }

        if (JumpReleased)
        {
            pState.jumping = false;
            anim.SetBool("Jumping", false);
            anim.SetBool("Falling", true);
            if (rb.velocity.y > 3)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
        }

        anim.SetBool("Jumping", !Grounded());
        anim.SetBool("Falling", !Grounded());
    }

    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            if (!landingSoundPlayed)
            {
                audsrc.pitch = UnityEngine.Random.Range(.9f, 1.1f);
                audsrc.PlayOneShot(landingSound);
                landingSoundPlayed = true;
            }
            anim.SetBool("Jumping", false);
            anim.SetBool("Falling", false);
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            landingSoundPlayed = false;
        }

        if (JumpJustPressed)
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter = jumpBufferCounter - Time.deltaTime * 10;
        }
    }

    public void UpdateMap()
    {
        foreach (string item in scenesVisited)
        {
            if (!scenesOnMap.Contains(item) && SaveData.Instance.playerHasMapItem)
            {
                scenesOnMap.Add(item);
                UIManager.Instance.ShowMapIcon();
            }
        }
    }

    public void SpriteEmpty()
    {
        anim.Play("Empty");
    }
    public void ReturnToIdle()
    {
        anim.SetBool("Walking", false);
        anim.SetBool("Jumping", false);
        anim.Play("PlayerIdle");
    }

    public IEnumerator ResetCancelCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        cancelCooldown = false;
    }
    
    public IEnumerator ActivateMenuForLoading()
    {
        yield return new WaitForSeconds(.05f);
        UIManager.Instance.gameMenu.SetActive(true);
        UIManager.Instance.gameMenu.SetActive(false);
        UIManager.Instance.logs.SetActive(true);
        UIManager.Instance.logs.SetActive(false);

        ToggleCursorOff();
    }

    public void ChangeToFalling()
    {
        if(!Grounded())
        {
            anim.SetBool("Falling", true);
        }
        else
        {
            anim.SetBool("Falling", false);
        }

    }

    public void CollectedMoney(string which)
    {
        if (which == "Rice")
        {
            healthController.riceCollected = true;
        }
        else if (which == "Coins")
        {
            healthController.coinsCollected = true;
        }
        healthController.UpdateMoneyHUD();
    }


    public void PlayerInterrupted()
    {
        UIManager.Instance.inventory.SetActive(false);
        UIManager.Instance.gameMenu.SetActive(false);
        UIManager.Instance.logs.SetActive(false);
        CutsceneSystem.instance.EndDialogue();
        foreach (var subMenu in UIManager.Instance.subMenus)
        {
            subMenu.gameObject.SetActive(false);
        }
        pState.gameMenu = false;
        UIManager.Instance.subMenusActive = false;
        anim.SetBool("Sitting", false);
        pState.sitting = false;
    }
}
