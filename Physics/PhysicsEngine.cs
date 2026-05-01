using System.Numerics;
using CreateGame.Engine;

namespace CreateGame.Physics;

public static class PhysicsEngine
{
    private static readonly List<RigidBody3D> _bodies = [];
    public static Vector3 Gravity { get; set; } = new Vector3(0, -28.1f, 0);

    public static void Register(RigidBody3D body) => _bodies.Add(body);

    public static void Update(float deltaTime, List<PrimitiveObject> staticColliders)
    {
        foreach (var body in _bodies)
        {
            PhysicsState state = body.Physics;

            if (!state.IsGrounded)
            {
                state.Acceleration += Gravity * state.GravityScale;
            }

            // v = u + at
            state.Velocity += state.Acceleration * deltaTime;

            // apply drag
            state.Velocity *= (1.0f / (1.0f + body.Drag * deltaTime));

            // Save state so ResolveCollisions uses the updated velocity
            body.Physics = state;

            // AABB resolve (moves position internally and updates body.Physics)
            ResolveCollisions(body, staticColliders, deltaTime);

            // Read the state back after collisions (Velocity may be zeroed, IsGrounded updated)
            state = body.Physics;

            // reset acceleration for the next frame
            state.Acceleration = Vector3.Zero;
            body.Physics = state;
        }
    }

    private static void ResolveCollisions(RigidBody3D body, List<PrimitiveObject> staticColliders, float deltaTime)
    {
        PhysicsState state = body.Physics;
        state.IsGrounded = false;

        // X-Axis
        body.Position += new Vector3(state.Velocity.X * deltaTime, 0, 0);
        foreach (var collider in staticColliders)
        {
            if (body.Intersects(collider as RigidBody3D ?? new RigidBody3D { Position = collider.Position, Size = collider.Size }))
            {
                float offset = (body.Size.X / 2 + collider.Size.X / 2);
                if (state.Velocity.X > 0) body.Position = new Vector3(collider.Position.X - offset, body.Position.Y, body.Position.Z);
                else if (state.Velocity.X < 0) body.Position = new Vector3(collider.Position.X + offset, body.Position.Y, body.Position.Z);
                state.Velocity.X = 0;
            }
        }

        // Z-Axis
        body.Position += new Vector3(0, 0, state.Velocity.Z * deltaTime);
        foreach (var collider in staticColliders)
        {
            if (body.Intersects(collider as RigidBody3D ?? new RigidBody3D { Position = collider.Position, Size = collider.Size }))
            {
                float offset = (body.Size.Z / 2 + collider.Size.Z / 2);
                if (state.Velocity.Z > 0) body.Position = new Vector3(body.Position.X, body.Position.Y, collider.Position.Z - offset);
                else if (state.Velocity.Z < 0) body.Position = new Vector3(body.Position.X, body.Position.Y, collider.Position.Z + offset);
                state.Velocity.Z = 0;
            }
        }

        // Y-Axis
        body.Position += new Vector3(0, state.Velocity.Y * deltaTime, 0);
        foreach (var collider in staticColliders)
        {
            if (body.Intersects(collider as RigidBody3D ?? new RigidBody3D { Position = collider.Position, Size = collider.Size }))
            {
                float offset = (body.Size.Y / 2 + collider.Size.Y / 2);
                if (state.Velocity.Y > 0) // Top hit
                {
                    body.Position = new Vector3(body.Position.X, collider.Position.Y - offset, body.Position.Z);
                    state.Velocity.Y = 0;
                }
                else if (state.Velocity.Y < 0) // Bottom hit
                {
                    body.Position = new Vector3(body.Position.X, collider.Position.Y + offset, body.Position.Z);
                    state.Velocity.Y = 0;
                    state.IsGrounded = true;
                }
            }
        }
        body.Physics = state;
    }
}