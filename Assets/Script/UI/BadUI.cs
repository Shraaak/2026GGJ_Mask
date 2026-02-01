using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BadUI : MonoBehaviour
{
    [Header("UI组件")]
    public CanvasGroup uiCanvasGroup;
    public TextMeshProUGUI dialogueText;
    public Image bgImage;

    [Header("文本配置")]
    [TextArea(3, 5)]
    public string[] dialogueLines;
    public float fadeInDuration = 1f;
    public float typeSpeed = 0.05f;

    private int currentLineIndex = 0;
    private bool isTyping = false;

    void Awake()
    {
        // 初始隐藏UI
        if (uiCanvasGroup != null)
        {
            uiCanvasGroup.alpha = 0;
            uiCanvasGroup.interactable = false;
            uiCanvasGroup.blocksRaycasts = false;
        }
    }

    // 外部调用：开始显示失败UI
    public void ShowBadUI()
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeInUI());
    }

    // UI淡入
    IEnumerator FadeInUI()
    {
        float timer = 0;
        while (timer < fadeInDuration)
        {
            timer += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0, 1, timer / fadeInDuration);
            uiCanvasGroup.alpha = alpha;
            yield return null;
        }
        uiCanvasGroup.alpha = 1;
        uiCanvasGroup.interactable = true;
        uiCanvasGroup.blocksRaycasts = true;

        // 淡入完成后开始播放第一句文本
        StartCoroutine(TypeDialogue(dialogueLines[currentLineIndex]));
    }

    // 逐字显示文本
    IEnumerator TypeDialogue(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(typeSpeed);
        }
        isTyping = false;
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
                // 所有文本播放完毕，可以在这里加退出/重启逻辑
                Debug.Log("所有对话播放完毕");
            }
        }
    }
}