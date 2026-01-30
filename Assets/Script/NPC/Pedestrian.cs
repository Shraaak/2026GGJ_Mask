using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Pedestrian : MonoBehaviour
{
    public enum NPCState 
    { 
        Patrol,
        Suspicious, 
        Detected 
    }

    [Header("Base View")]
    public float viewAngle = 60f;
    public float suspiciousRadius = 20f;
    public float detectRadius = 10f;
    public float suspiciousStopDistance = 3.5f; // 离玩家多远停下
    public float suspiciousObserveTime = 2.5f;  // 停下来观察多久

    [Header("Movement")]
    public float patrolSpeed = 1.5f;
    public float alertSpeed = 2.5f;

    [Header("References")]
    public Transform player;
    protected PlayerMove playerMove;
    protected Stability playerStability;
    protected NavMeshAgent agent;

    [Header("Patrol")]
    [SerializeField] protected Transform[] patrolPoints;
    protected int patrolIndex = 0;

    protected NPCState currentState = NPCState.Patrol;

    protected bool observing = false;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        playerMove = player.GetComponent<PlayerMove>();
        playerStability = player.GetComponent<Stability>();
    }

    protected virtual void Update()
    {
        if (playerMove != null && playerMove.isHidden)
        {
            //Todo: 玩家隐藏时npc闲逛
            SetState(NPCState.Patrol);
            return;
        }

        //npc与玩家的距离
        float dist = Vector3.Distance(transform.position, player.position);

        //不在视野中npc闲逛
        if (!InView(player)) 
        {
            SetState(NPCState.Patrol);
            return;
        }

        if(dist < detectRadius)
            SetState(NPCState.Detected);
        else if(dist < suspiciousRadius)
            SetState(NPCState.Suspicious);
        else
            SetState(NPCState.Patrol);
    }

    protected virtual void SetState(NPCState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case NPCState.Patrol:
                agent.speed = patrolSpeed;
                OnPatrol();
                break;

            case NPCState.Suspicious:
                agent.speed = alertSpeed;
                OnSuspicious();
                break;

            case NPCState.Detected:
                agent.speed = alertSpeed;
                OnDetected();
                break;
        }
    }

    protected bool InView(Transform target)
    {
        Vector3 dir = (target.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dir);
        return angle < viewAngle / 2f;
    }

    protected void LookAtPlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.Slerp(
        transform.rotation,
        Quaternion.LookRotation(dir),
        Time.deltaTime * 4f
        );
    }

    // 给子类用的接口
    protected abstract void OnPatrol();
    protected abstract void OnSuspicious();
    protected abstract void OnDetected();

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Debug.Log("Draw Gizmos");
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

