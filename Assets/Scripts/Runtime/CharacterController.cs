using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : MonoBehaviour
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
    private int jumpCount;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        GetInput();
        Jumping();
    }
    private void FixedUpdate()
    {
        Movement();
        GroundCheck();
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
        if (MaxConCurrentJump < 0) return;
        if (jumpCount >= MaxConCurrentJump)
        {
            if (!isGrounded) return;
            jumpCount = 0;
        }
        if (!jumped) return;
        if (jumpCount == 0 && !isGrounded) return;
        Vector2 currentVelocity = rb.velocity;
        currentVelocity.y = JumpForce;
        rb.velocity = currentVelocity;
        jumpCount++;
    }

    private void GetInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        jumped = Input.GetButtonDown("Jump");
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck)
        {
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
