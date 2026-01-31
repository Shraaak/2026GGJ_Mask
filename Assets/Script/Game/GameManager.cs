using UnityEngine;

public enum GameState
{
    Ready,
    Playing,
    GameOver,
    Success
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public GameState currentState = GameState.Ready;

    [Header("Core Systems")]
    public PlayerMove player;
    public RhythmManager rhythm;
    public Stability stability;
    public ItemBarManager itemBar;
    private bool pause;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        StartGame();
    }

    void Update()
    {
        if (currentState != GameState.Playing)
            return;

        // Game Over 条件
        if (stability.IsDead())
        {
            GameOver();
        }
    }

    // =====================
    // 状态控制
    // =====================

    public void StartGame()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        if (currentState == GameState.GameOver)
            return;

        currentState = GameState.GameOver;
        Time.timeScale = 0f;

        Debug.Log("GAME OVER");
        // TODO: 弹 UI
    }

    public void Success()
    {
        currentState = GameState.Success;
        Time.timeScale = 0f;

        Debug.Log("SUCCESS");
        // TODO: 弹 UI
    }

    public void Pause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pause = !pause;
            Time.timeScale = pause ? 0f : 1f;
        }
    }
}