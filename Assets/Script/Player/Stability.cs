using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stability : MonoBehaviour
{
    [Header("Stability Settings")]
    public float maxStability = 100f;       
    public float currentStability = 100f;   
    public float idleDrainPerSecond = 2f;

    //TODO: 一些道具的参数到时候添加

    // Start is called before the first frame update
    void Start()
    {
        currentStability = maxStability;
    }

    // Update is called once per frame
    void Update()
    {
        DrainOverTime();

        CheckGameOver();
    }

    //稳定度随时间流逝
    private void DrainOverTime()
    {
        currentStability -= idleDrainPerSecond * Time.deltaTime;
        currentStability = Mathf.Clamp(currentStability, 0f, maxStability);
    }

    // 稳定度 <=0 时触发游戏结束
    private void CheckGameOver()
    {
        if (currentStability <= 0f)
        {
            Debug.Log("Game Over: Stability Broken");
            //游戏暂停
            Time.timeScale = 0f;

            // TODO: 后续可以调用 GameManager 的 GameOver() 方法
        }
    }

    // 外部调用：增加稳定度（节奏成功）
    public void AddStability(float value)
    {
        currentStability += value;
        currentStability = Mathf.Clamp(currentStability, 0f, maxStability);
    }

    // 外部调用：减少稳定度（节奏失败 / NPC 扣血）
    public void ReduceStability(float value)
    {
        currentStability -= value;
        currentStability = Mathf.Clamp(currentStability, 0f, maxStability);      
    }
}
