using UnityEngine;

public enum GameState
{
    Ready,       // 初始准备（教程阶段）
    Playing,     // 游戏中
    GameOver,    // 失败
    Success,     // 成功
    Paused       // 暂停
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
    public PaperUI paperUI;
    public TutorialUI tutorialUI;
    public BadUI badUI;          // 直接赋值，不用Find
    public SuccessUI successUI;  // 直接赋值，不用Find

    private bool isPaused = false; // 暂停标记

    void Awake()
    {
        // 单例逻辑（保留）
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        tutorialUI.gameObject.SetActive(true);
        // 1. 暂停游戏
        Time.timeScale = 0f;
        FindObjectOfType<RhythmManager>().PauseRhythmGame();
        // 2. 显示教程UI
        tutorialUI.ShowTutorial();
    }

    void Update()
    {
        // 教程阶段：检测点击标记
        if (currentState == GameState.Ready)
        {
            if (tutorialUI.IsClicked())
            {
                // 点击后触发开始游戏
                StartGame();
                // 隐藏教程UI
                tutorialUI.HideTutorial();
                return; // 避免执行后续逻辑
            }
        }

        // 根据不同状态处理逻辑
        switch (currentState)
        {
            case GameState.Playing:
                HandlePlayingLogic();
                break;
            case GameState.Paused:
                HandlePausedLogic();
                break;
            // GameOver/Success状态不处理任何交互
            case GameState.GameOver:
            case GameState.Success:
                break;
        }
    }

    // =====================
    // 状态逻辑拆分（更清晰）
    // =====================

    /// <summary>
    /// 游戏中逻辑（暂停、失败/成功判断）
    /// </summary>
    void HandlePlayingLogic()
    {
        // 暂停/恢复逻辑
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        // 失败条件
        if (stability.IsDead())
        {
            GameOver();
        }

        // 成功条件（修正拼写：currentPaparCount → currentPaperCount）
        if (paperUI.currentPaparCount == 3 && !stability.IsDead())
        {
            Success();
        }
    }

    /// <summary>
    /// 暂停状态逻辑（仅处理恢复）
    /// </summary>
    void HandlePausedLogic()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // =====================
    // 核心状态控制方法
    // =====================

    /// <summary>
    /// 开始游戏（由TutorialUI点击后调用）
    /// </summary>
    public void StartGame()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1f;
        Debug.Log("Game Start!");
    }

    /// <summary>
    /// 暂停/恢复游戏
    /// </summary>
    public void TogglePause()
    {
        isPaused = !isPaused;
        currentState = isPaused ? GameState.Paused : GameState.Playing;
        Time.timeScale = isPaused ? 0f : 1f;
        Debug.Log(isPaused ? "Game Paused" : "Game Resumed");
    }

    /// <summary>
    /// 游戏失败
    /// </summary>
    public void GameOver()
    {
        if (currentState == GameState.GameOver) return;

        currentState = GameState.GameOver;
        Time.timeScale = 0f;
        Debug.Log("GAME OVER");

        // 直接调用赋值好的BadUI（避免Find出错）
        if (badUI != null)
        {
            badUI.ShowBadUI();
        }
        else
        {
            Debug.LogError("BadUI未在GameManager赋值！");
        }
    }

    /// <summary>
    /// 游戏成功
    /// </summary>
    public void Success()
    {
        if (currentState == GameState.Success) return;

        currentState = GameState.Success;
        Time.timeScale = 0f;
        Debug.Log("SUCCESS");

        // 直接调用赋值好的SuccessUI（避免Find出错）
        if (successUI != null)
        {
            successUI.ShowSuccessUI();
        }
        else
        {
            Debug.LogError("SuccessUI未在GameManager赋值！");
        }
    }
}