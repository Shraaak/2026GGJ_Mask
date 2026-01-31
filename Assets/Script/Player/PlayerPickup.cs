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
    public KeyCode pickupKey = KeyCode.E;// 拾取按键
    public LayerMask detectLayer;        // 检测层（可选，不加也能跑）

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
        // 新增：根据高度参数计算胶囊起点/终点（垂直方向可调节）
        Vector3 start = transform.position + Vector3.up * heightOffset; // 胶囊底部
        Vector3 end = start + Vector3.up * capsuleHeight;               // 胶囊顶部
        // 胶囊向前延伸detectDistance（长度可调节）
        start += transform.forward * detectDistance;
        end += transform.forward * detectDistance;
        
        // 胶囊射线检测（方向改为forward，检测前方）
        if (Physics.CapsuleCast(start, end, capsuleRadius, transform.forward, out RaycastHit hit, detectDistance, detectLayer))
        {
            // 检测是否是prop标签
            if (hit.collider.CompareTag("prop"))
            {
                return hit.collider.gameObject;
            }
        }
        return null;
    }

    // 拾取道具：隐藏物体
    void PickupProp(GameObject prop)
    {
        Debug.Log("拾取道具：" + prop.name);
        prop.SetActive(false); // 隐藏物体（也可以Destroy，但是SetActive更安全）
    }

    // Gizmos可视化调试（同步新增参数，高度/长度可直观看到）
    void OnDrawGizmos()
    {
        // 同步更新胶囊起点/终点（和检测逻辑一致）
        Vector3 start = transform.position + Vector3.up * heightOffset;
        Vector3 end = start + Vector3.up * capsuleHeight;
        start += transform.forward * detectDistance;
        end += transform.forward * detectDistance;

        // 绘制胶囊（黄色=检测范围，高度/半径/长度都可调）
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(start, capsuleRadius); // 胶囊底部圆
        Gizmos.DrawWireSphere(end, capsuleRadius);   // 胶囊顶部圆
        // 绘制胶囊侧面线条（适配高度）
        Gizmos.DrawLine(start + transform.right * capsuleRadius, end + transform.right * capsuleRadius);
        Gizmos.DrawLine(start - transform.right * capsuleRadius, end - transform.right * capsuleRadius);
        Gizmos.DrawLine(start + transform.up * capsuleRadius, end + transform.up * capsuleRadius);
        Gizmos.DrawLine(start - transform.up * capsuleRadius, end - transform.up * capsuleRadius);

        // 绘制射线（辅助看方向）
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + Vector3.up * heightOffset, start);

        // 检测到道具时，绘制道具位置（绿色）
        GameObject prop = DetectProp();
        if (prop != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(prop.transform.position, 0.3f);
        }
    }
}