using System;
using UnityEngine;


public class TraditionController : MonoBehaviour
{
    public float moveSpeed = 5f;         // 移动速度
    public float jumpHeight = 2f;        // 跳跃高度
    public float swipeThreshold = 100f;  // 滑动阈值（判断滑动距离）
    public float gravity = 9.81f;       // 重力
    public float doubleTapTime = 0.3f;   // 双击的时间间隔
    

    private Rigidbody rb;
    [SerializeField] private Animator animator;

    private float moveDirection = 1f;
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;

    private bool isJumping = false;
    private float lastTapTime = 0f;
    private bool isSwiping = false;
    private bool isHoldingFlashlight = false;
    

    private bool isGround = false;
    private bool isFingerMove = false;
    public float groundCheckDistance = 0.2f;

    public Transform modelTransform;
    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        Debug.Log(rb.velocity);
        HandleTouchInput();
        isGround = CheckGround();
       // isFingerMove = isFingerMoving();
        
        
        // 只针对水平的
        
       
        
        HandleMovement();
    }

    // 检测角色是否在地面
    private bool CheckGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(rb.position, Vector3.down, out hit, groundCheckDistance, LayerMask.GetMask("Ground")))
        {
            return true;
        }
        return false;
    }
    
    // 处理触摸输入
    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
                
                rb.velocity = Vector3.zero;

                // 检查双击（双击屏幕拿出手电筒）
                if (Time.time - lastTapTime < doubleTapTime)
                {
                    ToggleFlashlight();  // 如果双击，拿出手电筒
                }
                lastTapTime = Time.time;
            }
            
            
            // 仅对于平移
            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                touchEndPos = touch.position; 
                isSwiping = true;
                handleSwipe(touchStartPos, touchEndPos);
            }
            
            // 触摸结束时
            if (touch.phase == TouchPhase.Ended)
            {
                isSwiping = false;
              
                touchEndPos = Vector2.zero;
                touchStartPos = Vector2.zero;
                
                if (!isHoldingFlashlight && !isJumping)
                {
                    animator.CrossFade("Idle", 0.1f);
                }
                animator.SetBool("isRunning", false);
                
                /*
                if(rb.velocity.y != 0f)
                    rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
                else
                {
                    rb.velocity = Vector3.zero;
                }*/
                
                rb.velocity = Vector3.zero;
                
                
            }
        }
    }

    // 处理平移滑动的方向
    private void handleSwipe(Vector2 startPos, Vector2 endPos)
    {
        Vector2 swipeDirection = endPos - startPos;

        if (swipeDirection.magnitude > swipeThreshold)  // 判断滑动是否有效
        {
            // 只是给速度方向但没有发生实际位移
            if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
            {
                animator.SetBool("isRunning", true);  // 播放奔跑动画
                animator.CrossFade("Running", 0.01f);
                // 左右滑动
                if (swipeDirection.x > 0)
                {
                    // 向右滑动
                    MoveRight();
                }
                else if(swipeDirection.x < 0)
                {
                    // 向左滑动
                    MoveLeft();
                }
            }
            else
            {
                // 上下滑动
                Jump();
                isJumping = true;
            }
        }
    }
    
    

    // 移动到右边
    private void MoveRight()
    {
        Vector3 currentVelocity = rb.velocity;
        currentVelocity.z = -moveDirection * moveSpeed;  // 水平移动
        rb.velocity = currentVelocity;
        modelTransform.rotation = Quaternion.Euler(0, 180, 0);  // 设置朝向
        
    }

    // 移动到左边
    private void MoveLeft()
    {
        Vector3 currentVelocity = rb.velocity;
        currentVelocity.z = moveDirection * moveSpeed;  // 水平移动
        rb.velocity = currentVelocity;
        modelTransform.rotation = Quaternion.Euler(0, -60, 0);  // 设置朝向
        
    }


    private bool isFingerMoving()
    {
        Vector2 curFingerPos = touchEndPos;
        if(curFingerPos.y != touchStartPos.y)
            return true;
        return false;
    }
    
    // 跳跃 -原地
    private void Jump()
    {
        if (isGround && isSwiping)
        {
            rb.velocity = new Vector3(0f, Mathf.Sqrt(jumpHeight * -2f * gravity), 0f);
            animator.CrossFade("Jumping", 0.01f);
            animator.SetBool("isJumping", true);
        }
    }

    // 处理角色的移动和重力
    private void HandleMovement()
    {
        Vector3 currentVelocity = rb.velocity;
        
        if (isGround)
        {
            // 如果角色在地面，y轴速度归零，停止下落
            if (currentVelocity.y < 0)
            {
                currentVelocity.y = 0f;
                animator.SetBool("isJumping", false);
                isJumping = false;
            }
            
                
        }
        // 设置 Rigidbody 的速度
        rb.velocity = currentVelocity;
    }

    // 双击屏幕时触发拿出手电筒
    private void ToggleFlashlight()
    {
        isHoldingFlashlight = !isHoldingFlashlight;
        if (isHoldingFlashlight)
        {
            animator.SetBool("isHolding", true);
            animator.CrossFade("hold", 0.05f);
            Debug.Log("Flashlight out!");
        }
        else
        {
            animator.SetBool("isHolding", false);
            animator.CrossFade("Idle", 0.01f);
            Debug.Log("Flashlight in!");
        }
    }
}

