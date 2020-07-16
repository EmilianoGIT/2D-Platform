using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{

    //Config
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 10f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(0f, 25f);

    //State
    bool isAlive = true;
    bool isExiting = false;
    bool isGrabbingLadders = false;

    //Chached component references
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeet;
    float gravityScaleAtStart;
    float animatorSpeedAtStart;

    void Start()
    {

        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeet = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidBody.gravityScale;
        animatorSpeedAtStart = myAnimator.speed;

    }

    // Update is called once per frame
    void Update()
    {
        while (!isAlive || isExiting) { return; }
            
        Run();
        Jump();
        FlipSprite();
        ClimbLadder();
        Die();

    }


    private void Run()
    {
        //value is between -1 to +1
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal");

        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;

        bool playerHasHorizonatalSpeed = Math.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizonatalSpeed)
            myAnimator.SetBool("Running", true);
        else
            myAnimator.SetBool("Running", false);

    }

    private void ClimbLadder()
    {
        bool pressedUp = Math.Abs(CrossPlatformInputManager.GetAxis("Vertical")) > 0;
        //        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Climbing")) || (!pressedUp && !isGrabbingLadders))
        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Climbing")) || (!pressedUp && myRigidBody.gravityScale != 0))
        {
            myRigidBody.gravityScale = gravityScaleAtStart;
            myAnimator.speed = animatorSpeedAtStart;
            myAnimator.SetBool("Climbing", false);
            //isGrabbingLadders = false;
            return;
        }

        //isGrabbingLadders = true;

        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
        Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, controlThrow * climbSpeed);
        myRigidBody.velocity = climbVelocity;
        myRigidBody.gravityScale = 0f;
        myAnimator.SetBool("Climbing", true);
        myAnimator.speed = 0f;

        bool playerHasHorizontalSpeed = Math.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            myAnimator.speed = animatorSpeedAtStart;
            myAnimator.SetBool("Climbing", playerHasHorizontalSpeed);
        }
    }

    private void Jump()
    {

        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Ground"))
           )
        { return; }

        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {


                //Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
                Vector2 jumpVelocityToAdd = new Vector2(myRigidBody.velocity.x, jumpSpeed);
                myRigidBody.velocity += jumpVelocityToAdd;

        }

    }

    void Die()
    {
        if (myFeet.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards")) || myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            myRigidBody.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();

        }
    }

    public void Exit()
    {
        isExiting = true;
    }

    public void StopExiting()
    {
        isExiting = false;
    }
    private void FlipSprite()
    {
        bool playerHasHorizonatalSpeed = Math.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizonatalSpeed)
        {
            transform.localScale = new Vector2(Math.Abs(transform.localScale.x) * Math.Sign(myRigidBody.velocity.x), transform.localScale.y);
        }
    }


    public bool GetExitingStatus()
    {
        return isExiting;
    }
    /*
     * My first thought..it works but there could have been done more in terms of readability
     * 
    private void FlipSprite(float controlThrow)
    {
        if (controlThrow < 0)
            transform.localScale = new Vector3(Math.Abs(transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);
        else
        {
            transform.localScale = new Vector3(Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
    */

}
