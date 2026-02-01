using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// 直接挂载在Button上
public class StartGameUI : MonoBehaviour
{
    public AudioClip hoverSound;    // 鼠标悬浮音效
    public AudioClip clickSound;    // 点击音效
    public AudioSource audioSource; // 播放音效的音频源

    // 鼠标进入按钮区域时自动调用（Unity内置方法，无需手动绑定）
    public void OnMouseEnter()
    {
        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    // 按钮点击时调用（直接在Inspector绑定，不用代码绑定）
    public void OnButtonClick()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
        SceneManager.LoadScene(1); // 切换到索引1的场景
    }
}