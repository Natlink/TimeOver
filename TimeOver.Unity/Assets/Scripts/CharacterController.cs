using System;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private IMovingEntity entity;

    private bool jump;
    private bool doubleJump;
    private float moveDirection;
    private readonly float inputDelay = 0.3f;

    private float lastJumpTime = 0;

    private Direction nextAttackDirection;
    private float lastAttackTime;
    private bool attack;
    private bool groundAttack;
    private int doubleJumpCount;
    
    [Range(0,2)]
    public float attackMovementSpeedFactor = 0.5f;
    [Range(0,2)]
    public float movementSpeed = 1;
    [Range(1,50)]
    public float maxMovementSpeed = 10;
    [Range(1,10)]
    public float jumpHeight = 20;
    [Range(0,10)]
    public int maxDoubleJump;
    
    private void Start()
    {
        entity = GetComponent<IMovingEntity>();
    }

    private void Update()
    {
        if (groundAttack)
        {
            return;
        }

        var time = Time.time;
        if (Input.GetKeyDown(KeyCode.Z) && time - lastJumpTime > inputDelay)
        {
            if (entity.IsGrounded)
            {
                jump = true;
            }
            else if (doubleJumpCount < maxDoubleJump)
            {
                doubleJump = true;
            }
            return;
        }
        
        moveDirection = Input.GetAxis("Horizontal");

        if (lastAttackTime + inputDelay > time)
        {
            moveDirection *= attackMovementSpeedFactor;
            return;
        }

        if (entity.IsGrounded)
        {
            doubleJumpCount = 0;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            groundAttack = true;
            entity.AirToGroundAttack();
            return;
        }
        
        var leftAttack = moveDirection <= 0 && Input.GetKey(KeyCode.LeftArrow);
        var rightAttack = moveDirection >= 0 && Input.GetKey(KeyCode.RightArrow);
        if (!leftAttack && !rightAttack) return;
        
        moveDirection *= attackMovementSpeedFactor;
        nextAttackDirection = rightAttack ? Direction.Right : Direction.Left;
        attack = true;
    }

    private void FixedUpdate()
    {
        if (groundAttack && entity.IsGrounded)
        {
            groundAttack = false;
            return;
        }
        
        var time = Time.time;
        
        if (jump)
        {
            entity.Jump(jumpHeight);
            jump = false;
            lastJumpTime = time;
            return;
        }

        if (doubleJump)
        {
            entity.DoubleJump(jumpHeight);
            lastJumpTime = time;
            doubleJumpCount++;
            doubleJump = false;
            return;
        }

        entity.Move(moveDirection*movementSpeed, maxMovementSpeed);
        
        if (!attack) return;
        entity.Attacking(nextAttackDirection);
        attack = false;
        lastAttackTime = time;
    }
}
