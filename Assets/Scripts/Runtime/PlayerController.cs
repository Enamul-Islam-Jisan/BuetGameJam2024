using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : SingletonMonoBehaviour<PlayerController>
{
    [field: SerializeField, Range(0, 10)]
    public float MoveSpeed { get; private set; } = 1f;
    [field: SerializeField, Range(0, 10)]
    public float JumpForce { get; private set; } = 1f;
    [field: SerializeField, Range(0, 3)]
    public int MaxConCurrentJump { get; private set; } = 1;


    [Header("Ground Check")]
    [SerializeField, Range(0, 1)]
    private float groundCheckRadius;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private Transform groundCheck;


    private Rigidbody2D rb;

    private float moveInput;
    private bool jumped = false;
    private bool isGrounded = false;
    private bool groundCheckEnabled = true;
    private float groundCheckTimer;
    private int jumpCount;

    public static event Action ready;
    public event Action died;


    protected override void Awake()
    {
        base.Awake();
        ready?.Invoke();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        GetInput();
        FlipPlayer();
        GroundCheck();
        Jumping();
    }
    private void LateUpdate()
    {
        
    }

    private void FlipPlayer()
    {
        if (moveInput == 0) return;
        Vector3 currentScale = transform.localScale;
        if(moveInput > 0 && currentScale.x < 0)
        {
            currentScale.x *= -1;
            transform.localScale = currentScale;
        }
        else if(moveInput < 0 && currentScale.x > 0)
        {
            currentScale.x *= -1;
            transform.localScale = currentScale;
        }
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        Vector2 currentVelocity = rb.velocity;
        currentVelocity.x = moveInput * MoveSpeed;
        rb.velocity = currentVelocity;
    }


    private void GroundCheck()
    {
        if (!groundCheckEnabled && groundCheckTimer > 0)
        {
            groundCheckTimer -= Time.deltaTime;
            if (groundCheckTimer <= 0)
            {
                groundCheckTimer = 0;
                groundCheckEnabled = true;
            }
        }
        isGrounded = Physics2D.CircleCast(groundCheck.position, groundCheckRadius, Vector2.zero, 1, groundLayer) && groundCheckEnabled;
    }


    private void Jumping()
    {
        if (MaxConCurrentJump < 0) return;
        if (jumpCount > 0 && isGrounded || jumpCount >= MaxConCurrentJump)
        {
            jumpCount = 0;
            return;
        }
        if (!jumped) return;
        if (jumpCount == 0 && !isGrounded) return;
        Vector2 currentVelocity = rb.velocity;
        currentVelocity.y = JumpForce;
        rb.velocity = currentVelocity;
        jumpCount++;
        groundCheckEnabled = false;
        groundCheckTimer = 0.15f;
    }

    private void GetInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        jumped = Input.GetButtonDown("Jump");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pit"))
        {
            died?.Invoke();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Obstacle"))
        {
            died?.Invoke();
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (groundCheck)
        {
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
