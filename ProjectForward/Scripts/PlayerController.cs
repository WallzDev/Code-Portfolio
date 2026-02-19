using System;
using System.Collections;
using System.Data.Common;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //RULE: the player ALWAYS moves forward
    //There is NO input that stops or reverses movement

    public static PlayerController Instance;

    [Header("Player Stats")]
    public bool advance = true;
    public bool levelFinished = false;
    public int maxHealth = 3;
    public int currentHealth;
    [HideInInspector] public enum SpeedTier { Base, Lv1, Lv2, Lv3 }
    public SpeedTier speedTier = SpeedTier.Base;

    [Header("Forward Movement Settings")]
    public float baseSpeed = 5f;
    public float lv1Speed = 7f;
    public float lv2Speed = 9f;
    public float lv3Speed = 12f;

    [Header("Jump Settings")]
    public float jumpForce = 8f;
    public Transform groundCheck;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    public LayerMask groundLayer;
    public bool isGrounded;
    
    [Header("Jump Assist")]
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    [Header("Slide Settings")]
    public float slideDuration = 1f;

    [Header("Attack Settings")]
    public GameObject attackHitbox;
    public float attackCooldown = 0.25f;
    private float lastAttackTime;

    [Header("Parry Settings")]
    public GameObject parryHitbox;
    public float parryAnimDuration = 0.12f;
    public float parrySpeedMultiplier = 0.5f;

    [Header("Pushback Settings")]
    public float pushbackForce = 6f;
    public float pushbackDuration = 0.2f;
    private Coroutine pushbackCoroutine;

    [Header("Hitstop Settings")]
    public float hitstopDuration = 0.07f;
    public float parryHitstopDuration = 0.1f;
    public bool isInHitstop = false;

    [Header("Screen Shake Settings")]
    public CinemachineImpulseSource wallHitImpulseSource;
    public CinemachineImpulseSource parryImpulseSource;

    [Header("Camera Settings")]
    public float baseFOV = 40f;
    public float lv1FOV = 41f;
    public float lv2FOV = 43f;
    public float lv3FOV = 45f;
    

    #region Input Actions
    private PlayerInput playerInput;
    private InputAction jumpAction;
    private InputAction attackAction;
    private InputAction parryAction;
    private InputAction crouchAction;
#endregion
    #region Input Bools
    public bool JumpJustPressed { get; private set; }
    public bool JumpBeingHeld { get; private set; }
    public bool JumpReleased { get; private set; }
    public bool AttackInput { get; private set; }
    public bool ParryInput { get; private set; }
    public bool CrouchInput { get; private set; }
    #endregion


    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public AudioSource audsrc;
    [HideInInspector] public PlayerStates pState;
    [HideInInspector] public AudioManager audioManager;


    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        playerInput = GetComponent<PlayerInput>();
        SetupInputActions();

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audsrc = GetComponent<AudioSource>();
        pState = GetComponent<PlayerStates>();
    }

    void Start()
    {
        audioManager = AudioManager.Instance;
        currentHealth = maxHealth;
        PlayerHUDManager.Instance.UpdateHealthUI();
        PlayerHUDManager.Instance.UpdateSpeedTierUI();
    }

    private void SetupInputActions()
    {
        jumpAction = playerInput.actions["Jump"];
        attackAction = playerInput.actions["Attack"];
        parryAction = playerInput.actions["Parry"];
        crouchAction = playerInput.actions["Crouch"];
    }
    public void GetInputs()
    {
        AttackInput = attackAction.WasPressedThisFrame();
        ParryInput = parryAction.WasPressedThisFrame();
        CrouchInput = crouchAction.IsPressed();
        JumpJustPressed = jumpAction.WasPressedThisFrame();
        JumpBeingHeld = jumpAction.IsPressed();
        JumpReleased = jumpAction.WasReleasedThisFrame();
    }

    void Update()
    {
        if (GameManager.Instance.gameIsPaused || levelFinished) return;

        CheckGround();
        GetInputs();
        HandleJump();
        Slide();
        Attack();
        Parry();
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.gameIsPaused || levelFinished) return;
        
        if (advance)
        {
            rb.linearVelocity = new Vector2(GetSpeedFromTier(), rb.linearVelocityY);
        }
    }

    private void Jump()
    {
        if (!pState.jumping && !pState.sliding && !pState.parrying)
        {
            pState.jumping = true;
            audioManager.PlayRandomJumpSound();
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
        }
    }

    private void HandleJump()
    {
        UpdateCoyoteTime();
        UpdateJumpBuffer();
        TryJump();
    }
    private void UpdateCoyoteTime()
    {
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }
    private void UpdateJumpBuffer()
    {
        if (JumpJustPressed)
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;
    }
    private void TryJump()
    {
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            Jump();
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }
    }

    private void Slide()
    {
        if (CrouchInput && isGrounded && !pState.sliding && !pState.attacking && !pState.parrying)
        {
            StartCoroutine(PerformSlide());
        }
    }

    private IEnumerator PerformSlide()
    {
        pState.sliding = true;
        anim.SetTrigger("Slide");
        audioManager.PlaySFX(audioManager.slideSound);
        yield return new WaitForSeconds(slideDuration);
        pState.sliding = false;
    }

    private void Attack()
    {
        if (AttackInput && Time.time >= lastAttackTime + attackCooldown && !pState.attacking && !pState.sliding && !pState.parrying)
        {
            pState.attacking = true;
            anim.SetTrigger("Attack");
            audioManager.PlayRandomAttackSound();
            
            lastAttackTime = Time.time;
        }
    }

    private void Parry()
    {
        if (ParryInput && !pState.parrying && !pState.attacking && !pState.sliding && !pState.beingPushedBack)
        {
            StartCoroutine(ParryRoutine());
        }
        
    }

    private IEnumerator ParryRoutine()
    {
        if (pState.beingPushedBack)
            yield break;

        pState.parrying = true;
        anim.SetTrigger("Parry");
        audioManager.PlaySFX(audioManager.parrySound);
        float originalSpeed = GetSpeedFromTier();
        advance = false;
        rb.linearVelocity = new Vector2(originalSpeed * parrySpeedMultiplier, rb.linearVelocityY);
        yield return new WaitForSeconds(parryAnimDuration);
        if (CanAdvance())
        {
            advance = true;
        }
        pState.parrying = false;
    }

    public void OnSuccessfulParry()
    {
        pState.parrying = false;
        anim.SetTrigger("ParrySuccess");
        audioManager.PlayRandomParriedSound();
        StartCoroutine(TemporaryInvincibility(0.05f));
        GainSpeedTier();
        StartCoroutine(Hitstop(parryHitstopDuration));
        StartCoroutine(SmallSpeedBoost());
        parryImpulseSource.GenerateImpulse();
    }

    public IEnumerator TemporaryInvincibility(float duration)
    {
        pState.invincible = true;
        yield return new WaitForSeconds(duration);
        pState.invincible = false;
    }

    public IEnumerator SmallSpeedBoost()
    {
        float originalSpeed = GetSpeedFromTier();
        advance = false;
        rb.linearVelocity = new Vector2(originalSpeed * 1.5f, rb.linearVelocityY);
        yield return new WaitForSeconds(0.2f);
        if (CanAdvance())
        {
            advance = true;
        }
    }

    public float GetSpeedFromTier()
    {
        switch (speedTier)
        {
            case SpeedTier.Lv1:
                return lv1Speed;
            case SpeedTier.Lv2:
                return lv2Speed;
            case SpeedTier.Lv3:
                return lv3Speed;
            default:
                return baseSpeed;
        }
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckY, groundLayer)
            || Physics2D.Raycast(groundCheck.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, groundLayer)
            || Physics2D.Raycast(groundCheck.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, groundLayer);

        if (isGrounded && rb.linearVelocityY <= 0)
        {
            pState.jumping = false;
        }

    }

    private void OnDrawGizmos()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;

        Vector3 center = groundCheck.position;
        Vector3 right = center + new Vector3(groundCheckX, 0, 0);
        Vector3 left = center + new Vector3(-groundCheckX, 0, 0);

        Gizmos.DrawLine(center, center + Vector3.down * groundCheckY);
        Gizmos.DrawLine(right, right + Vector3.down * groundCheckY);
        Gizmos.DrawLine(left, left + Vector3.down * groundCheckY);
    }

    public void GainSpeedTier()
    {
        if (speedTier < SpeedTier.Lv3)
        {
            speedTier++;
            PlayerHUDManager.Instance.UpdateSpeedTierUI();
            audioManager.PlaySFX(audioManager.speedTierGainSound);
        }
    }
    public void LoseSpeedTier()
    {
        if (speedTier > SpeedTier.Base)
        {
            if (pState.invincible) return;

            speedTier--;
            PlayerHUDManager.Instance.UpdateSpeedTierUI();
            audioManager.PlaySFX(audioManager.speedTierLoseSound);
        }
    }

    public void TakeDamage(int damage)
    {
        if (pState.invincible) return;
        
        currentHealth -= damage;
        PlayerHUDManager.Instance.UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //TODO
        advance = false;
    }


    public void ApplyPushback()
    {
        pState.beingPushedBack = true;
        anim.SetTrigger("Hit");
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(-pushbackForce, pushbackForce * 0.5f), ForceMode2D.Impulse);

        LoseSpeedTier();
        wallHitImpulseSource.GenerateImpulse();
        StartCoroutine(Hitstop(hitstopDuration));
        
        if (pushbackCoroutine != null)
        {
            StopCoroutine(pushbackCoroutine);
        }
        pushbackCoroutine = StartCoroutine(ContinueAfterPushback(pushbackDuration));
    }

    public IEnumerator ContinueAfterPushback(float delay)
    {
        advance = false;
        yield return new WaitForSeconds(delay);
        advance = true;
        pState.beingPushedBack = false;
    }
    IEnumerator Hitstop(float time)
    {
        if (isInHitstop)
            yield break;

        isInHitstop = true;

        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(time);

        if (!GameManager.Instance.gameIsPaused)
        {
            Time.timeScale = 1f;
        }
        isInHitstop = false;
    }

    public void ChangeCameraFov()
    {
        baseFOV = CameraManager.Instance.normalCameraFov;
        switch (speedTier)
        {
            case SpeedTier.Lv1:
                CameraManager.Instance.currentCamera.Lens.FieldOfView = lv1FOV;
                break;
            case SpeedTier.Lv2:
                CameraManager.Instance.currentCamera.Lens.FieldOfView = lv2FOV;
                break;
            case SpeedTier.Lv3:
                CameraManager.Instance.currentCamera.Lens.FieldOfView = lv3FOV;
                break;
            default:
                CameraManager.Instance.currentCamera.Lens.FieldOfView = baseFOV;
                break;
        }
    }

    public void EnableAttackHitbox()
    {
        attackHitbox.SetActive(true);
    }

    public void DisableAttackHitbox()
    {
        pState.attacking = false;
        attackHitbox.SetActive(false);
    }

    public void EnableParryHitbox()
    {
        parryHitbox.SetActive(true);
    }

    public void DisableParryHitbox()
    {
        pState.parrying = false;
        parryHitbox.SetActive(false);
    }

    public bool CanAdvance()
    {
        return !pState.beingPushedBack;
    }

    /// UTILITIES

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

}
