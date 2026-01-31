using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Pedestrian : MonoBehaviour
{
    public enum NPCState
    {
        Patrol,
        Suspicious,
        Detected
    }

    [Header("View")]
    public float viewAngle = 60f;
    public float suspiciousRadius = 20f;
    public float detectRadius = 10f;
    public LayerMask obstacleMask;

    [Header("Suspicious Behavior")]
    public float suspiciousStopDistance = 3.5f;
    public float observeTime = 2.5f;
    public float stabilityDrainMultiplier = 1.5f;

    [Header("Movement")]
    public float patrolSpeed = 1.5f;
    public float alertSpeed = 2.5f;

    [Header("References")]
    public Transform player;
    public Animator animator;

    NavMeshAgent agent;
    PlayerMove playerMove;
    Stability stability;

    [Header("Patrol")]
    public Transform[] patrolPoints;
    int patrolIndex;

    NPCState currentState = NPCState.Patrol;
    bool observing = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        playerMove = player.GetComponent<PlayerMove>();
        stability = player.GetComponent<Stability>();
    }

    void Update()
    {
        if (playerMove.isHidden)
        {
            SetPatrol();
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);

        if (!InView(player) || IsBlocked(player))
        {
            SetPatrol();
            return;
        }

        if (dist <= detectRadius)
        {
            SetDetected();
        }
        else if (dist <= suspiciousRadius)
        {
            SetSuspicious();
        }
        else
        {
            SetPatrol();
        }
    }

    // ======================
    // 状态切换
    // ======================

    void SetPatrol()
    {
        if (currentState == NPCState.Patrol) return;

        currentState = NPCState.Patrol;
        observing = false;

        agent.speed = patrolSpeed;
        agent.isStopped = false;

        animator.SetBool("Walk", true);
        animator.SetBool("Observe", false);
    }

    void SetSuspicious()
    {
        if (currentState == NPCState.Suspicious && observing) return;

        currentState = NPCState.Suspicious;
        agent.speed = alertSpeed;

        stability.SetDrainMultiplier(stabilityDrainMultiplier);

        if (!observing)
            StartCoroutine(SuspiciousRoutine());
    }

    void SetDetected()
    {
        if (currentState == NPCState.Detected) return;

        currentState = NPCState.Detected;
        GameManager.Instance.GameOver();
    }

    // ======================
    // 行为
    // ======================

    IEnumerator SuspiciousRoutine()
    {
        observing = true;

        // 走向玩家
        agent.isStopped = false;
        animator.SetBool("Walk", true);
        animator.SetBool("Observe", false);

        while (Vector3.Distance(transform.position, player.position) > suspiciousStopDistance)
        {
            agent.SetDestination(player.position);
            LookAtPlayer();
            yield return null;
        }

        // 停下观察
        agent.isStopped = true;
        animator.SetBool("Walk", false);
        animator.SetBool("Observe", true);

        yield return new WaitForSeconds(observeTime);

        // 恢复
        stability.SetDrainMultiplier(1f);
        SetPatrol();
    }

    // ======================
    // 视野 & 工具
    // ======================

    bool InView(Transform target)
    {
        Vector3 dir = (target.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dir);
        return angle <= viewAngle * 0.5f;
    }

    bool IsBlocked(Transform target)
    {
        Vector3 origin = transform.position + Vector3.up * 1.6f;
        Vector3 dir = (target.position + Vector3.up) - origin;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, suspiciousRadius, obstacleMask))
        {
            return hit.transform != target;
        }

        return false;
    }

    void LookAtPlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            Time.deltaTime * 5f
        );
    }

    void FixedUpdate()
    {
        if (currentState == NPCState.Patrol && patrolPoints.Length > 0 && !agent.pathPending)
        {
            if (!agent.hasPath || agent.remainingDistance < 0.3f)
            {
                agent.SetDestination(patrolPoints[patrolIndex].position);
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!enabled) return;

        Vector3 pos = transform.position + Vector3.up * 0.1f;

        // 根据状态换颜色
        Color stateColor = Color.green;
        switch (currentState)
        {
            case NPCState.Patrol:
                stateColor = Color.green;
                break;
            case NPCState.Suspicious:
                stateColor = Color.yellow;
                break;
            case NPCState.Detected:
                stateColor = Color.red;
                break;
        }

        // 可疑半径
        Gizmos.color = new Color(1f, 0.8f, 0f, 0.15f);
        Gizmos.DrawSphere(pos, suspiciousRadius);

        // 发现半径
        Gizmos.color = new Color(1f, 0f, 0f, 0.15f);
        Gizmos.DrawSphere(pos, detectRadius);

        // 扇形视野
        DrawViewGizmos(pos, viewAngle, suspiciousRadius, stateColor);

        // 朝向线
        Gizmos.color = stateColor;
        Gizmos.DrawLine(pos, pos + transform.forward * suspiciousRadius);
    }

    void DrawViewGizmos(Vector3 pos, float angle, float radius, Color color)
    {
        Gizmos.color = color;

        int segments = 30;
        float halfAngle = angle / 2f;

        Vector3 prevPoint = pos;

        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = -halfAngle + (angle / segments) * i;
            Vector3 dir = Quaternion.Euler(0, currentAngle, 0) * transform.forward;
            Vector3 point = pos + dir * radius;

            if (i > 0)
                Gizmos.DrawLine(prevPoint, point);

            prevPoint = point;
        }
    }
#endif
}