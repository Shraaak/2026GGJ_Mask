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

    [Header("State")]
    public PlayerState currentState = PlayerState.Normal;
    public bool canMove = true;
    public bool isHidden = false;

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

        // 可选，如果跨场景保留
        DontDestroyOnLoad(gameObject); 

        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!canMove)
        {
            moveInput = Vector3.zero;
            return;
        }

        float input_x = Input.GetAxisRaw("Horizontal");
        float input_y = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(input_x, 0, input_y).normalized;

        //冲刺逻辑
        if (Input.GetKey(KeyCode.LeftShift) && stamina != null && stamina.CanDash())
        {
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

    /* =======================
     * 掩体交互
     * ======================= */


}
