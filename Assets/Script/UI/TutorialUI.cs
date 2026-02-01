using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [Header("组件引用")]
    public TextMeshProUGUI promptText; // 仅保留闪烁文字

    [Header("闪烁配置")]
    public float blinkInterval = 0.8f; // 闪烁周期

    private bool isClickTriggered = false; // 点击触发标记

    void Awake()
    {
        // 初始隐藏UI
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 显示教程UI并开始文字闪烁
    /// </summary>
    public void ShowTutorial()
    {
        gameObject.SetActive(true);
        isClickTriggered = false; // 重置标记
        StartCoroutine(BlinkText()); // 启动文字闪烁
    }

    // 文字闪烁核心逻辑（仅控制透明度）
    IEnumerator BlinkText()
    {
        while (gameObject.activeSelf && !isClickTriggered)
        {
            // 淡出（0.5秒）
            yield return FadeText(1, 0, blinkInterval / 2);
            // 淡入（0.5秒）
            yield return FadeText(0, 1, blinkInterval / 2);
        }
    }

    // 文字透明度渐变
    IEnumerator FadeText(float startAlpha, float targetAlpha, float duration)
    {
        float timer = 0;
        Color startColor = promptText.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime; // 不受Time.timeScale影响
            float t = timer / duration;
            promptText.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
    }

    void Update()
    {
        // 点击任意处触发
        if (Input.GetMouseButtonDown(0) && gameObject.activeSelf && !isClickTriggered)
        {
            isClickTriggered = true; // 标记为已点击
        }
    }

    /// <summary>
    /// 外部获取点击状态（核心：返回bool）
    /// </summary>
    public bool IsClicked()
    {
        return isClickTriggered;
    }

    /// <summary>
    /// 隐藏教程UI
    /// </summary>
    public void HideTutorial()
    {
        FindObjectOfType<RhythmManager>().ResumeRhythmGame();
        gameObject.SetActive(false);
        StopAllCoroutines(); // 停止闪烁协程
    }
}