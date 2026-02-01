using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class RhythmManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource music;

    [Header("UI")]
    public RectTransform ring;

    [Header("Timing")]
    public float appearOffset = 1.3f;     // 提前出现时间
    public float perfectWindow = 0.16f;   // PERFECT 判定
    public float badWindow = 0.24f;       // BAD 判定
    private float speedMultiplier = 1f;       // 速度添加倍率

    [Header("Ring Size")]
    public float startSize =500f;
    public float targetSize = 263.1f;

    [Header("Miss UI 配置")]
    public RectTransform missUI; // 你的MissUI RectTransform
    public float missBlinkInterval = 0.5f; // MissUI闪烁间隔
    public float missBlinkDuration = 2f; // 闪烁持续时长（可调整，默认2秒）
    private Coroutine missBlinkCoroutine; // 闪烁协程标记
    private Coroutine missAutoCloseCoroutine; // 自动关闭协程标记

    [Header("Stability Impact")]
    public Stability stability;        // 稳定度组件引用
    public float perfectGain = 5f;     // PERFECT 增加量
    public float badPenalty = 5f;      // BAD 扣除量
    public float missPenalty = 10f;    // MISS 扣除量

    // ===== 新增：节拍系统暂停/恢复控制 =====
    private bool isRhythmPaused = false; // 节拍暂停标记
    private float pauseEndTime = 0f;     // 暂停结束时间

    // ===== 内部状态 =====
    List<float> beats = new List<float>();
    int index = 0;

    float currentBeat = -1f;
    bool active = false;

    void Start()
    {
        LoadCSV();

        if (music == null || ring == null)
        {
            Debug.LogError("Music 或 Ring 没有绑定！");
            return;
        }

        music.Play();
        ring.gameObject.SetActive(false);

        // ===== 新增：初始关闭MissUI =====
        if (missUI != null)
        {
            missUI.gameObject.SetActive(false);
        }
        // ===============================
    }

    void Update()
    {
        CheckResumeRhythm(); // 检查是否到时间恢复
        if (isRhythmPaused) return; // 暂停时直接退出Update，不执行任何节拍逻辑

        appearOffset *= speedMultiplier;

        float songTime = music.time;

        // 1️⃣ 生成新的节奏点
        if (!active && index < beats.Count &&
            songTime >= beats[index] - appearOffset)
        {
            SpawnRing(beats[index]);
            index++;
        }

        if (!active) return;

        // 2️⃣ 圆环缩放
        UpdateRing(songTime);

        // 3️⃣ 按键判定
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AudioManager.Instance.PlayOneShot("Hit1");
            Judge(songTime);
            return;
        }

        // 4️⃣ 漏按 MISS
        if (songTime > currentBeat + badWindow)
        {
            Miss_NoInput();
        }
    }

    // ===== 核心逻辑 =====

    void SpawnRing(float beatTime)
    {
        currentBeat = beatTime;
        active = true;

        ring.sizeDelta = Vector2.one * startSize;
        ring.gameObject.SetActive(true);
    }

    void UpdateRing(float songTime)
    {
        float t = Mathf.Clamp01(
            1f - (currentBeat - songTime) / appearOffset
        );

        float size = Mathf.Lerp(startSize, targetSize, t);
        ring.sizeDelta = Vector2.one * size;
    }

    void Judge(float songTime)
    {
        float diff = Mathf.Abs(songTime - currentBeat);
        bool isPerfect = false;

        if (diff <= perfectWindow)
        {
            Debug.Log("PERFECT");
            isPerfect = true;

            if (stability != null)
                stability.AddStability(perfectGain);
        }
        else if (diff <= badWindow)
        {
            Debug.Log("BAD");
            if (stability != null)
                stability.ReduceStability(badPenalty);
        }
        else
        {
            Debug.Log("MISS (Timing)");
            if (stability != null)
                stability.ReduceStability(missPenalty);
        }

        // ===== 新增：控制MissUI =====
        if (isPerfect)
        {
            DeactivateMissUI(); // Perfect时关闭MissUI
        }
        else
        {
            ActivateMissUI();   // 非Perfect时激活并闪烁
        }
        // ==========================

        EndBeat();
    }

    void Miss_NoInput()
    {
        Debug.Log("MISS (No Input)");

        if (stability != null)
            stability.ReduceStability(missPenalty);

        // ===== 新增：漏按也激活MissUI =====
        ActivateMissUI();
        // =================================

        EndBeat();
    }

    void EndBeat()
    {
        active = false;
        ring.gameObject.SetActive(false);
    }

    // ===== CSV =====

    void LoadCSV()
    {
        TextAsset csv = Resources.Load<TextAsset>("beat");

        if (csv == null)
        {
            Debug.LogError("Resources/beat.csv 没找到！");
            return;
        }

        string[] lines = csv.text.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            if (float.TryParse(lines[i], out float time))
                beats.Add(time);
        }
    }

    //用于控制音乐速度
    public void SetSpeedMultiplier(float value)
    {
        speedMultiplier = value;
    }

    /// <summary>
    /// 暂停节拍系统指定时长
    /// </summary>
    /// <param name="duration">暂停时长（秒）</param>
    public void PauseRhythm(float duration)
    {
        isRhythmPaused = true;
        pauseEndTime = Time.time + duration;
        
        // 暂停时直接隐藏圆环，标记为非激活
        if (ring != null)
        {
            ring.gameObject.SetActive(false);
        }
        active = false;
        
        Debug.Log($"节拍系统已暂停，将在 {duration} 秒后恢复");
    }

    /// <summary>
    /// 检查并恢复节拍系统（内部自动调用）
    /// </summary>
    private void CheckResumeRhythm()
    {
        if (isRhythmPaused && Time.time >= pauseEndTime)
        {
            isRhythmPaused = false;
            Debug.Log("节拍系统已恢复");
        }
    }

    // ===== 新增：Gizmos 可视化圆环尺寸 =====
    void OnDrawGizmos()
    {
        // 仅在Scene视图且ring不为空时绘制
        if (ring == null) return;

        // 1. 绘制 startSize（初始尺寸，红色）
        Gizmos.color = Color.red;
        DrawCircleGizmo(ring.position, startSize / 2); // 半径是尺寸的一半

        // 2. 绘制 targetSize（目标尺寸，绿色）
        Gizmos.color = Color.green;
        DrawCircleGizmo(ring.position, targetSize / 2);

        // 3. 标注尺寸数值（可选，更直观）
        UnityEditor.Handles.Label(ring.position + Vector3.up * (startSize / 2 + 20), 
            $"Start Size: {startSize}px");
        UnityEditor.Handles.Label(ring.position + Vector3.down * (targetSize / 2 + 20), 
            $"Target Size: {targetSize}px");
    }

    // 辅助方法：绘制圆形Gizmo
    void DrawCircleGizmo(Vector3 center, float radius)
    {
        int segments = 32; // 圆形分段数（越多越圆）
        Vector3 prevPoint = center + Vector3.right * radius;
        for (int i = 1; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * (i * 360f / segments);
            Vector3 currPoint = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            Gizmos.DrawLine(prevPoint, currPoint);
            prevPoint = currPoint;
        }
    }


    /// <summary>
    /// 激活MissUI并开始闪烁
    /// </summary>
    private void ActivateMissUI()
    {
        if (missUI == null) return;
        
        // 激活UI
        missUI.gameObject.SetActive(true);
        
        // 停止之前的自动关闭协程（避免重复）
        if (missAutoCloseCoroutine != null)
        {
            StopCoroutine(missAutoCloseCoroutine);
        }
        
        // 启动闪烁协程（避免重复启动）
        if (missBlinkCoroutine != null)
        {
            StopCoroutine(missBlinkCoroutine);
        }
        missBlinkCoroutine = StartCoroutine(BlinkMissUI());
        
        // 新增：启动自动关闭协程（闪烁指定时长后关闭）
        missAutoCloseCoroutine = StartCoroutine(AutoCloseMissUI());
    }

    /// <summary>
    /// 关闭MissUI并停止闪烁
    /// </summary>
    private void DeactivateMissUI()
    {
        if (missUI == null) return;
        
        // 停止闪烁协程
        if (missBlinkCoroutine != null)
        {
            StopCoroutine(missBlinkCoroutine);
            missBlinkCoroutine = null;
        }
        
        // 停止自动关闭协程
        if (missAutoCloseCoroutine != null)
        {
            StopCoroutine(missAutoCloseCoroutine);
            missAutoCloseCoroutine = null;
        }
        
        // 关闭UI
        missUI.gameObject.SetActive(false);
        // 恢复UI透明度（避免残留半透明状态）
        Image missImage = missUI.GetComponent<Image>();
        if (missImage != null)
        {
            missImage.color = new Color(1, 1, 1, 1);
        }
    }

    /// <summary>
    /// MissUI闪烁协程
    /// </summary>
    private IEnumerator BlinkMissUI()
    {
        Image missImage = missUI.GetComponentInChildren<Image>();
        if (missImage == null) yield break;
        
        while (missUI.gameObject.activeSelf)
        {
            // 淡出
            yield return FadeMissUI(missImage, 1f, 0f, missBlinkInterval / 2);
            // 淡入
            yield return FadeMissUI(missImage, 0f, 1f, missBlinkInterval / 2);
        }
    }

        /// <summary>
        /// MissUI透明度渐变
        /// </summary>
    private IEnumerator FadeMissUI(Image image, float startAlpha, float targetAlpha, float duration)
    {
        float timer = 0;
        Color startColor = image.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            image.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
    }

    /// <summary>
    /// 自动关闭MissUI（闪烁指定时长后）
    /// </summary>
    private IEnumerator AutoCloseMissUI()
    {
        // 等待指定的闪烁时长
        yield return new WaitForSeconds(missBlinkDuration);
        
        // 关闭MissUI
        DeactivateMissUI();
    }

    // ===== 新增：音游全局暂停/恢复 =====
    /// <summary>
    /// 暂停音游（停止音乐+冻结节拍逻辑）
    /// </summary>
    public void PauseRhythmGame()
    {
        if (music != null)
        {
            music.Pause(); // 暂停音乐播放
        }
        isRhythmPaused = true; // 复用原有暂停标记，冻结节拍逻辑
    }

    /// <summary>
    /// 恢复音游（继续音乐+恢复节拍逻辑）
    /// </summary>
    public void ResumeRhythmGame()
    {
        if (music != null && !music.isPlaying)
        {
            music.Play(); // 恢复音乐播放
        }
        isRhythmPaused = false; // 恢复节拍逻辑
    }
}