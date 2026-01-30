using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneGuy : Pedestrian
{
    [Header("PhoneGuy Value")]
    //public RhythmUI rhythmUI;
    [SerializeField] private float noiseRadius = 12f;
    [SerializeField] private float reduceStabilityValue = 10f;

    protected override void OnDetected()
    {
        EmitNoise();
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
        EmitNoise();
        playerStability.ReduceStability(reduceStabilityValue * Time.deltaTime);

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

    void EmitNoise()
    {
        // float dist = Vector3.Distance(transform.position, player.position);
        // if (dist < noiseRadius)
        // {
        //     rhythmUI.AddDisturbance(0.5f); // 模糊 / 抖动
        // }
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
