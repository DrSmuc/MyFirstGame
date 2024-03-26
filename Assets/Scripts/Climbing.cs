using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    [Header("Reference")]
    public Transform orientation;
    public Rigidbody rb;
    public LayerMask whatIsWall;
    //public LedgeGrabbing lg;
    public PlayerMovement pm;

    [Header("Climbing")]
    public float climbSpeed;
    public float maxClimbTime;
    private float climbTimer;

    private bool climbing;

    [Header("ClimbjJumping")]
    public float climbJumpUpForce;
    public float climbJumpBackForce;

    public KeyCode jumpKey = KeyCode.Space;
    public int climbJumps;
    private int climbJumpsLeft;

    [Header("Detection")]
    public float detectionLenght;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;

    private Transform lastWall;
    private Vector3 lastWallNormal;
    public float minWallNormalAngleChange;

    [Header("Exiting")]
    public bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    private void Start()
    {
        //lg = GetComponent<LedgeGrabbing>();
    }

    private void Update()
    {
        WallCheck();
        StateMachine();

        if (climbing && !exitingWall)
            ClimbingMovement();
    }

    private void StateMachine()
    {
        // State 0 - Ledge Grabbing
        /*if (lg.holding)
        {
            if (climbing) StopClimbing();

            // everything else gets handled by the SubStateMachine() in the ledge grabbing script
        }*/
        // state 1 - climbing
        if (wallFront && Input.GetKey(KeyCode.W) && wallLookAngle < maxWallLookAngle && !exitingWall)
        {
            if (!climbing && climbTimer > 0)
                StartClimb();
            
            // timer
            if (climbTimer > 0)
                climbTimer -= Time.deltaTime;
            if (climbTimer < 0)
                StopClimbing();
        }

        else if (exitingWall)
        {
            if (climbing)
                StopClimbing();
            
            if (exitWallTime > 0)
                exitWallTimer -= Time.deltaTime;
            if (exitWallTimer < 0)
                exitingWall = false;
        }

        // state 3 - none
        else
        {
            if (climbing)
                StopClimbing();
        }

        if (Input.GetKeyDown(jumpKey) && wallFront && climbJumpsLeft > 0 && wallLookAngle < maxWallLookAngle)
            ClimbJump();
    }

    private void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLenght, whatIsWall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        bool newWall = frontWallHit.transform != lastWall || Mathf.Abs(Vector3.Angle(lastWallNormal, frontWallHit.normal)) > minWallNormalAngleChange;

        if ((wallFront && newWall) || pm.grounded)
        {
            climbTimer = maxClimbTime;
            climbJumpsLeft = climbJumps;
        }
    }

    private void StartClimb()
    {
        climbing = true;
        pm.climbing = true;

        lastWall = frontWallHit.transform;
        lastWallNormal = frontWallHit.normal;

        // camera change fov
    }

    private void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);

        // sound effect
    }

    private void StopClimbing()
    {
        climbing = false;
        pm.climbing = false;

        // particle effect
    }

    private void ClimbJump()
    {
        if (pm.grounded) return;
        //if (lg.holding || lg.exitingLedge) return;

        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 forceToApply = transform.up * climbJumpUpForce + frontWallHit.normal * climbJumpBackForce;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);

        climbJumpsLeft--;  
    }
}
