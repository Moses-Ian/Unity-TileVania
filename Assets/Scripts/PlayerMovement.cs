using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float accelSpeed = 2f;
    [SerializeField] float runSpeed = 0f;
    [SerializeField] float topRunSpeed = 10f;
    [SerializeField] float jumpSpeed = 10f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] float deathSpeedX = 1f;
    [SerializeField] float deathSpeedY = 10f;
    [SerializeField] float pauseTime = 1f;
    [SerializeField] float deathSpeed = 1f;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;

    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myCapsuleCollider;
    BoxCollider2D myFeetCollider;
    float baseGravity;
    bool isAlive = true;


    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myCapsuleCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        baseGravity = myRigidbody.gravityScale;
    }

    void Update()
    {
        if (!isAlive)
            return;

        Run();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    void OnMove(InputValue value)
    {
        if (!isAlive)
            return;

        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!isAlive)
            return;

        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            return;
        if (!value.isPressed)
            return;
        
        myRigidbody.velocity += new Vector2(0f, jumpSpeed);
    }

    void OnFire(InputValue value)
    {
        if (!isAlive)
            return;

        Instantiate(bullet, gun.position, transform.rotation);
    }

    void Run()
    {
        runSpeed = moveInput.x == 0 ? 0 : runSpeed + accelSpeed * moveInput.x;
        if (Mathf.Abs(runSpeed) > topRunSpeed)
            runSpeed = Mathf.Sign(moveInput.x) * topRunSpeed;
        Vector2 playerVelocity = new Vector2(runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

    }

    void FlipSprite()
    {
        bool isRunning = myRigidbody.velocity.x != 0f;
        if (isRunning)
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);

        myAnimator.SetBool("isRunning", isRunning);
    }

    void ClimbLadder()
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            // definitely not the best way to do this
            myRigidbody.gravityScale = baseGravity;
            myAnimator.SetBool("isClimbing", false);
            return;
        }

        Vector2 playerVelocity = new Vector2(myRigidbody.velocity.x, moveInput.y * climbSpeed);
        myRigidbody.velocity = playerVelocity;
        myRigidbody.gravityScale = 0f;

        myAnimator.SetBool("isClimbing", myRigidbody.velocity.y != 0);
    }

    void Die()
    {
        if (myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            myRigidbody.velocity = new Vector2(deathSpeedX * -Mathf.Sign(myRigidbody.velocity.x), deathSpeedY);
            StartCoroutine(DeathPause());
        }
    }

    IEnumerator DeathPause()
    {
        Time.timeScale = 0f;
        float pauseEndTime = Time.realtimeSinceStartup + pauseTime;
        while (Time.realtimeSinceStartup < pauseEndTime)
            yield return 0;
        Time.timeScale = deathSpeed;
    }
}
