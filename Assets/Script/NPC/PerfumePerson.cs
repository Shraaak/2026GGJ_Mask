using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerfumePerson : Pedestrian
{
    public GameObject smellZonePrefab;

    protected override void OnPatrol()
    {
        Debug.Log("SweepingAunt Patrolling: " + agent.isOnNavMesh);

        SpawnSmell();

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

    protected override void OnDetected()
    {
        
    }

    public class SmellZone : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // other.GetComponent<PlayerMove>()
                //     .TryForceSneeze();
            }
        }
    }

    void SpawnSmell()
    {
        if (Random.value < 0.01f)
        {
            Instantiate(smellZonePrefab, transform.position, Quaternion.identity);
        }
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
