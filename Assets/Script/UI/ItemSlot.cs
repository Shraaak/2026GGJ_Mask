using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Image iconImage;          // 道具图标 Image
    private ItemData currentItem;    // 当前格子里的道具数据
    public Animation UIAnim;
    public Sprite uiSprite;

    // 外部用来判断这个格子是不是空的
    public bool IsEmpty => currentItem == null;

    void Awake()
    {
        // 一开始要隐藏图标
        // 不然 Unity 默认 Image 是显示的
        iconImage.enabled = false;
    }

    // 设置道具（拾取道具时调用）
    public void SetItem(ItemData item)
    {
        currentItem = item;          // 记录数据
        iconImage.sprite = item.BarIcon; // 换成对应图标
        iconImage.enabled = true;     // 显示图标
        uiSprite = item.UIIcon;
    }

    // 获取当前道具（使用道具时调用）
    public ItemData GetItem()
    {
        return currentItem;
        //播放道具拾取UI动画

    }

    // 清空格子（道具用完）
    public void Clear()
    {
        currentItem = null;           // 数据清空
        iconImage.sprite = null;      // 清掉 sprite（防止脏数据）
        iconImage.enabled = false;    // 隐藏图标
    }
}