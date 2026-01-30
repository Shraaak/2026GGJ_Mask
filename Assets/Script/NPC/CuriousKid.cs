using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuriousKid : Pedestrian
{
    [Header("CuriousKid Settings")]
    [SerializeField] private float reduceStabilityValue = 15f;
    [SerializeField] private float StareAtTime = 3f;
    //是否盯着Player
    private bool staring = false;

    protected override void OnDetected()
    {
        playerStability.ReduceStability(reduceStabilityValue*Time.deltaTime);
    }

    protected override void OnPatrol()
    {
        Debug.Log("CuriousKid Patrolling: " + agent.isOnNavMesh);
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        agent.isStopped = false;
        agent.speed = patrolSpeed;

        // 如果没有目标 或 快到达当前点 切下一个
        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            agent.SetDestination(patrolPoints[patrolIndex].position);
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            
        }
    }

    protected override void OnSuspicious()
    {
        // 玩家躲着 → 原逻辑
        if (playerMove.isHidden)
        {
            if (!staring)
                StartCoroutine(StareAtCabinet());
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);

        // 还没靠近到“怀疑距离” → 走过去
        if (dist > suspiciousStopDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            LookAtPlayer();
        }
        // 已经到位 → 停下来盯
        else
        {
            agent.isStopped = true;

            if (!observing)
                StartCoroutine(ObservePlayer());
        }
    }

    //小孩盯着看逻辑
    IEnumerator StareAtCabinet()
    {
        staring = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(StareAtTime);

        agent.isStopped = false;
        staring = false;
    }

    IEnumerator ObservePlayer()
    {
        observing = true;

        // 原地看着玩家或者播放相应动画
        float timer = 0f;
        while (timer < suspiciousObserveTime)
        {
            LookAtPlayer();
            timer += Time.deltaTime;
            yield return null;
        }

        observing = false;
    }
}
