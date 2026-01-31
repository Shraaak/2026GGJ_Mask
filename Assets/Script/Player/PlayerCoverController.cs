using UnityEngine;

public class PlayerCoverController : MonoBehaviour
{
    [Header("Raycast")]
    public float detectDistance = 1.5f;
    public LayerMask coverLayer;

    [Header("References")]
    public PlayerMove playerMove;
    public Animator animator;

    // 当前是否处于掩体状态
    private bool inCover = false;

    // 当前掩体（用来播动画）
    private Cover currentCover;

    void Update()
    {
        if (playerMove == null) return;

        // 按 Q 切换“进入 / 退出掩体”
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!inCover)
                TryEnterCover();
            else
                ExitCover();
        }
    }

    /// <summary>
    /// 尝试进入掩体
    /// </summary>
    void TryEnterCover()
    {
        Cover cover = RaycastCover();
        if (cover == null) return;

        currentCover = cover;

        // 播放掩体动画（柜子打开 / 桌子下钻）
        cover.OnOpen();

        // 玩家进入隐藏状态
        EnterCover();
    }

    /// <summary>
    /// 玩家进入掩体
    /// </summary>
    void EnterCover()
    {
        inCover = true;

        // 玩家不能动
        playerMove.canMove = false;

        // ⭐关键：NPC 看不到你
        playerMove.isHidden = true;

        // 播放玩家躲藏动画
        if (animator != null)
            animator.SetTrigger("EnterCover");

        // 直接隐藏玩家模型（最省事）
        SetPlayerVisible(false);
    }

    /// <summary>
    /// 玩家离开掩体
    /// </summary>
    void ExitCover()
    {
        inCover = false;

        // 播放掩体关闭 / 出来动画
        if (currentCover != null)
            currentCover.OnExit();

        // 播放玩家出来动画
        if (animator != null)
            animator.SetTrigger("ExitCover");

        // 恢复玩家状态
        playerMove.canMove = true;
        playerMove.isHidden = false;

        SetPlayerVisible(true);

        currentCover = null;
    }

    /// <summary>
    /// 射线检测掩体
    /// </summary>
    Cover RaycastCover()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.8f, transform.forward);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, detectDistance, coverLayer))
            return null;

        return hit.collider.GetComponent<Cover>();
    }

    /// <summary>
    /// 显示 / 隐藏玩家（最暴力也最稳）
    /// </summary>
    void SetPlayerVisible(bool visible)
    {
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = visible;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Vector3 origin = transform.position + Vector3.up * 0.8f;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(origin, transform.forward * detectDistance);
    }
#endif
}