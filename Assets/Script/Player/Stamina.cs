using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina = 100f;

    [Header("Cost / Recover")]
    public float sprintCostPerSecond = 25f;//每秒消耗
    public float recoverPerSecond = 15f;//每秒恢复
    public float hideRecoverMultiplier = 1.5f;//在掩体内的恢复倍率

    private PlayerMove player;

    void Start()
    {
        player = GetComponent<PlayerMove>();
        currentStamina = maxStamina;
    }

    void Update()
    {
        Recover();
    }

    void Recover()
    {
        if (player == null) return;

        float rate = recoverPerSecond;

        // 掩体内恢复更快
        if (player.currentState == PlayerMove.PlayerState.Hiding)
            rate *= hideRecoverMultiplier;

        // 不冲刺就恢复
        if (!Input.GetKey(KeyCode.LeftShift) || !player.canMove)
        {
            currentStamina += rate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }
    }

    public bool CanDash()
    {
        return currentStamina > 0f && player.currentState == PlayerMove.PlayerState.Normal;
    }

    public void ConsumeSprint()
    {
        currentStamina -= sprintCostPerSecond * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }
}
