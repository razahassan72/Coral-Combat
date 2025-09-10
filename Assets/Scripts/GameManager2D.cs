using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager2D : MonoBehaviour
{
    public static GameManager2D Instance { get; private set; }

    [Header("HUD UI")]
    [SerializeField] private TextMeshProUGUI hudTimerText;

    [Header("Game Over / Pause UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI currentTimeText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("Music UI (Pause Panel)")]
    [SerializeField] private GameObject musicOnButton;
    [SerializeField] private GameObject musicOffButton;

    [SerializeField] private FishController playerController;

    private float elapsedTime = 0f;
    private bool isRunning = false;
    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Time.timeScale = 1f;
    }
    private void Start()
    {
        elapsedTime = 0f;
        isRunning = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);  
    }

    private void Update()
    {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        if (hudTimerText != null)
            hudTimerText.text = FormatTime(elapsedTime);
    }
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        var spawner = FindFirstObjectByType<EnemySpawner2D>();
        if (spawner) spawner.StopSpawning();

        isRunning = false;

        if (playerController != null)
            playerController.enabled = false;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (currentTimeText != null)
            currentTimeText.text = "Time: " + FormatTime(elapsedTime);

        float bestTime = PlayerPrefs.GetFloat("HighScore", 0f);
        if (elapsedTime > bestTime)
        {
            bestTime = elapsedTime;
            PlayerPrefs.SetFloat("HighScore", bestTime);
            PlayerPrefs.Save();
        }

        if (highScoreText != null)
            highScoreText.text = "Best: " + FormatTime(bestTime);

        Time.timeScale = 0f;
    }
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void PauseGame()
    {
        if (isGameOver) return;

        isRunning = false;
        Time.timeScale = 0f;

        if (playerController != null)
            playerController.enabled = false;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (currentTimeText != null)
            currentTimeText.text = "Time: " + FormatTime(elapsedTime);

        float bestTime = PlayerPrefs.GetFloat("HighScore", 0f);
        if (highScoreText != null)
            highScoreText.text = "Best: " + FormatTime(bestTime);
    }

    public void ResumeGame()
    {
        Debug.Log("Resume button clicked");

        if (isGameOver) return;

        isRunning = true;
        Time.timeScale = 1f;

        if (playerController != null)
            playerController.enabled = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public bool PlayerCanMove()
    {
        return isRunning && !isGameOver;
    }

    public void GoToHome()
    {
        Debug.Log("Home button clicked");
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}