using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    public Animator anim;

    public float moveSpeed; // CONTROLS PLAYER MOVEMENT SPEED
    public float jumpForce;
    private float moveDirection;
    public float checkRadius;
    
    public LayerMask groundObjects;
    
    // GROUND AND CEILING CHECK VARIABLES
    public Transform ceilingCheck;
    public Transform groundCheck;
    
    private bool faceRight = true;
    private bool isJumping = false;
    private bool isGrounded;


    public GameManager gameManager;
    // Awake is called before Start()
    void Awake(){
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        KillBlockScript.onKill += OnDeath;
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();
        Animate();
    }

    private void FixedUpdate(){
        // CHECK IF PLAYER ON GROUND
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundObjects);

        Move();
    }

    // MOVE METHOD
    private void Move(){
        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
        if(isJumping){
            rb.AddForce(new Vector2(0f, jumpForce));
        }
        isJumping = false;
    }

    // ANIMATION METHOD
    private void Animate(){
        if(moveDirection > 0 && !faceRight)
        {
            FlipCharacter();
        } else if (moveDirection < 0 && faceRight){
            FlipCharacter();
        }

    }

    // Collider Methods
    void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.gameObject.tag == "Coin")
        {
            Score.score += 10;
            Destroy(coll.gameObject);
        }
        
        if(coll.gameObject.tag == "Platform")
        {
            this.transform.parent = coll.transform;
        }
        
        if(coll.gameObject.tag == "Trap")
        {
            gameManager.GameOver();
        }

        if(coll.gameObject.tag == "Win")
        {
            gameManager.GameOver();
        }

    }

    void OnCollisionExit2D(Collision2D coll)
    {
        if(coll.gameObject.tag == "Platform")
        {
            this.transform.parent = null;
        }
    }

    // METHOD TO FLIP CHAR (WHEN TURNING LEFT/RIGHT - IT FLIPS PLAYER IMAGE)
    private void FlipCharacter()
    {
        faceRight = !faceRight; // Inversebool (allows for rotating player model 180 degrees)
        transform.Rotate(0f, 180f, 0f);
    }

    private void Inputs()
    {
        moveDirection = Input.GetAxis("Horizontal");
        anim.SetFloat("Speed", Mathf.Abs(moveDirection));
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            isJumping = true;
        }
    }

    // DEATH CONDITION
    void OnDeath()
    {
        GetComponent<BoxCollider2D>().enabled = false;
    }

    void OnDestroy()
    {
        KillBlockScript.onKill -= OnDeath;
    }
}
