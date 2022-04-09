using System;
using UnityEngine;

public class CharacterEntity : MonoBehaviour, IMovingEntity
{
    private static readonly float JUMP_FACTOR = 100;
    
    public Rigidbody2D rgbd;
    public SpriteRenderer spriteRenderer;
    public Collider2D groundCollider;
    public Animator animator;
    
    private Direction facingDirection = Direction.Right;

    public bool IsMoving { get; set; } 
    public bool IsGrounded { get; set; }

    private int groundLayerMask;
    private static readonly int AnimRunning = Animator.StringToHash("Running");
    private static readonly int AnimStartJump = Animator.StringToHash("StartJump");
    private static readonly int AnimGrounded = Animator.StringToHash("Grounded");
    private static readonly int AnimInAir = Animator.StringToHash("InAir");
    private static readonly int AnimAttack = Animator.StringToHash("Attacking");
    private static readonly int AnimAttackAir = Animator.StringToHash("AttackAir");

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rgbd = GetComponentInChildren<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        groundLayerMask = LayerMask.GetMask("EnvironmentPhysic", "Entity");
    }

    public void Move(float direction, float maxSpeed)
    {
        if (direction == 0)
        {
            if (!IsMoving && rgbd.velocity.x == 0) return;
            IsMoving = false;
            animator.SetBool(AnimRunning, false);
            rgbd.velocity = new Vector2(0, rgbd.velocity.y);
            return;
        }

        IsMoving = true;
        animator.SetBool(AnimRunning, true);

        var velocity = rgbd.velocity;
        var xSpeed = velocity.x;

        var moveDirection = direction < 0 ? Direction.Left : Direction.Right;
        if (moveDirection != facingDirection)
        {
            SetDirection(moveDirection);
            xSpeed = 0;
        }

        xSpeed += direction;
        velocity.x = moveDirection == Direction.Left ? Math.Max(-maxSpeed, xSpeed) : Math.Min(maxSpeed, xSpeed);
        rgbd.velocity = velocity;
    }

    public void Jump(float force)
    {
        if (!groundCollider.IsTouchingLayers(groundLayerMask))
        {
            return;
        }
        IsGrounded = false;
        animator.SetTrigger(AnimStartJump);
        rgbd.AddForce(new Vector2(0, force * JUMP_FACTOR));
    }
    
    public void DoubleJump(float force)
    {
        animator.SetTrigger(AnimStartJump);
        var velocity = rgbd.velocity;
        if (velocity.y < 0)
        {
            velocity.y = 0;
            rgbd.velocity = velocity;
        }
        rgbd.AddForce(new Vector2(0, force * JUMP_FACTOR));
    }

    public void SetDirection(Direction direction)
    {
        gameObject.transform.localScale = new Vector3(direction == Direction.Left ? -1 : 1, 1, 1);
        facingDirection = direction;
    }

    public void Attacking(Direction direction)
    {
        if (facingDirection != direction)
        {
            SetDirection(direction);
        }

        var velocity = rgbd.velocity;
        velocity.x *= 0.2f; 
        rgbd.velocity = velocity;
        animator.SetTrigger(AnimAttack);
    }

    public void AirToGroundAttack()
    {
        animator.SetTrigger(AnimAttackAir);
        var yVelocity = rgbd.velocity.y;
        yVelocity = yVelocity > 0 ? 0 : yVelocity * 1.5f;
        rgbd.velocity = new Vector2(0, yVelocity);
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 6)
        {
            animator.SetTrigger(AnimGrounded);
            animator.ResetTrigger(AnimStartJump);
            animator.SetBool(AnimInAir, false);
            IsGrounded = col.IsTouching(groundCollider);
        } 
    }
    
    public void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == 6)
        {
            animator.ResetTrigger(AnimGrounded);
            animator.SetBool(AnimInAir, true);
            IsGrounded = false;
        } 
    }
}