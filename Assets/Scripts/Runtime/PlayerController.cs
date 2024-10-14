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
    public event Action onHit;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        GetInput();
        FlipPlayer();
        GroundCheck();
        Jumping();
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
        isGrounded = Physics2D.CircleCast(groundCheck.position, groundCheckRadius, Vector2.zero, 1, groundLayer);
    }


    private void Jumping()
    {
        if (!jumped) return;
        if (!isGrounded) return;
        Vector2 currentVelocity = rb.velocity;
        currentVelocity.y = JumpForce;
        rb.velocity = currentVelocity;
    }

    private void GetInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        jumped = Input.GetButtonDown("Jump");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Obstacle"))
        {
            onHit?.Invoke();
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