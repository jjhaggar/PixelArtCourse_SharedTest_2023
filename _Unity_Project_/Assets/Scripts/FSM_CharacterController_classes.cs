using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FSM_CharacterController_classes : MonoBehaviour
{
    private float speed = 5f;
    private float jumpForce = 9f;

    private State currentState;
    private bool isGrounded = true;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        currentState = new IdleState(this);
    }

    private void Update()
    {
        currentState.HandleInputs();
        currentState.UpdatePhysics();
        currentState.UpdateAnimations();
    }

    public void SetState(State state)
    {
        currentState = state;
        UpdateCharacterStateText();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
        }
    }


    /* abstract: Esta palabra clave se utiliza para declarar una clase o un miembro de 
     * una clase que está incompleto y debe implementarse en una clase derivada. 
     * Las clases y los métodos marcados como abstract no pueden instanciarse directamente. 
     * En su lugar, necesitas crear una subclase que implemente los métodos abstractos.
     * 
     * La clase State es una clase abstracta. Esto significa que no puedes crear un objeto 
     * de tipo State directamente. En cambio, defines subclases (como IdleState y WalkState), 
     * que implementan los métodos abstractos de la clase */
    public abstract class State
    {
        // Referencia al controlador del personaje
        protected FSM_CharacterController_classes player;

        // Constructor de la clase, se ejecuta cuando se crea una instancia de un objeto.
        protected State(FSM_CharacterController_classes player)
        {
            this.player = player;
        }

        // Métodos abstractos que se deben implementar en las clases que heredan de "State".
        public abstract void HandleInputs();
        public abstract void UpdatePhysics();
        public abstract void UpdateAnimations();
    }

    public class IdleState : State
    {

        // Constructor de la clase IdleState, llama al constructor de la clase base.
        public IdleState(FSM_CharacterController_classes player) : base(player) { }

        // Implementación del método HandleInputs para el estado Idle.
        public override void HandleInputs()
        {
            float moveHorizontal = Input.GetAxis("Horizontal");

            if (Input.GetButtonDown("Jump") && player.isGrounded)
            {
                player.rb.velocity = new Vector2(player.rb.velocity.x, player.jumpForce);
                player.isGrounded = false;
                player.SetState(new JumpState(player));
            }
            else if (moveHorizontal != 0 && player.isGrounded)
            {
                player.SetState(new WalkState(player));
            }
        }

        public override void UpdatePhysics()
        {
            player.rb.velocity = new Vector2(Input.GetAxis("Horizontal") * player.speed, player.rb.velocity.y);
        }

        public override void UpdateAnimations()
        {
            player.animator.Play("PotionMaster_Standing");
        }
    }

    public class WalkState : State
    {
        public WalkState(FSM_CharacterController_classes player) : base(player) { }

        public override void HandleInputs()
        {
            float moveHorizontal = Input.GetAxis("Horizontal");

            if (Input.GetButtonDown("Jump") && player.isGrounded)
            {
                player.rb.velocity = new Vector2(player.rb.velocity.x, player.jumpForce);
                player.isGrounded = false;
                player.SetState(new JumpState(player));
            }
            else if (moveHorizontal == 0 && player.isGrounded)
            {
                player.SetState(new IdleState(player));
            }
        }

        public override void UpdatePhysics()
        {
            player.rb.velocity = new Vector2(Input.GetAxis("Horizontal") * player.speed, player.rb.velocity.y);
        }

        public override void UpdateAnimations()
        {
            player.animator.Play("PotionMaster_Walking");

            if (player.rb.velocity.x < 0) player.sr.flipX = true;
            else if (player.rb.velocity.x > 0) player.sr.flipX = false;
        }
    }

    public class JumpState : State
    {
        public JumpState(FSM_CharacterController_classes player) : base(player) { }

        public override void HandleInputs()
        {
            float moveHorizontal = Input.GetAxis("Horizontal");

            if (player.isGrounded && moveHorizontal == 0)
            {
                player.SetState(new IdleState(player));
            }
            else if (player.isGrounded && moveHorizontal != 0)
            {
                player.SetState(new WalkState(player));
            }
        }

        public override void UpdatePhysics()
        {
            player.rb.velocity = new Vector2(Input.GetAxis("Horizontal") * player.speed, player.rb.velocity.y);
        }

        public override void UpdateAnimations()
        {
            player.animator.Play("PotionMaster_Jumping");

            if (player.rb.velocity.x < 0) player.sr.flipX = true;
            else if (player.rb.velocity.x > 0) player.sr.flipX = false;
        }
    }

    void UpdateCharacterStateText()
    {
        GameObject.Find("CharacterState").GetComponent<TextMeshProUGUI>().text = "Estado: " + currentState.GetType().Name;
    }
}
