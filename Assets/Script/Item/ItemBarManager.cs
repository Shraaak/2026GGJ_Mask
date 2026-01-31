using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemBarManager : MonoBehaviour
{
    [Header("Settings")]
    public ItemSlot[] slots = new ItemSlot[4];
    public ItemData testItemData;
    public PlayerMove playerMove;
    public RhythmManager rhythm;

    //测试测试
    void Start()
    {
        AddItem(testItemData);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) UseItem(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseItem(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) UseItem(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) UseItem(3);
    }

    // 添加道具（拾取时调用）
    public bool AddItem(ItemData item)
    {
        AudioManager.Instance.PlayOneShot("GetItem");
        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
            {
                slot.SetItem(item);
                return true;
            }
        }
        return false;//满了
    }

    void UseItem(int index)
    {
        if (index < 0 || index >= slots.Length) return;
        if(slots[index].IsEmpty) return;

        ItemData item = slots[index].GetItem();
        ActivateItem(item);
        slots[index].Clear();
    }

    void ActivateItem(ItemData item)
    {
        Debug.Log("使用道具：" + item.itemType);

        //通过枚举实现各个道具功能
        switch (item.itemType)
        {
            case ItemType.Cloak:
                AudioManager.Instance.PlayOneShot("ClothUse");
                // TODO: 稳定度暂停
                StartCoroutine(CloakCoroutine(item.workingTime));
                break;

            case ItemType.PlumJuice:
                AudioManager.Instance.PlayOneShot("JuiceUse");
                // TODO: 节奏 +30%
                StartCoroutine(PlumJuiceCoroutine(item.workingTime));
                break;

            case ItemType.SpicyNoodle:
                AudioManager.Instance.PlayOneShot("NoodleUse");
                // TODO: 暂停 + 后惩罚
                StartCoroutine(SpicyNoodleCoroutine(item.workingTime));
                break;
                
            case ItemType.Skateboard:
                AudioManager.Instance.PlayOneShot("SketeboardsUse");
                // TODO: 冲刺不耗体力
                StartCoroutine(SkateboardCoroutine(item.workingTime));
                break;
        }
    }

    IEnumerator CloakCoroutine(float duration)
    {
        // 玩家不能动
        playerMove.canMove = false;

        // 告诉 NPC：玩家是隐藏态
        playerMove.isHidden = true;

        yield return new WaitForSeconds(duration);

        playerMove.canMove = true;
        playerMove.isHidden = false;
    }

    IEnumerator PlumJuiceCoroutine(float duration)
    {

        rhythm.SetSpeedMultiplier(1.3f);
        playerMove.SetSpeedMultiplier(1.1f);

        yield return new WaitForSeconds(duration);

        rhythm.SetSpeedMultiplier(1f);
        playerMove.SetSpeedMultiplier(1f);
    }

    IEnumerator SpicyNoodleCoroutine(float duration)
    {
        // 扣血
        playerMove.stability.ReduceStability(10);
        //图标消失

        yield return new WaitForSeconds(duration);

        rhythm.SetSpeedMultiplier(1.5f);

        yield return new WaitForSeconds(20f);

        rhythm.SetSpeedMultiplier(1f);
        //图标恢复
    }


    IEnumerator SkateboardCoroutine(float duration)
    {

        playerMove.EnableFreeDash(true);
        playerMove.SetSpeedMultiplier(playerMove.dashMultiplier);

        yield return new WaitForSeconds(duration);

        playerMove.EnableFreeDash(false);
        playerMove.SetSpeedMultiplier(-playerMove.dashMultiplier);
    }

}
