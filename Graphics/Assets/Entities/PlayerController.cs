using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private InputMaster inputMaster;
    public Rigidbody2D rb;
    PlayerScript playerScript;
    public Transform feet, head;
    public LayerMask whatIsGround;

    public float speed, inAirSpeed, maxMoveSpeed, dragForce, jumpForce, downGravity, slideSpeed, wallJumpDistance;
    float moveValue;
    [HideInInspector]
    public bool isGrounded, sliding, stopSlide, wallJumping;

    private void Awake()
    {
        playerScript = GetComponent<PlayerScript>();

        rb = this.GetComponent<Rigidbody2D>();//players rigidbody
    }
    void Start()
    {
        inputMaster = playerScript.input;//for input keys

        //what to do if different keys are pressed
        inputMaster.Player.Jump.performed += _ => Jump();

        inputMaster.Player.Slide.performed += _ => Slide();
        inputMaster.Player.Slide.canceled += _ => stopSlide = true;
    }
    void Update()
    {
        //movement
        moveValue = (int)inputMaster.Player.Movement.ReadValue<float>();

        playerScript.anim.SetBool("sliding", sliding);
    }
    private void FixedUpdate()
    {
        moveCharacter();
    }
    void moveCharacter()
    {
        isGrounded = Physics2D.OverlapCircle(feet.position, 0.3f, whatIsGround);
        playerScript.anim.SetBool("inAir", !isGrounded);
        //running animations
        if (isGrounded && !(moveValue == 0)) playerScript.anim.SetBool("isRunning", true);
        else if (moveValue == 0) playerScript.anim.SetBool("isRunning", false);
        //flipping player
        bool canFlip = !wallJumping && !sliding;
        if (moveValue > 0 && canFlip) transform.localScale = new Vector3(1, 1, 1);
        if (moveValue < 0 && canFlip) transform.localScale = new Vector3(-1, 1, 1);

        //movement
        float xVel = rb.velocity.x;
        if (!sliding)//can't move while sliding
        {
            bool canMove = ((moveValue == 1 && xVel < maxMoveSpeed) || (moveValue == -1 && xVel > -maxMoveSpeed));
            if (isGrounded && canMove) rb.AddForce(new Vector2(moveValue, 0) * speed);
            else if (!isGrounded && canMove) rb.AddForce(new Vector2(moveValue, 0) * inAirSpeed);

            if (moveValue == 0)//if both keys are not pressed
            {
                if (xVel > 0) rb.AddForce(new Vector2(-dragForce, 0));
                else if (xVel < 0) rb.AddForce(new Vector2(dragForce, 0));

                if (rb.velocity.x < 1 && rb.velocity.x > -1) rb.velocity = new Vector2(0f, rb.velocity.y);//so it does slowly move as dragforce changes
            }
        }
        //jumping down force
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (downGravity - 1) * Time.deltaTime;//falls faster after jump/ less floaty feeling
        }

        if (stopSlide)
        {
            if (!Physics2D.OverlapCircle(head.position, 0.5f, whatIsGround))//head is not touching ceiling
            {
                sliding = false;
                stopSlide = false;

                GetComponent<BoxCollider2D>().offset = new Vector2(-0.008f, -0.2054f);
                GetComponent<BoxCollider2D>().size = new Vector2(0.9f, 2.3938f);
            }
        }
    }
    void Jump()
    {
        if (sliding)//cancels slide if sliding
        {
            if (!Physics2D.OverlapCircle(head.position, 0.5f, whatIsGround))//head is not touching ceiling
            {
                sliding = false;
                stopSlide = false;

                GetComponent<BoxCollider2D>().offset = new Vector2(-0.008f, -0.2054f);
                GetComponent<BoxCollider2D>().size = new Vector2(0.9f, 2.3938f);
            }
            else return;
        }

        if (!isGrounded)//wall jump
        {
            Vector2 rightSide = new Vector2(transform.position.x + (transform.localScale.x * 0.5f), transform.position.y);
            Vector2 leftSide = new Vector2(transform.position.x - (transform.localScale.x * 0.5f), transform.position.y);
            bool hitRight = Physics2D.OverlapCircle(rightSide, 0.1f, whatIsGround);
            bool hitLeft = Physics2D.OverlapCircle(leftSide, 0.1f, whatIsGround);
            if (hitRight || hitLeft)//can wall jump
            {
                float facing = transform.localScale.x;
                if ((facing == 1 && hitRight) || (facing == -1 && hitLeft)) rb.velocity = new Vector2(-wallJumpDistance, jumpForce);
                else if ((facing == -1 && hitRight) || (facing == 1 && hitLeft)) rb.velocity = new Vector2(wallJumpDistance, jumpForce);

                playerScript.anim.SetTrigger("wallJump");
                if (hitRight) transform.localScale = new Vector3(-transform.localScale.x, 1, 1);

                wallJumping = true;
                StartCoroutine(endWallJump());//wait for wall jump animation to flip again

                FindObjectOfType<AudioManager>().Play("jump");
            }
        }
        else//regular jump
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            playerScript.anim.SetTrigger("takeOff");
            isGrounded = false;

            FindObjectOfType<AudioManager>().Play("jump");
        }
    }
    void Slide()
    {
        //if (!isGrounded) return;

        sliding = true;

        GetComponent<BoxCollider2D>().offset = new Vector2(-0.015f, -0.6563f);
        GetComponent<BoxCollider2D>().size = new Vector2(1.3f, 1.49204f);

        if (rb.velocity.y > 0) rb.velocity = new Vector2(rb.velocity.x, 0);//stop jump to slide

        if (transform.localScale.x == 1) rb.AddForce(Vector2.right * slideSpeed, ForceMode2D.Impulse);//player is facing right
        else if (transform.localScale.x == -1) rb.AddForce(Vector2.left * slideSpeed, ForceMode2D.Impulse);//player is facing left

        FindObjectOfType<AudioManager>().Play("slide");
    }
    IEnumerator endWallJump()
    {
        yield return new WaitForSeconds(0.25f);
        wallJumping = false;
    }
}
