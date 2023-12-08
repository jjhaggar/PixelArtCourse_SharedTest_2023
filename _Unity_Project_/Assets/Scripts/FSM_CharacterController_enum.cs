using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class FSM_CharacterController_enum : MonoBehaviour
{
    private enum PlayerState // Estados del personaje
    {
        Idle,
        Walk,
        Jump
    }
    private PlayerState currentState; // Estado actual del personaje

    private float speed = 5f; // Velocidad del personaje
    private float jumpForce = 9f; // Fuerza del salto
    private float moveHorizontal;
    private Vector2 movement;

    private bool isGrounded = true; // Booleano para comprobar si el personaje está en el suelo

    private Rigidbody2D rb; // Referencia al componente Rigidbody2D
    private SpriteRenderer sr; // Referencia al componente SpriteRenderer
    private Animator animator; // Referencia al componente Animator

    private void Start()
    {
        // Obtenemos las referencias a los componentes
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        UpdateCharacterStateText();
    }

    private void Update()
    {
        HandleInputs(); // Actualizamos el estado del personaje en función de la entrada del usuario
        UpdatePhysics(); // Aplicamos la física del personaje en función del estado actual
        UpdateAnimations(); // Actualizamos la animación del personaje en función del estado actual
    }

    private void ChangeState(PlayerState newState)
    {
        currentState = newState;
        Debug.Log("CHANGE currentState to " + currentState);
        UpdateCharacterStateText();
    }

    public void HandleInputs()
    {
        moveHorizontal = Input.GetAxis("Horizontal");

        switch (currentState)
        {
            case PlayerState.Idle:
                if (Input.GetButtonDown("Jump") && isGrounded)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Aplicamos una fuerza hacia arriba al personaje para que salte
                    isGrounded = false;
                    ChangeState(PlayerState.Jump);
                }
                else if (moveHorizontal != 0 && isGrounded)
                {
                    ChangeState(PlayerState.Walk);
                }
                break;
            case PlayerState.Walk:
                if (Input.GetButtonDown("Jump") && isGrounded)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Aplicamos una fuerza hacia arriba al personaje para que salte
                    isGrounded = false;
                    ChangeState(PlayerState.Jump);
                }
                else if (moveHorizontal == 0 && isGrounded)
                {
                    ChangeState(PlayerState.Idle);
                }
                break;
            case PlayerState.Jump:
                if (isGrounded && moveHorizontal == 0)
                {
                    ChangeState(PlayerState.Idle);
                }
                else if (isGrounded && moveHorizontal != 0)
                {
                    ChangeState(PlayerState.Walk);
                }
                break;
        }
    }

    public void UpdatePhysics()
    {

        switch (currentState)
        {
            case PlayerState.Idle:
                rb.velocity = new Vector2(0, 0);
                break;
            case PlayerState.Walk:
                movement = new Vector2(moveHorizontal * speed, rb.velocity.y);
                rb.velocity = movement;
                if (moveHorizontal == 0)
                    ChangeState(PlayerState.Idle);
                break;
            case PlayerState.Jump:

                movement = new Vector2(moveHorizontal * speed, rb.velocity.y);
                rb.velocity = movement;

                if (isGrounded && moveHorizontal == 0)
                    ChangeState(PlayerState.Idle);
                else if (isGrounded && moveHorizontal != 0)
                    ChangeState(PlayerState.Walk);

                break;
        }
    }

    public void UpdateAnimations()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                animator.Play("PotionMaster_Standing");
                break;
            case PlayerState.Walk:
                animator.Play("PotionMaster_Walking");
                break;
            case PlayerState.Jump:
                animator.Play("PotionMaster_Jumping");
                break;
        }

        // Volteamos horizontalmente el sprite si nos movemos a la izquierda
        if (rb.velocity.x < 0) sr.flipX = true;
        else if (rb.velocity.x > 0) sr.flipX = false;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
        }
    }

    void UpdateCharacterStateText() 
    {
        GameObject.Find("CharacterState").GetComponent<TextMeshProUGUI>().text = "Estado: " + currentState;
    }
}
