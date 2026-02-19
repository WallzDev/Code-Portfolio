using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button pauseMenuButton;
    public bool gameIsPaused;

    private PlayerInput playerInput;
    private InputAction pauseAction;


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
        Time.timeScale = 1f;

        playerInput = FindFirstObjectByType<PlayerInput>();
        pauseAction = playerInput.actions["Pause"];
    }

    private void Update()
    {
        if (pauseAction.WasPressedThisFrame())
        {
            TogglePause();
        }

    }

    public void TogglePause()
    {
        if (PlayerController.Instance.pState.inTutorial || PlayerController.Instance.levelFinished) return;

        if (gameIsPaused)
        ResumeGame();
        else
        PauseGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        gameIsPaused = true;
        pauseMenu.SetActive(true);
        pauseMenuButton.Select();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        gameIsPaused = false;
        pauseMenu.SetActive(false);
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
