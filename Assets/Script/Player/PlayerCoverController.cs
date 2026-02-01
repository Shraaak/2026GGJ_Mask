using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 挂载在角色身上
public class PlayerCoverController : MonoBehaviour
{
    [Header("掩体检测配置（和拾取系统一致）")]
    public float detectDistance = 1.5f;    // 检测距离
    public float capsuleRadius = 0.5f;     // 胶囊半径（适配角色宽度）
    public float capsuleHeight = 1f;       // 胶囊垂直高度
    public float heightOffset = 0.8f;      // 胶囊垂直偏移（对应原0.8f高度）
    public float rayBackOffset = 0.3f;     // 新增：射线向后偏移的距离（调这个值即可）
    public KeyCode coverKey = KeyCode.Q;   // 进入/退出掩体按键
    public LayerMask coverLayer;           // 掩体检测层

    [Header("引用")]
    public PlayerMove playerMove;
    public Animator animator;

    // 当前是否处于掩体状态
    private bool inCover = false;
    // 当前掩体
    private Cover currentCover;

    void Update()
    {
        if (playerMove == null) return;

        // 按Q切换进入/退出掩体
        if (Input.GetKeyDown(coverKey))
        {
            if (!inCover)
            {
                AudioManager.Instance.PlayOneShot("open");
                TryEnterCover();
            }
            else
            {
                AudioManager.Instance.PlayOneShot("close");
                ExitCover();
            }
        }
    }

    /// <summary>
    /// 尝试进入掩体（核心：胶囊射线检测）
    /// </summary>
    void TryEnterCover()
    {
        print("TryEnterCover");
        Cover cover = DetectCover(); // 替换为胶囊检测
        if (cover == null)
        {
            print("未检测到掩体");
            return;
        }

        currentCover = cover;
        cover.OnOpen(); // 播放柜子打开动画

        // 玩家进入掩体状态
        EnterCover();
    }

    /// <summary>
    /// 胶囊射线检测前方掩体（和拾取系统完全一致的模式）
    /// </summary>
    Cover DetectCover()
    {
        // 核心修改：叠加向后偏移（transform.forward是前，-transform.forward是后）
        Vector3 backOffset = -transform.forward * rayBackOffset;
        // 胶囊起点/终点 = 角色位置 + 垂直偏移 + 向后偏移
        Vector3 start = transform.position + Vector3.up * heightOffset + backOffset; // 胶囊底部（后移）
        Vector3 end = start + Vector3.up * capsuleHeight;                            // 胶囊顶部（同步后移）

        Cover hitCover = null; // 临时变量承接，避免返回null
        // 胶囊射线检测前方掩体（检测方向还是向前，只是发射点后移）
        if (Physics.CapsuleCast(start, end, capsuleRadius, transform.forward, out RaycastHit hit, detectDistance, coverLayer))
        {
            print("命中碰撞体：" + hit.collider.gameObject.name);
            // 获取掩体组件
            hitCover = hit.collider.GetComponent<Cover>();
            if (hitCover != null)
            {
                print("检测到掩体：" + hitCover.gameObject.name);
            }
            else
            {
                print("命中的物体没有Cover组件");
            }
        }
        return hitCover;
    }

    /// <summary>
    /// 进入掩体（原有逻辑不变）
    /// </summary>
    void EnterCover()
    {
        inCover = true;
        playerMove.canMove = false;
        playerMove.isHidden = true;

        // 播放玩家躲藏动画
        if (animator != null)
            animator.SetTrigger("EnterCover");

        // 隐藏玩家模型
        SetPlayerVisible(false);
    }

    /// <summary>
    /// 退出掩体（原有逻辑不变）
    /// </summary>
    void ExitCover()
    {
        inCover = false;

        if (currentCover != null)
            currentCover.OnExit();

        if (animator != null)
            animator.SetTrigger("ExitCover");

        playerMove.canMove = true;
        playerMove.isHidden = false;

        SetPlayerVisible(true);
        currentCover = null;
    }

    /// <summary>
    /// 显示/隐藏玩家（原有逻辑不变）
    /// </summary>
    void SetPlayerVisible(bool visible)
    {
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = visible;
    }

    #region Gizmos可视化（绿色，同步后移）
    void OnDrawGizmos()
    {
        // 同步修改：Gizmos绘制的位置也加上向后偏移，和检测逻辑一致
        Vector3 backOffset = -transform.forward * rayBackOffset;
        Vector3 start = transform.position + Vector3.up * heightOffset + backOffset;
        Vector3 end = start + Vector3.up * capsuleHeight;

        // 1. 绘制胶囊检测范围（绿色，按你的要求）
        Gizmos.color = Color.green;
        // 胶囊底部/顶部圆
        Gizmos.DrawWireSphere(start, capsuleRadius);
        Gizmos.DrawWireSphere(end, capsuleRadius);
        // 胶囊侧面线条
        Gizmos.DrawLine(start + transform.right * capsuleRadius, end + transform.right * capsuleRadius);
        Gizmos.DrawLine(start - transform.right * capsuleRadius, end - transform.right * capsuleRadius);
        Gizmos.DrawLine(start + transform.up * capsuleRadius, end + transform.up * capsuleRadius);
        Gizmos.DrawLine(start - transform.up * capsuleRadius, end - transform.up * capsuleRadius);

        // 2. 绘制检测方向射线（浅绿色，区分范围）
        Gizmos.color = new Color(0, 1, 0, 0.5f); // 半透明绿色
        Gizmos.DrawLine(start, start + transform.forward * detectDistance);

        // 3. 检测到掩体时，绘制掩体位置（深绿色小球）
        Cover cover = DetectCover();
        if (cover != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(cover.transform.position, 0.3f);
        }
    }
    #endregion
}