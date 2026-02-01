using UnityEngine;
using System.Collections;

public class AudioGameController : MonoBehaviour
{
    [Header("音频名称配置（对应AudioManager的Sound名）")]
    public string musicIntroName = "MusicIntro";  // 无节拍开头音频名
    public string musicLoopName = "MusicLoop";    // 循环节拍音频名
    public string successAudioName = "Success";   // 成功音频名
    public string failAudioName = "Fail";         // 失败音频名

    [Header("核心引用")]
    public GameManager gameManager;
    public RhythmManager rhythmManager;

    // 内部状态
    private bool isPlayingIntro = false;          // 是否在播放Intro
    private float loopClipLength = 0f;            // Loop音频时长
    private int loopCycleCount = 0;               // Loop循环次数
    private AudioManager.Sound loopSound;         // 缓存Loop音频

    void Start()
    {
        // 校验引用
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            Debug.LogWarning("未赋值GameManager，自动查找");
        }
        if (rhythmManager == null)
        {
            rhythmManager = FindObjectOfType<RhythmManager>();
            Debug.LogWarning("未赋值RhythmManager，自动查找");
        }

        // 初始禁用音游逻辑
        if (rhythmManager != null)
        {
            rhythmManager.PauseRhythmGame();
        }
    }

    void Update()
    {
        // 根据游戏状态控制音频
        switch (gameManager.currentState)
        {
            case GameState.Ready:
                // Ready阶段：静音，禁用音游
                StopAllAudio();
                if (rhythmManager != null)
                {
                    rhythmManager.PauseRhythmGame();
                }
                break;

            case GameState.Playing:
                // Playing阶段：控制Intro→Loop播放
                HandlePlayingAudio();
                break;

            case GameState.GameOver:
                // 失败：停止音乐，播放失败音频，禁用音游
                StopAllAudio();
                AudioManager.Instance.PlayOneShot(failAudioName);
                if (rhythmManager != null)
                {
                    rhythmManager.PauseRhythmGame();
                }
                break;

            case GameState.Success:
                // 成功：停止音乐，播放成功音频，禁用音游
                StopAllAudio();
                AudioManager.Instance.PlayOneShot(successAudioName);
                if (rhythmManager != null)
                {
                    rhythmManager.PauseRhythmGame();
                }
                break;

            case GameState.Paused:
                // 暂停：暂停所有音频
                PauseAllAudio();
                break;
        }
    }

    #region 核心音频控制逻辑
    /// <summary>
    /// 处理Playing状态的音频播放（Intro→Loop）
    /// </summary>
    private void HandlePlayingAudio()
    {
        // 还没开始播放Intro → 启动Intro播放
        if (!isPlayingIntro && !IsAudioPlaying(musicIntroName) && !IsAudioPlaying(musicLoopName))
        {
            StartCoroutine(PlayIntroThenLoop());
            return;
        }

        // 监控Loop循环，同步音游节拍
        if (!isPlayingIntro && loopSound != null && loopSound.source.isPlaying)
        {
            CheckLoopCycle();
        }
    }

    /// <summary>
    /// 播放Intro后自动切Loop
    /// </summary>
    private IEnumerator PlayIntroThenLoop()
    {
        isPlayingIntro = true;
        
        // 1. 播放Intro，禁用音游逻辑
        AudioManager.Instance.Play(musicIntroName);
        if (rhythmManager != null)
        {
            rhythmManager.PauseRhythmGame();
        }

        // 等待Intro播放完毕
        AudioManager.Sound introSound = AudioManager.Instance.GetSound(musicIntroName);
        if (introSound != null && introSound.clip != null)
        {
            yield return new WaitForSeconds(introSound.clip.length);
        }
        else
        {
            Debug.LogWarning("Intro音频未配置，直接播放Loop");
        }

        // 2. 停止Intro，启动Loop
        AudioManager.Instance.Stop(musicIntroName);
        isPlayingIntro = false;

        // 播放Loop并设置循环
        AudioManager.Instance.Play(musicLoopName);
        loopSound = AudioManager.Instance.GetSound(musicLoopName);
        if (loopSound != null)
        {
            loopSound.source.loop = true;
            loopClipLength = loopSound.clip.length;
        }

        // 3. 恢复音游逻辑
        if (rhythmManager != null)
        {
            rhythmManager.ResumeRhythmGame();
        }
    }

    /// <summary>
    /// 检测Loop循环，重置音游节拍点
    /// </summary>
    private void CheckLoopCycle()
    {
        if (loopSound == null || loopClipLength <= 0) return;

        // 检测Loop是否重新开始播放
        if (loopSound.source.time < 0.1f && loopCycleCount < int.MaxValue)
        {
            loopCycleCount++;
            // 重置音游节拍索引（核心！让CSV打击点循环）
            ResetRhythmIndex();
            Debug.Log($"Loop循环{loopCycleCount}次，音游节拍已重置");
        }
    }
    #endregion

    #region 工具方法
    /// <summary>
    /// 重置音游节拍索引（通过反射，不改动原有代码）
    /// </summary>
    private void ResetRhythmIndex()
    {
        if (rhythmManager == null) return;

        // 反射修改rhythmManager的index变量（避免改原有代码）
        var field = typeof(RhythmManager).GetField("index", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(rhythmManager, 0);
        }
    }

    /// <summary>
    /// 检查指定音频是否正在播放
    /// </summary>
    private bool IsAudioPlaying(string audioName)
    {
        var sound = AudioManager.Instance.GetSound(audioName);
        return sound != null && sound.source.isPlaying;
    }

    /// <summary>
    /// 停止所有游戏音频
    /// </summary>
    private void StopAllAudio()
    {
        AudioManager.Instance.Stop(musicIntroName);
        AudioManager.Instance.Stop(musicLoopName);
        isPlayingIntro = false;
        loopCycleCount = 0;
    }

    /// <summary>
    /// 暂停所有游戏音频
    /// </summary>
    private void PauseAllAudio()
    {
        var introSound = AudioManager.Instance.GetSound(musicIntroName);
        if (introSound != null && introSound.source.isPlaying)
        {
            introSound.source.Pause();
        }

        var loopSound = AudioManager.Instance.GetSound(musicLoopName);
        if (loopSound != null && loopSound.source.isPlaying)
        {
            loopSound.source.Pause();
        }
    }
    #endregion
}