public interface IMovingEntity
{
    /// <summary>
    /// Aks the entity to move in the direction passed in parameter
    /// Negative value for left, positive value for right
    /// Value should be scaled between [0, 1]
    /// </summary>
    /// <param name="direction"></param>
    void Move(float direction);

    /// <summary>
    /// Aks the entity to jump in Y axis
    /// The force parameter define how high should entity jump
    /// Value should be positive, and scaled between [0, 1]
    /// </summary>
    /// <param name="force"></param>
    void Jump(float force);

    /// <summary>
    /// Set the entity looking direction by defining the right sprite, animation, or Y rotation
    /// This method should be called by Move method if current direction is not the same as the chosen direction
    /// </summary>
    /// <param name="direction"></param>
    void SetDirection(Direction direction);

    /// <summary>
    /// Ask the entity to perform a basic attack in the direction passed in parameter
    /// </summary>
    /// <param name="direction"></param>
    void Attacking(Direction direction);

    bool IsMoving { get; set; }
    bool IsGrounded { get; set; }
    void AirToGroundAttack();
}

public enum Direction
{
    Right,
    Left
}