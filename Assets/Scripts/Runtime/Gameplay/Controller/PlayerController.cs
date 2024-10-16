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

    [Header("Audio")]
    [SerializeField]
    private AudioClip jumpClip;
    [SerializeField]
    private AudioClip dropClip;


    private Rigidbody2D rb;
    public Animator animator { get; private set; }

    private float moveInput;
    private bool jumped = false;
    private bool canJump = true;
    private bool isGrounded = false;
    public event Action<Collider2D> onHit;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        GetInput();
        FlipPlayer();
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
        GroundCheck();
        Movement();
    }

    private void Movement()
    {
        Vector2 currentVelocity = rb.velocity;
        currentVelocity.x = moveInput * MoveSpeed;
        rb.velocity = currentVelocity;
        animator.SetBool("IsMoving", moveInput != 0);
    }


    private void GroundCheck()
    {
        isGrounded = Physics2D.CircleCast(groundCheck.position, groundCheckRadius, Vector2.zero, 1, groundLayer) && rb.velocity.y > -4;

    }


    private void Jumping()
    {
        if(!canJump && isGrounded && rb.velocity.y <= 0)
        {
            animator.Play("Idle");
            canJump = true;
        }
        if (!jumped || !canJump) return;
        Vector2 currentVelocity = rb.velocity;
        currentVelocity.y = JumpForce;
        rb.velocity = currentVelocity;
        AudioSource.PlayClipAtPoint(jumpClip, groundCheck.position);
        canJump = false;
        animator.Play("MidAir");
    }

    private void GetInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        jumped = Input.GetButtonDown("Jump");
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Orb"))
        {
            onHit?.Invoke(collision);
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