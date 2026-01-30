using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweepingAunt : Pedestrian
{
    [Header("SweepingAunt Settings")]
    [SerializeField] private float viewAngleMu = 1.5f;
    [SerializeField] private float detectRadiusMu = 1.5f;
    [SerializeField] private float alertSpeedMu = 1.5f;
    [SerializeField] private float reduceStabilityValue= 30f;
    [SerializeField] private float goBerserkTime= 3f;

    //是否愤怒
    private bool enraged = false;

    protected override void OnDetected()
    {
        //看向玩家并减少稳定度
        LookAtPlayer();
        playerStability.ReduceStability(reduceStabilityValue * Time.deltaTime);
    }

    protected override void OnPatrol()
    {
        Debug.Log("SweepingAunt Patrolling: " + agent.isOnNavMesh);
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

    IEnumerator GoBerserk()
    {
        enraged = true;
        viewAngle *= viewAngleMu;
        detectRadius *= detectRadiusMu;
        alertSpeed *= alertSpeedMu;

        yield return new WaitForSeconds(goBerserkTime);

        enraged = false;
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
