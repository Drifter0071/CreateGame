using CreateGame.Engine;
using System.Numerics;

namespace CreateGame.Physics;
public class RigidBody3D : PrimitiveObject
{
    public PhysicsState Physics { get; set; } = new();
    public float Mass { get; set; } = 1.0f;
    public float Drag { get; set; } = 5.0f;

    // AABB bounds
    public Vector3 Min => Position - (Size / 2);
    public Vector3 Max => Position + (Size / 2);

    public void ApplyForce(Vector3 force)
    {
        PhysicsState state = Physics;
        state.Acceleration += force / Mass;
        Physics = state;
    }

    public bool Intersects(RigidBody3D other)
    {
        return (Min.X < other.Max.X && Max.X > other.Min.X) &&
                (Min.Y < other.Max.Y && Max.Y > other.Min.Y) &&
                (Min.Z < other.Max.Z && Max.Z > other.Min.Z);
    }
}