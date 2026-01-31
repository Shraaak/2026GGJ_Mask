using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Cloak,//神秘迷彩布
    PlumJuice,//西梅汁
    SpicyNoodle,//火鸡面
    FakeTooth,//缺牙齿
    Skateboard//超级滑板
}

[CreateAssetMenu(menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    public ItemType itemType;
    public Sprite BarIcon;
    public Sprite UIIcon;
    public float workingTime;
}
