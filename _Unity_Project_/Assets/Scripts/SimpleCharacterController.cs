using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCharacterController : MonoBehaviour
{
    private float speed = 5.0f;
    private float jumpForce = 9.0f;

    private bool isJumping = false;
    private Animator animator;
    private Rigidbody2D rb2d;

    private enum State { Idle, Walking, Jumping }
    private State state = State.Idle;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");

        Vector2 movement = new Vector2(moveX * speed, rb2d.velocity.y);
        rb2d.velocity = movement;

        if (!isJumping)
        {
            if (Mathf.Abs(moveX) > 0)
            {
                //transform.localScale = new Vector2(Mathf.Sign(moveX), 1);
                state = State.Walking;
            }
            else
            {
                state = State.Idle;
            }

            if (Input.GetButtonDown("Jump"))
            {
                rb2d.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                isJumping = true;
                state = State.Jumping;
            }
        }

        ChangeAnimationState();
    }

    void ChangeAnimationState()
    {
        switch (state)
        {
            case State.Idle:
                animator.Play("PotionMaster_Standing");
                break;
            case State.Walking:
                animator.Play("PotionMaster_Walking");
                break;
            case State.Jumping:
                animator.Play("PotionMaster_Jumping");
                break;
        }

        // Volteamos horizontalmente el sprite si nos movemos a la izquierda
        if (rb2d.velocity.x != 0)
            transform.localScale = new Vector2(Mathf.Sign(rb2d.velocity.x), 1);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isJumping = false;
        }
    }
}
