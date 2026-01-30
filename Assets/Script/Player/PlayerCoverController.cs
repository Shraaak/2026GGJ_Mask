using System.Collections;
using UnityEngine;

public class PlayerCoverController : MonoBehaviour
{
    [Header("Raycast")]
    public float detectDistance = 1.5f;
    public LayerMask coverLayer;

    [Header("References")]
    public Rigidbody rb;
    public PlayerMove playerMove;
    public Animator animator;

    private Cover currentCover;
    private bool cabinetOpened = false;

    void Update()
    {
        if (playerMove == null) return;

        // Exit cover
        if (playerMove.isHidden && Input.GetKeyDown(KeyCode.Escape))
        {
            ExitCover();
            return;
        }

        if (!playerMove.canMove)
            return;

        // Q：打开柜子（只对柜子生效）
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TryOpenCabinet();
        }

        // E：进入掩体（桌子 / 已打开的柜子）
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryEnterCover();
        }
    }

    void TryOpenCabinet()
    {
        Cover cover = RaycastCover();
        if (cover == null) return;

        if (cover.coverType != CoverType.Cabinet)
            return;

        if (cabinetOpened)
            return;

        currentCover = cover;
        cabinetOpened = true;

        // 播放打开柜子的动画
        if (animator != null)
            animator.SetTrigger("OpenCabinet");
    }

    void TryEnterCover()
    {
        Cover cover = RaycastCover();
        if (cover == null || cover.isOccupied)
            return;

        // 桌子：直接进入
        if (cover.coverType == CoverType.Table)
        {
            EnterCover(cover);
            return;
        }

        // 柜子：必须先打开
        if (cover.coverType == CoverType.Cabinet && cabinetOpened)
        {
            EnterCover(cover);
        }
    }

    Cover RaycastCover()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.8f, transform.forward);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, detectDistance, coverLayer))
            return null;

        return hit.collider.GetComponent<Cover>();
    }

    void EnterCover(Cover cover)
    {
        currentCover = cover;
        cover.isOccupied = true;

        rb.velocity = Vector3.zero;
        playerMove.canMove = false;
        playerMove.isHidden = true;

        transform.position = cover.hidePoint.position;
        transform.rotation = cover.hidePoint.rotation;

        if (animator != null)
            animator.SetTrigger("EnterCover");

        playerMove.currentState = PlayerMove.PlayerState.Hiding;
    }

    void ExitCover()
    {
        if (currentCover == null) return;

        currentCover.isOccupied = false;
        currentCover = null;
        cabinetOpened = false;

        playerMove.canMove = true;
        playerMove.isHidden = false;
        playerMove.currentState = PlayerMove.PlayerState.Normal;

        if (animator != null)
            animator.SetTrigger("ExitCover");
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(
            transform.position + Vector3.up * 0.8f,
            transform.position + Vector3.up * 0.8f + transform.forward * detectDistance
        );
    }
#endif
}
