using UnityEngine;
using UnityEngine.UI;

// 挂在UI动画的GameObject上，用来更新动画里的Image
public class UIAnimImage : MonoBehaviour
{
    public Image animImage; // 动画里显示道具图标的Image组件
    public GameObject animRoot; // 动画的根对象（整个弹出UI的父物体）

    void Awake()
    {
        // 游戏启动时强制隐藏动画UI（核心！）
        HideAnimUI();
    }

    // 更新动画的图标并显示动画UI
    public void SetAnimSprite(Sprite sprite)
    {
        if (animImage != null && sprite != null)
        {
            animImage.sprite = sprite;
            animImage.enabled = true;
        }
        // 显示动画根对象
        if (animRoot != null)
            animRoot.SetActive(true);
    }

    // 隐藏动画Image + 动画根对象（初始/动画结束后调用）
    public void HideAnimUI()
    {
        // 隐藏图标
        if (animImage != null)
        {
            animImage.enabled = false;
            animImage.sprite = null;
        }
        // 隐藏整个动画UI（关键：把整个动画对象关掉）
        if (animRoot != null)
            animRoot.SetActive(false);
    }
}