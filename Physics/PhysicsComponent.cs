using System.Numerics;

namespace CreateGame.Physics;

public struct PhysicsState
{
    public Vector3 Velocity;
    public Vector3 Acceleration;
    public float GravityScale;
    public bool IsGrounded;

    public PhysicsState()
    {
        Velocity = Vector3.Zero;
        Acceleration = Vector3.Zero;
        GravityScale = 1.0f;
        IsGrounded = false;
    }
}