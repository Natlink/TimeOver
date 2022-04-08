using System;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private IMovingEntity entity;

    private bool jump;
    private float moveDirection;
    private readonly float inputDelay = 0.3f;

    private float lastJumpTime = 0;

    private Direction nextAttackDirection;
    private float lastAttackTime;
    private bool attack;
    private bool groundAttack;
    
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
            jump = true;
            return;
        }
        
        moveDirection = Input.GetAxis("Horizontal");

        if (lastAttackTime + inputDelay > time)
        {
            return;
        }

        if (!entity.IsGrounded && Input.GetKeyDown(KeyCode.DownArrow))
        {
            groundAttack = true;
            entity.AirToGroundAttack();
            return;
        }
        
        var leftAttack = moveDirection <= 0 && Input.GetKey(KeyCode.LeftArrow);
        var rightAttack = moveDirection >= 0 && Input.GetKey(KeyCode.RightArrow);
        if (!leftAttack && !rightAttack) return;
        
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
            entity.Jump(1);
            jump = false;
            lastJumpTime = time;
            return;
        }

        entity.Move(moveDirection);
        
        if (!attack) return;
        entity.Attacking(nextAttackDirection);
        attack = false;
        lastAttackTime = time;
    }
}
