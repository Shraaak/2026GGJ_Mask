using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform follow;
    private Vector3 offset;

    [Header("平滑跟随配置")]
    public float smoothSpeed = 5f; // 跟随平滑速度（值越小越丝滑，建议2-8）
    public bool useDamping = true; // 是否启用阻尼（更自然的减速效果）
    public float dampingFactor = 0.1f; // 阻尼系数（值越小阻尼越强，建议0.05-0.2）

    private Vector3 velocity = Vector3.zero; // 阻尼用速度变量

    void Start()
    {
        // 保留原有偏移逻辑
        offset = transform.position - follow.position;
    }

    // 改用 LateUpdate：确保跟随目标先移动，相机后更新，避免抖动
    void LateUpdate()
    {
        if (follow == null) return;

        // 目标位置 = 跟随目标位置 + 初始偏移
        Vector3 targetPosition = follow.position + offset;
        
        // 平滑跟随（二选一，根据useDamping切换）
        if (useDamping)
        {
            // 阻尼平滑（最丝滑，推荐）
            transform.position = Vector3.SmoothDamp(
                transform.position, 
                targetPosition, 
                ref velocity, 
                dampingFactor, 
                smoothSpeed
            );
        }
        else
        {
            // 线性插值平滑（基础版）
            transform.position = Vector3.Lerp(
                transform.position, 
                targetPosition, 
                smoothSpeed * Time.deltaTime
            );
        }
    }
}