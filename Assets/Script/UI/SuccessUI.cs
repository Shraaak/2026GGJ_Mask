using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SuccessUI : MonoBehaviour
{
    [Header("UI组件")]
    public CanvasGroup uiCanvasGroup;
    public TextMeshProUGUI dialogueText;
    public Image bgImage;

    [Header("文本配置")]
    [TextArea(3, 5)]
    public string[] dialogueLines; // 成功结局的台词数组
    public float fadeInDuration = 1f; // UI淡入时长（默认1秒）
    public float typeSpeed = 0.05f; // 逐字播放速度（默认0.05秒/字）

    private int currentLineIndex = 0;
    private bool isTyping = false;

    // 单例（和BadUI一致，保证全局唯一）
    public static SuccessUI Instance;

    void Awake()
    {
        // 单例初始化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 可选：跨场景保留
        }
        else
        {
            Destroy(gameObject);
        }

        // 初始隐藏UI
        if (uiCanvasGroup != null)
        {
            uiCanvasGroup.alpha = 0;
            uiCanvasGroup.interactable = false;
            uiCanvasGroup.blocksRaycasts = false;
        }
    }

    // 外部调用：开始显示成功UI
    public void ShowSuccessUI()
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeInUI());
    }

    // UI淡入逻辑（和BadUI完全一致）
    IEnumerator FadeInUI()
    {
        float timer = 0;
        while (timer < fadeInDuration)
        {
            timer += Time.unscaledDeltaTime; // 不受Time.timeScale影响
            float alpha = Mathf.Lerp(0, 1, timer / fadeInDuration);
            uiCanvasGroup.alpha = alpha;
            yield return null;
        }
        uiCanvasGroup.alpha = 1;
        uiCanvasGroup.interactable = true;
        uiCanvasGroup.blocksRaycasts = true;

        // 淡入完成后播放第一句文本
        StartCoroutine(TypeDialogue(dialogueLines[currentLineIndex]));
    }

    // 逐字显示文本（和BadUI完全一致）
    IEnumerator TypeDialogue(string line)
    {
        isTyping = true;
        dialogueText.text = ""; // 清空原有文本
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(typeSpeed);
        }
        isTyping = false; // 打字完成
    }

    void Update()
    {
        // 点击屏幕播放下一句（仅当不在打字时）
        if (!isTyping && Input.GetMouseButtonDown(0))
        {
            currentLineIndex++;
            if (currentLineIndex < dialogueLines.Length)
            {
                StartCoroutine(TypeDialogue(dialogueLines[currentLineIndex]));
            }
            else
            {
                // 所有文本播放完毕（可加重启/退出逻辑）
                Debug.Log("成功结局台词播放完毕");
            }
        }
    }
}
