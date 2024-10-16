using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public class GhostController : SingletonMonoBehaviour<GhostController>
{
    [field: SerializeField, Range(0, 10)]
    public float MoveSpeed { get; private set; } = 1f;

    private Rigidbody2D rb;

    private Vector2 moveInput;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        GetInput();
        FlipPlayer();
    }

    private void FlipPlayer()
    {
        if (moveInput.x == 0) return;
        Vector3 currentScale = transform.localScale;
        if(moveInput.x > 0 && currentScale.x < 0)
        {
            currentScale.x *= -1;
            transform.localScale = currentScale;
        }
        else if(moveInput.x < 0 && currentScale.x > 0)
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
        rb.velocity = moveInput * MoveSpeed;
    }

    private void GetInput()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = Vector2.ClampMagnitude(moveInput, 1);
    }


}