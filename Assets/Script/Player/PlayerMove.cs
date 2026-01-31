using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    public static PlayerMove Instance {get; private set; }
    public enum PlayerState
    {
        Normal,
        Hiding
    }

    [Header("Move")]
    public float moveSpeed = 5f;
    public float dashMultiplier = 1.4f;
    public float rotateSmoothTime = 0.1f;//越小越快
    private float rotateSmoothVelocity;
    private bool freeDash = false;

    [Header("State")]
    public PlayerState currentState = PlayerState.Normal;
    public bool canMove = true;
    public bool isHidden = false;

    [Header("Anim")]
    public Animator anim;
    private bool IsWalk = false;

    private Rigidbody rb;
    private Vector3 moveInput;

    //外部系统引用
    public Stamina stamina;
    public Stability stability;

    void Awake()
    {
        if(Instance!=null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 如果跨场景保留
        DontDestroyOnLoad(gameObject); 

        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {

        if (!canMove)
        {
            moveInput = Vector3.zero;
            IsWalk = false;
            anim.SetBool("IsWalk", IsWalk);
            return;
        }

        float input_x = Input.GetAxisRaw("Horizontal");
        float input_y = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(input_x, 0, input_y).normalized;

        //判断是否行走并更新动画
        IsWalk = moveInput.magnitude > 0.1f;
        anim.SetBool("IsWalk", IsWalk);

        if (moveInput.magnitude > 0.1f) // 有效输入时才旋转
        {
            // 计算目标旋转角度（根据输入方向）
            float targetAngle = Mathf.Atan2(moveInput.x, moveInput.z) * Mathf.Rad2Deg;
            // 平滑旋转（避免瞬间转向，更自然）
            float smoothAngle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y, 
                targetAngle, 
                ref rotateSmoothVelocity, 
                rotateSmoothTime
            );
            // 应用旋转（只绕Y轴转，保持角色直立）
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
        }

        //冲刺逻辑
        if (Input.GetKey(KeyCode.LeftShift) && stamina != null && stamina.CanDash())
        {
            if(!freeDash)
                stamina.ConsumeSprint();
            moveInput *= dashMultiplier;
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector3(
            moveInput.x*moveSpeed, 
            moveInput.y*moveSpeed, 
            moveInput.z*moveSpeed
            );
    }

    public void SetSpeedMultiplier(float value)
    {
        dashMultiplier += value;
    }

    /* =======================
     * 掩体交互
     * ======================= */

    public void EnableFreeDash(bool value)
    {
        freeDash = value;
    }
}
