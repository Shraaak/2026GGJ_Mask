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

    void Update()
    {
        if (playerMove == null) return;

        // 退出掩体
        if (playerMove.isHidden && Input.GetKeyDown(KeyCode.Escape))
        {
            ExitCover();
            return;
        }

        if (!playerMove.canMove)
            return;

        // Q：打开（只有柜子有反应）
        if (Input.GetKeyDown(KeyCode.Q))
        {
            print("打开柜子");
            Cover cover = RaycastCover();
            if (cover != null)
                cover.OnOpen();
        }

        // E：进入掩体
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryEnterCover();
        }
    }

    void TryEnterCover()
    {
        Cover cover = RaycastCover();
        if (cover == null) return;

        if (!cover.CanEnter())
            return;

        EnterCover(cover);
    }

    Cover RaycastCover()
    {
        print("发出射线");

        Ray ray = new Ray(transform.position + Vector3.up * 0.8f, transform.forward);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, detectDistance))
        {
            print("无");
            return null;
        }

        print(hit.collider.GetComponent<Cover>());
        

        return hit.collider.GetComponent<Cover>();
    }

    void EnterCover(Cover cover)
    {
        currentCover = cover;
        cover.isOccupied = true;
        cover.OnEnter();

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
        currentCover.OnExit();
        currentCover = null;

        playerMove.canMove = true;
        playerMove.isHidden = false;
        playerMove.currentState = PlayerMove.PlayerState.Normal;

        if (animator != null)
            animator.SetTrigger("ExitCover");
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Vector3 origin = transform.position + Vector3.up * 0.8f;
        Vector3 dir = transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(origin, dir * detectDistance);

        // 画一个小球，表示射线终点
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(origin + dir * detectDistance, 0.05f);
    }
#endif
}