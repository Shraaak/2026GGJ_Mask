using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 挂载在角色身上即可
public class PlayerPickup : MonoBehaviour
{
    [Header("拾取配置")]
    public float detectDistance = 2f;    // 检测距离（胶囊长度）
    public float capsuleRadius = 0.5f;   // 胶囊半径（适配角色宽度）
    public float capsuleHeight = 1f;     // 新增：胶囊高度（垂直方向范围）
    public float heightOffset = 0.5f;    // 新增：胶囊垂直偏移（调节胶囊中心高度）
    public float rayBackOffset = 0.3f;   // 新增：射线向后偏移距离（调这个值即可）
    public KeyCode pickupKey = KeyCode.E;// 拾取按键
    public LayerMask detectLayer;        // 检测层（可选，不加也能跑）

    public ItemBarManager itemBarManager;
    public PaperUI paperUI;

    void Update()
    {
        // 按下拾取键 + 检测到道具 → 拾取
        if (Input.GetKeyDown(pickupKey))
        {
            GameObject prop = DetectProp();
            
            if (prop != null)
            {
                PickupProp(prop);
            }
        }
    }

    // 胶囊射线检测前方prop标签物体
    GameObject DetectProp()
    {
        // 核心修改：计算向后偏移量（-transform.forward是角色后方）
        Vector3 backOffset = -transform.forward * rayBackOffset;
        // 胶囊起点/终点 = 角色位置 + 垂直偏移 + 向后偏移
        Vector3 start = transform.position + Vector3.up * heightOffset + backOffset; // 胶囊底部（后移）
        Vector3 end = start + Vector3.up * capsuleHeight;                           // 胶囊顶部（同步后移）
        
        // 修复2：用临时变量承接命中的物体，避免返回值丢失
        GameObject hitProp = null;
        
        // 胶囊射线检测：方向forward，检测前方detectDistance内的物体
        if (Physics.CapsuleCast(start, end, capsuleRadius, transform.forward, out RaycastHit hit, detectDistance, detectLayer))
        {
            print("命中了碰撞体：" + hit.collider.gameObject.name); // 新增：打印命中的物体
            // 检测是否是prop标签
            if (hit.collider.CompareTag("prop"))
            {
                hitProp = hit.collider.gameObject; // 赋值给临时变量
            }
        }
        return hitProp; // 返回临时变量（核心修复）
    }

    // 拾取道具：隐藏物体
    void PickupProp(GameObject prop)
    {
        if(prop.GetComponent<Item>())
            itemBarManager.AddItem(prop.GetComponent<Item>().selfData);
        else if(prop.GetComponent<Paper>())
            //paperUI数目+1
            paperUI.currentPaparCount++;
        prop.SetActive(false); // 隐藏物体（也可以Destroy，但是SetActive更安全）
    }

    // Gizmos可视化调试（同步修复起点/终点 + 后移）
    void OnDrawGizmos()
    {
        // 同步修改：Gizmos绘制位置也加向后偏移，和检测逻辑一致
        Vector3 backOffset = -transform.forward * rayBackOffset;
        Vector3 start = transform.position + Vector3.up * heightOffset + backOffset;
        Vector3 end = start + Vector3.up * capsuleHeight;

        // 绘制胶囊（黄色=检测范围）
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(start, capsuleRadius); // 胶囊底部圆
        Gizmos.DrawWireSphere(end, capsuleRadius);   // 胶囊顶部圆
        // 绘制胶囊侧面线条（适配高度）
        Gizmos.DrawLine(start + transform.right * capsuleRadius, end + transform.right * capsuleRadius);
        Gizmos.DrawLine(start - transform.right * capsuleRadius, end - transform.right * capsuleRadius);
        Gizmos.DrawLine(start + transform.up * capsuleRadius, end + transform.up * capsuleRadius);
        Gizmos.DrawLine(start - transform.up * capsuleRadius, end - transform.up * capsuleRadius);

        // 绘制射线（从后移后的位置到前方detectDistance，辅助看方向）
        Gizmos.color = Color.red;
        Gizmos.DrawLine(start, start + transform.forward * detectDistance);

        // 检测到道具时，绘制道具位置（绿色）
        GameObject prop = DetectProp();
        if (prop != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(prop.transform.position, 0.3f);
        }
    }
}